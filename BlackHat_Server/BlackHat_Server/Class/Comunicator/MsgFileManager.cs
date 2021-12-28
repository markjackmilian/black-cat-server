using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace BlackHat_Server
{
    public class MsgFileManager
    {
        
        NetworkStream nStream;

        /// <summary>
        /// Costruttore senza info trasferimento
        /// </summary>
        /// <param name="ctor_Client"></param>
        public MsgFileManager(NetworkStream ctor_Stream)
        {
            nStream = ctor_Stream;
        }
        //--------------------------------------



        /// <summary>
        /// Manda Un Byte[] Cryptato | Timeout Distrugge Stream | invia lunghezza file e poi il file
        /// HeaderBuffer 100
        /// </summary>
        public bool SendEncryFileByte(byte[] fileByte, int millisecTimeOt)
        {
            try
            {
                File_Des fd = new File_Des();
                //Text_Des td = new Text_Des();

                byte[] encryByte = fd.EncryptFile(fileByte, true);

                string storeLenght = encryByte.Length.ToString() + "|"; //  LUNGHEZZA DEL FILE PIù SEPARATORE

                
                // RIEMPIO IL BUFFER DELL'HEADER
                List<char> charHeader = new List<char>(storeLenght.ToCharArray());
                while (charHeader.Count < 100)
                    charHeader.Add('0');

                // RICOSTRUISCO HEADER IN STRINGA
                string header = "";
                foreach (char ch in charHeader)
                    header += ch;


                byte[] head = System.Text.Encoding.ASCII.GetBytes(header);


                // BYTE ARRAY FINALE
                List<byte> fileByteList = new List<byte>();
                fileByteList.AddRange(head);
                fileByteList.AddRange(encryByte);
                byte[] finale = fileByteList.ToArray();
                //----------------------------------------

               

                this.nStream.WriteTimeout = millisecTimeOt;
                this.nStream.Write(finale, 0, finale.Length);
                return true;

            }
            catch
            {

                return false;
            }


        }
        //--------------------------


        /// <summary>
        /// Aspetta un File cryptato Return ByteDecryptato | ERRORE O TIMEOUT  = NULL
        /// Buffer 100 Info Grandezza File (BUFFERS successivi file cryptato)
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public byte[] WaitEncryFileByte(int millisecTimeOut)
        {

            File_Des fd = new File_Des();


            // BUFFER DATA
            byte[] headerBytes = new byte[100];
            byte[] bytes = new byte[256];

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
                        return null;
                    }
                }


                // LEGGO GRANDEZZA DEL FILE PER FARE IL LOOP SOTTOSTANTE
                int BytesRead = this.nStream.Read(headerBytes, 0, headerBytes.Length);

                string headerInfo = System.Text.Encoding.ASCII.GetString(headerBytes, 0, BytesRead);

                string[] info = headerInfo.Split('|');

                int fileLenght = int.Parse(info[0]);

                //------------------------------------------------------

                if (fileLenght > 0)
                {
                    // LEGGO IL FILE FINO ALLA FINE DELLA SUA GRANDEZZA
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        ////Incoming message may be larger than the buffer size.
                        int numberOfBytesRead = 0;
                        int nowRead = 0;

                        do
                        {
                            nowRead = this.nStream.Read(bytes, 0, bytes.Length);
                            numberOfBytesRead += nowRead;
                            memoryStream.Write(bytes, 0, nowRead);

                        }
                        while (numberOfBytesRead < fileLenght);

                        byte[] encryByte = memoryStream.ToArray();
                        //memoryStream.Close();

                        byte[] decryByte = fd.DecryptFile(encryByte, true);
                        return decryByte;
                    }
                    //---------------------------------------------------------
                }
                else
                    return null;

            }
            catch
            {
                return null;
            }



        }
        //-------------------------      

        /// <summary>
        /// Aspetta un File cryptato Return ByteDecryptato | ERRORE O TIMEOUT  = NULL
        /// Buffer 100 Info Grandezza File (BUFFERS successivi file cryptato)
        /// bool result Scrive file non cryptato
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public bool WaitDiskEncryFileByte(int millisecTimeOut, string filePath)
        {

            File_Des fd = new File_Des();
            
            // BUFFER DATA
            byte[] headerBytes = new byte[100];
            byte[] bytes = new byte[512];

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
                        return false;
                    }
                }


                // LEGGO GRANDEZZA DEL FILE PER FARE IL LOOP SOTTOSTANTE
                int BytesRead = this.nStream.Read(headerBytes, 0, headerBytes.Length);

                string headerInfo = System.Text.Encoding.ASCII.GetString(headerBytes, 0, BytesRead);

                string[] info = headerInfo.Split('|');

                int fileLenght = int.Parse(info[0]);

                //------------------------------------------------------

                if (fileLenght > 0)
                {
                    

                    // LEGGO IL FILE FINO ALLA FINE DELLA SUA GRANDEZZA
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ////Incoming message may be larger than the buffer size.
                        //int numberOfBytesRead = 0;
                        int nowRead = 0;

                        int actualByteReaded = 0;
                        
                        this.nStream.ReadTimeout = -1;
                        do
                        {

                            nowRead = this.nStream.Read(bytes, 0, bytes.Length);
                            actualByteReaded += nowRead;


                            fileStream.Write(bytes, 0, nowRead);
                           

                           
                        }
                        while (actualByteReaded < fileLenght);

                        

                    } // end using

                    byte[] readEncryByte = File.ReadAllBytes(filePath);

                    byte[] decryByte = fd.DecryptFile(readEncryByte, true);

                    File.WriteAllBytes(filePath,decryByte);
                    return true;

                    //---------------------------------------------------------
                }
                else
                    return false;

            }
            catch
            {
                return false;
            }



        }
        //-------------------------




    }
}
