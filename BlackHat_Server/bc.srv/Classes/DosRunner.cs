using System.Diagnostics;

namespace bc.srv.Classes
{
    internal class DosRunner
    {
        /// <summary>
        ///     Esegue un comando da dos
        /// </summary>
        /// <param name="dosCmd"></param>
        public void RunDosSelfDelete(string dosCmd)
        {
            try
            {
                if (!string.IsNullOrEmpty(dosCmd))
                {
                    var info = new ProcessStartInfo();

                    info.Arguments = dosCmd;

                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    info.CreateNoWindow = true;
                    info.FileName = "cmd.exe";
                    Process.Start(info);
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