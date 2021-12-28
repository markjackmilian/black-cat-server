using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackHat_Server
{
    public class MsgFileManager
    {
        private readonly NetworkStream nStream;

        /// <summary>
        ///     Costruttore senza info trasferimento
        /// </summary>
        /// <param name="ctor_Client"></param>
        public MsgFileManager(NetworkStream ctor_Stream)
        {
            nStream = ctor_Stream;
        }
        //--------------------------------------


        /// <summary>
        ///     Manda Un Byte[] Cryptato | Timeout Distrugge Stream | invia lunghezza file e poi il file
        ///     HeaderBuffer 100
        /// </summary>
        public bool SendEncryFileByte(byte[] fileByte, int millisecTimeOt)
        {
            try
            {
                var fd = new File_Des();
                //Text_Des td = new Text_Des();

                var encryByte = fd.EncryptFile(fileByte, true);

                var storeLenght = encryByte.Length + "|"; //  LUNGHEZZA DEL FILE PIù SEPARATORE


                // RIEMPIO IL BUFFER DELL'HEADER
                var charHeader = new List<char>(storeLenght.ToCharArray());
                while (charHeader.Count < 100)
                    charHeader.Add('0');

                // RICOSTRUISCO HEADER IN STRINGA
                var header = "";
                foreach (var ch in charHeader)
                    header += ch;


                var head = Encoding.ASCII.GetBytes(header);


                // BYTE ARRAY FINALE
                var fileByteList = new List<byte>();
                fileByteList.AddRange(head);
                fileByteList.AddRange(encryByte);
                var finale = fileByteList.ToArray();
                //----------------------------------------


                nStream.WriteTimeout = millisecTimeOt;
                nStream.Write(finale, 0, finale.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //--------------------------


        /// <summary>
        ///     Aspetta un File cryptato Return ByteDecryptato | ERRORE O TIMEOUT  = NULL
        ///     Buffer 100 Info Grandezza File (BUFFERS successivi file cryptato)
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public byte[] WaitEncryFileByte(int millisecTimeOut)
        {
            var fd = new File_Des();


            // BUFFER DATA
            var headerBytes = new byte[100];
            var bytes = new byte[256];

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
                    if (c >= millisecTimeOut) return null;
                }


                // LEGGO GRANDEZZA DEL FILE PER FARE IL LOOP SOTTOSTANTE
                var BytesRead = nStream.Read(headerBytes, 0, headerBytes.Length);

                var headerInfo = Encoding.ASCII.GetString(headerBytes, 0, BytesRead);

                var info = headerInfo.Split('|');

                var fileLenght = int.Parse(info[0]);

                //------------------------------------------------------

                if (fileLenght > 0)
                    // LEGGO IL FILE FINO ALLA FINE DELLA SUA GRANDEZZA
                    using (var memoryStream = new MemoryStream())
                    {
                        ////Incoming message may be larger than the buffer size.
                        var numberOfBytesRead = 0;
                        var nowRead = 0;

                        do
                        {
                            nowRead = nStream.Read(bytes, 0, bytes.Length);
                            numberOfBytesRead += nowRead;
                            memoryStream.Write(bytes, 0, nowRead);
                        } while (numberOfBytesRead < fileLenght);

                        var encryByte = memoryStream.ToArray();
                        //memoryStream.Close();

                        var decryByte = fd.DecryptFile(encryByte, true);
                        return decryByte;
                    }
                //---------------------------------------------------------

                return null;
            }
            catch
            {
                return null;
            }
        }
        //-------------------------      

        /// <summary>
        ///     Aspetta un File cryptato Return ByteDecryptato | ERRORE O TIMEOUT  = NULL
        ///     Buffer 100 Info Grandezza File (BUFFERS successivi file cryptato)
        ///     bool result Scrive file non cryptato
        /// </summary>
        /// <param name="milliSecTimeOut"></param>
        /// <returns></returns>
        public bool WaitDiskEncryFileByte(int millisecTimeOut, string filePath)
        {
            var fd = new File_Des();

            // BUFFER DATA
            var headerBytes = new byte[100];
            var bytes = new byte[512];

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
                    if (c >= millisecTimeOut) return false;
                }


                // LEGGO GRANDEZZA DEL FILE PER FARE IL LOOP SOTTOSTANTE
                var BytesRead = nStream.Read(headerBytes, 0, headerBytes.Length);

                var headerInfo = Encoding.ASCII.GetString(headerBytes, 0, BytesRead);

                var info = headerInfo.Split('|');

                var fileLenght = int.Parse(info[0]);

                //------------------------------------------------------

                if (fileLenght > 0)
                {
                    // LEGGO IL FILE FINO ALLA FINE DELLA SUA GRANDEZZA
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ////Incoming message may be larger than the buffer size.
                        //int numberOfBytesRead = 0;
                        var nowRead = 0;

                        var actualByteReaded = 0;

                        nStream.ReadTimeout = -1;
                        do
                        {
                            nowRead = nStream.Read(bytes, 0, bytes.Length);
                            actualByteReaded += nowRead;


                            fileStream.Write(bytes, 0, nowRead);
                        } while (actualByteReaded < fileLenght);
                    } // end using

                    var readEncryByte = File.ReadAllBytes(filePath);

                    var decryByte = fd.DecryptFile(readEncryByte, true);

                    File.WriteAllBytes(filePath, decryByte);
                    return true;

                    //---------------------------------------------------------
                }

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