using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;


namespace BlackHat_Server
{
    public class MsgManager
    {
        NetworkStream nStream;

        /// <summary>
        /// Dispose dello stream di questo oggetto
        /// </summary>
        public void StreamDispose()
        {
            this.nStream.Dispose();
        }
        //-----------------------------------------

        /// <summary>
        /// Costruttore senza info trasferimento
        /// </summary>
        /// <param name="ctor_Client"></param>
        public MsgManager(NetworkStream ctor_Stream)
        {
            nStream = ctor_Stream;
        }
        //--------------------------------------



        /// <summary>
        /// Manda Un Messaggio in Chiaro | Timeout Distrugge Stream
        /// </summary>
        public bool SendMessage(string msg, int millisecTimeOt)
        {
            try
            {

                this.nStream.WriteTimeout = millisecTimeOt;

                byte[] mess = System.Text.Encoding.ASCII.GetBytes(msg);


                this.nStream.Write(mess, 0, mess.Length);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        //--------------------------

        /// <summary>
        /// Manda Un Messaggio Cryptato | Timeout Distrugge Stream
        /// </summary>
        public bool SendEncryMessage(string msg, int millisecTimeOt)
        {
            Text_Des tc = new Text_Des();
            string msgEncry = tc.Encrypt(msg, true);

            try
            {

                this.nStream.WriteTimeout = millisecTimeOt;

                byte[] mess = System.Text.Encoding.ASCII.GetBytes(msgEncry);


                this.nStream.Write(mess, 0, mess.Length);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        //--------------------------


        /// <summary>
        /// Manda Un Messaggio Cryptato | Timeout Distrugge Stream
        /// Uso un Minibufferino di 28Byte prima del messaggio
        /// </summary>
        public bool SendLargeEncryMessage(string msg, int millisecTimeOt)
        {
            try
            {

                Text_Des tc = new Text_Des();
                this.nStream.WriteTimeout = millisecTimeOt;

                string msgEncry = tc.Encrypt(msg, true);  // CRYPTO IL MSG  E TROVO LA SUA GRANDEZZA

                int totalBuf = msgEncry.Length; // GRANDEZZA MESSAGGIO

                string buf28 = totalBuf.ToString() + "?";

                while (buf28.Length < 28)
                    buf28 += "0";

                byte[] buf28Arr = System.Text.Encoding.ASCII.GetBytes(buf28);

                byte[] mess = System.Text.Encoding.ASCII.GetBytes(msgEncry);

                List<byte> listaApp = new List<byte>();

                listaApp.AddRange(buf28Arr);
                listaApp.AddRange(mess);

                byte[] msgFinale = listaApp.ToArray();

                this.nStream.Write(msgFinale, 0, msgFinale.Length);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        //--------------------------             


        /// <summary>
        /// Aspetta un messaggio all'infinito
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForMessage()
        {
            // BUFFER DATA
            byte[] bytes = new byte[1024];
            string data = null;

            try
            {
                // Incoming message may be larger than the buffer size.
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = this.nStream.Read(bytes, 0, bytes.Length);

                    string incomingMsg = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;

                }
                while (this.nStream.DataAvailable);

            }
            catch
            {
                //return "TIMEOUT";
            }


            return data;
        }
        //-------------------------

        /// <summary>
        /// Aspetta un messaggio per x Millisecondi | Return "TIMEOUT" in caso di timeout | Disturugge Stream
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForMessage(int milliSecTimeOut)
        {
            // BUFFER DATA
            byte[] bytes = new byte[1024];
            string data = null;

            try
            {


                this.nStream.ReadTimeout = milliSecTimeOut;

                // Incoming message may be larger than the buffer size.
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = this.nStream.Read(bytes, 0, bytes.Length);

                    string incomingMsg = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;

                }
                while (this.nStream.DataAvailable);
            }
            catch
            {
                return "TIMEOUT";
            }


            return data;
        }
        //-------------------------

        /// <summary>
        /// Aspetta un messaggio cryptato per x Millisecondi | Return "TIMEOUT" in caso di timeout| Timeout distrugge Stream
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForEncryMessage(int milliSecTimeOut)
        {
            Text_Des tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            byte[] bytes = new byte[1024];
            string data = null;

            try
            {
                //stream = clientTcp.GetStream();

                this.nStream.ReadTimeout = milliSecTimeOut;

                // Incoming message may be larger than the buffer size.
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = this.nStream.Read(bytes, 0, bytes.Length);

                    string incomingMsg = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;

                }
                while (this.nStream.DataAvailable);

                data = tc.Decrypt(data, true);
            }
            catch
            {
                return "TIMEOUT";
            }


            return data;
        }
        //-------------------------

        /// <summary>
        /// Aspetta un messaggio cryptato all'infinito
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForEncryMessage()
        {
            Text_Des tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            byte[] bytes = new byte[1024];
            string data = null;

            try
            {

                // Incoming message may be larger than the buffer size.
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = this.nStream.Read(bytes, 0, bytes.Length);

                    string incomingMsg = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;

                }
                while (this.nStream.DataAvailable);

                data = tc.Decrypt(data, true);
            }
            catch
            {
                //return "TIMEOUT";
            }


            return data;
        }
        //-------------------------

        /// <summary>
        /// Aspetta un messaggio cryptato | TimeOut ricorsivo | Return "__ERROR__" in caso di eccezione nn gestita
        /// Unico Buffer 1024!
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForEncryMessageRicorsive(int millisecTimeOut)
        {
            Text_Des tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            byte[] bytes = new byte[1024];
            string data = null;

            try
            {

                // LEGGI UN MESSAGGIO SOLO SE ESISTE ENTRO MILLISEC
                int c = 0;
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    c += 100;

                    if (this.nStream.DataAvailable)
                        break;
                    if (c >= millisecTimeOut)
                    {
                        return "TIMEOUT";
                    }
                }

                // Incoming message may be larger than the buffer size.
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = this.nStream.Read(bytes, 0, bytes.Length);

                    string incomingMsg = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;

                }
                while (this.nStream.DataAvailable);




                data = tc.Decrypt(data, true);
            }
            catch
            {
                return "__ERROR__";
            }


            return data;
        }
        //-------------------------



        /// <summary>
        /// Aspetta un messaggio cryptato | TimeOut ricorsivo | Return "__ERROR__" in caso di eccezione nn gestita
        /// Buffer 28 info grandezza [28 buf non cryptato]
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForLargeEncryMessageRicorsive(int millisecTimeOut)
        {
            Text_Des tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            byte[] bytes = new byte[128];
            string data = null;

            try
            {

                // LEGGI UN MESSAGGIO SOLO SE ESISTE ENTRO MILLISEC
                int c = 0;
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    c += 100;

                    if (this.nStream.DataAvailable)
                        break;
                    if (c >= millisecTimeOut)
                    {
                        return "TIMEOUT";
                    }
                }

                // LEGGO BUFFERINO DI 28 [NON CRYPTATO] PER SAPERE LA GRANDEZZA DEL MSG CRYPTATO
                int readTo = 0;
                byte[] headBuf = new byte[28];

                int r = this.nStream.Read(headBuf, 0, headBuf.Length);

                string headMsg = System.Text.Encoding.ASCII.GetString(headBuf, 0, r);

                string[] headsplit = headMsg.Split('?');

                readTo = int.Parse(headsplit[0]);


                // Incoming message may be larger than the buffer size.
                int numberOfBytesRead = 0;
                int totalByteRead = 0;
                do
                {
                    numberOfBytesRead = this.nStream.Read(bytes, 0, bytes.Length);
                    totalByteRead += numberOfBytesRead;

                    string incomingMsg = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;

                }
                while (totalByteRead < readTo);

                data = tc.Decrypt(data, true);


                return data;

            }
            catch
            {
                return "__ERROR__";
            }



        }
        //-------------------------









    }
}
