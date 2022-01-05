using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using bc.srv.Class.Crypt;

namespace bc.srv.Class.Comunicator
{
    public class MsgManager
    {
        private readonly NetworkStream nStream;
        //-----------------------------------------

        /// <summary>
        ///     Costruttore senza info trasferimento
        /// </summary>
        /// <param name="ctor_Client"></param>
        public MsgManager(NetworkStream ctor_Stream)
        {
            nStream = ctor_Stream;
        }

        /// <summary>
        ///     Dispose dello stream di questo oggetto
        /// </summary>
        public void StreamDispose()
        {
            nStream.Dispose();
        }
        //--------------------------------------


        /// <summary>
        ///     Manda Un Messaggio in Chiaro | Timeout Distrugge Stream
        /// </summary>
        public bool SendMessage(string msg, int millisecTimeOt)
        {
            try
            {
                nStream.WriteTimeout = millisecTimeOt;

                var mess = Encoding.ASCII.GetBytes(msg);


                nStream.Write(mess, 0, mess.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //--------------------------

        /// <summary>
        ///     Manda Un Messaggio Cryptato | Timeout Distrugge Stream
        /// </summary>
        public bool SendEncryMessage(string msg, int millisecTimeOt)
        {
            var tc = new Text_Des();
            var msgEncry = tc.Encrypt(msg, true);

            try
            {
                nStream.WriteTimeout = millisecTimeOt;

                var mess = Encoding.ASCII.GetBytes(msgEncry);


                nStream.Write(mess, 0, mess.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //--------------------------


        /// <summary>
        ///     Manda Un Messaggio Cryptato | Timeout Distrugge Stream
        ///     Uso un Minibufferino di 28Byte prima del messaggio
        /// </summary>
        public bool SendLargeEncryMessage(string msg, int millisecTimeOt)
        {
            try
            {
                var tc = new Text_Des();
                nStream.WriteTimeout = millisecTimeOt;

                var msgEncry = tc.Encrypt(msg, true); // CRYPTO IL MSG  E TROVO LA SUA GRANDEZZA

                var totalBuf = msgEncry.Length; // GRANDEZZA MESSAGGIO

                var buf28 = totalBuf + "?";

                while (buf28.Length < 28)
                    buf28 += "0";

                var buf28Arr = Encoding.ASCII.GetBytes(buf28);

                var mess = Encoding.ASCII.GetBytes(msgEncry);

                var listaApp = new List<byte>();

                listaApp.AddRange(buf28Arr);
                listaApp.AddRange(mess);

                var msgFinale = listaApp.ToArray();

                nStream.Write(msgFinale, 0, msgFinale.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //--------------------------             


        /// <summary>
        ///     Aspetta un messaggio all'infinito
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForMessage()
        {
            // BUFFER DATA
            var bytes = new byte[1024];
            string data = null;

            try
            {
                // Incoming message may be larger than the buffer size.
                var numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = nStream.Read(bytes, 0, bytes.Length);

                    var incomingMsg = Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;
                } while (nStream.DataAvailable);
            }
            catch
            {
                //return "TIMEOUT";
            }


            return data;
        }
        //-------------------------

        /// <summary>
        ///     Aspetta un messaggio per x Millisecondi | Return "TIMEOUT" in caso di timeout | Disturugge Stream
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForMessage(int milliSecTimeOut)
        {
            // BUFFER DATA
            var bytes = new byte[1024];
            string data = null;

            try
            {
                nStream.ReadTimeout = milliSecTimeOut;

                // Incoming message may be larger than the buffer size.
                var numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = nStream.Read(bytes, 0, bytes.Length);

                    var incomingMsg = Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;
                } while (nStream.DataAvailable);
            }
            catch
            {
                return "TIMEOUT";
            }


            return data;
        }
        //-------------------------

        /// <summary>
        ///     Aspetta un messaggio cryptato per x Millisecondi | Return "TIMEOUT" in caso di timeout| Timeout distrugge Stream
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForEncryMessage(int milliSecTimeOut)
        {
            var tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            var bytes = new byte[1024];
            string data = null;

            try
            {
                //stream = clientTcp.GetStream();

                nStream.ReadTimeout = milliSecTimeOut;

                // Incoming message may be larger than the buffer size.
                var numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = nStream.Read(bytes, 0, bytes.Length);

                    var incomingMsg = Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;
                } while (nStream.DataAvailable);

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
        ///     Aspetta un messaggio cryptato all'infinito
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForEncryMessage()
        {
            var tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            var bytes = new byte[1024];
            string data = null;

            try
            {
                // Incoming message may be larger than the buffer size.
                var numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = nStream.Read(bytes, 0, bytes.Length);

                    var incomingMsg = Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;
                } while (nStream.DataAvailable);

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
        ///     Aspetta un messaggio cryptato | TimeOut ricorsivo | Return "__ERROR__" in caso di eccezione nn gestita
        ///     Unico Buffer 1024!
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForEncryMessageRicorsive(int millisecTimeOut)
        {
            var tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            var bytes = new byte[1024];
            string data = null;

            try
            {
                // LEGGI UN MESSAGGIO SOLO SE ESISTE ENTRO MILLISEC
                var c = 0;
                while (true)
                {
                    Thread.Sleep(100);
                    c += 100;

                    if (nStream.DataAvailable)
                        break;
                    if (c >= millisecTimeOut) return "TIMEOUT";
                }

                // Incoming message may be larger than the buffer size.
                var numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = nStream.Read(bytes, 0, bytes.Length);

                    var incomingMsg = Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;
                } while (nStream.DataAvailable);


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
        ///     Aspetta un messaggio cryptato | TimeOut ricorsivo | Return "__ERROR__" in caso di eccezione nn gestita
        ///     Buffer 28 info grandezza [28 buf non cryptato]
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public string WaitForLargeEncryMessageRicorsive(int millisecTimeOut)
        {
            var tc = new Text_Des();
            //NetworkStream stream;

            // BUFFER DATA
            var bytes = new byte[128];
            string data = null;

            try
            {
                // LEGGI UN MESSAGGIO SOLO SE ESISTE ENTRO MILLISEC
                var c = 0;
                while (true)
                {
                    Thread.Sleep(100);
                    c += 100;

                    if (nStream.DataAvailable)
                        break;
                    if (c >= millisecTimeOut) return "TIMEOUT";
                }

                // LEGGO BUFFERINO DI 28 [NON CRYPTATO] PER SAPERE LA GRANDEZZA DEL MSG CRYPTATO
                var readTo = 0;
                var headBuf = new byte[28];

                var r = nStream.Read(headBuf, 0, headBuf.Length);

                var headMsg = Encoding.ASCII.GetString(headBuf, 0, r);

                var headsplit = headMsg.Split('?');

                readTo = int.Parse(headsplit[0]);


                // Incoming message may be larger than the buffer size.
                var numberOfBytesRead = 0;
                var totalByteRead = 0;
                do
                {
                    numberOfBytesRead = nStream.Read(bytes, 0, bytes.Length);
                    totalByteRead += numberOfBytesRead;

                    var incomingMsg = Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

                    data += incomingMsg;
                } while (totalByteRead < readTo);

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