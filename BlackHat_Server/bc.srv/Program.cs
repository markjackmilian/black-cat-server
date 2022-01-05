using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace bc.srv
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // todo init st_client

            using (var mutex = new Mutex(false, "todo"))
            {
                if (!mutex.WaitOne(0, false))
                    return;


                // start all
            }

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Restart();
        }

        public static void Restart()
        {
            // todo add delay to process start using extternal process cmd
            Process.Start(Assembly.GetExecutingAssembly().Location);
            Exit(1);
        }

        public static void Exit(int code)
        {
            Environment.Exit(code);
        }
    }
}