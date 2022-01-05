using System;
using System.Net.Sockets;
using System.Threading;
using bc.srv.Classes.Comunicator;
using bc.srv.Muduli;

namespace bc.srv.Classes
{
    // OGGETTO GUARDIAN ASPETTA UN MESSAGGIO E PASSA IL MESSAGGIO A CMDINTERPRETER
    internal class Guardian
    {
        /// <summary>
        ///     Avvia Guardiano su nuovo thread
        /// </summary>
        public void StartGuardian()
        {
            var t = new Thread(this.CmdGuardian);
            t.IsBackground = true;
            t.Start();
        }
        //---------------------------------------


        /// <summary>
        ///     Worker thread Guardian
        /// </summary>
        private void CmdGuardian()
        {
            var ka = new KeepAlive();
            var com = new MsgManager(SrvData.Instance.Connessione.GetStream());

            while (SrvData.Instance.isConnected)
                try
                {
                    //string t = ST_Client.Instance.Connessione.Client.RemoteEndPoint.ToString();

                    // ASPETTO PER 60 SECONDI UN MESSAGGIO 
                    // OGNI 90 SU CLIENT!
                    var cmd = com.WaitForEncryMessageRicorsive(60000);

                    if (cmd != "TIMEOUT")
                    {
                        var cmdit = new CmdInterpreter();
                        cmdit.Interpreter(cmd);
                    }

                    // SE LA CONNESSIONE DEL SERVER NON P STATA CHIUSA MANUALMENTE CONTROLLO IS ALIVE
                    if (SrvData.Instance.isConnected)
                        SrvData.Instance.isConnected = ka.IsAlivemMessageMode();
                }
                catch (InvalidOperationException)
                {
                    break;
                }

            this.CloseAndReconnect();
        }
        //---------------------------------------

        /// <summary>
        ///     Chiude la connessione e ne crea una nuova
        /// </summary>
        public void CloseAndReconnect()
        {
            // NON SONO PIù CONNESSO RILANCIO STARSERVER            
            SrvData.Instance.Connessione.Close();

            SrvData.Instance.CloseAllChannels();

            SrvData.Instance.Connessione = new TcpClient();
            var con = new Connection();
            con.StartServer();
        }
    }
}