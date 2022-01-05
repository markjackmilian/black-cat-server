using System.Net;
using System.Net.Sockets;
using System.Threading;
using bc.srv.Classes.Comunicator;

namespace bc.srv.Classes
{
    internal class Connection
    {
        public void StartServer()
        {
            this.TryConnect();
            var guard = new Guardian();
            guard.StartGuardian();
        }


        /// <summary>
        ///     Prima connessione Con invio di Info di avvio!
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private void TryConnect()
        {
            var host = SrvData.Instance.Host;
            var port = SrvData.Instance.Port;

            IPAddress testIp;
            var isIp = IPAddress.TryParse(host, out testIp);

            while (!SrvData.Instance.isConnected)
                try
                {
                    if (!isIp)
                        SrvData.Instance.Connessione.Connect(host,
                            port); // provo a connettermi altrimenti entro nel catch e aspetto X sec
                    else
                        SrvData.Instance.Connessione.Connect(testIp,
                            port); // provo a connettermi altrimenti entro nel catch e aspetto X sec

                    SrvData.Instance.isConnected = this.FirstConnection();

                    if (!SrvData.Instance.isConnected)
                        Thread.Sleep(Program.Random.Next(25000,60000));
                }
                catch
                {
                    Thread.Sleep(Program.Random.Next(5000,10000));
                }
        }
        //***********************************************************


        /// <summary>
        ///     Trovo una connessione controllo diritti e invio info di avvio
        /// </summary>
        private bool FirstConnection()
        {
            var ns = SrvData.Instance.Connessione.GetStream();
            var psw = SrvData.Instance.Password;

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


                var ms = string.Format("{0}|{1}|{2}|{3}|{4}", SrvData.Instance.UnivoqueId, un, mn,
                    SrvData.Instance.ServerName, SrvData.Instance.ServerVersion);

                var sent = mm.SendEncryMessage(ms, 20000);
                // SE INVIO CORRETTAMENTE QUESTO MESSAGGIO SONO CONNESSO 
                if (sent) return true;

                SrvData.Instance.Connessione.Close();
                SrvData.Instance.Connessione = new TcpClient();
                return false;
            }

            SrvData.Instance.Connessione.Close();
            SrvData.Instance.Connessione = new TcpClient();
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
                    newSlot.Connect(SrvData.Instance.Host, SrvData.Instance.Port);
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