using System;
using System.Threading;
using System.Windows.Forms;

namespace BlackHat_Server
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            // INIZIALIZZO LE INFO DEL SERVER
            var cc = new CreateStClient();
            cc.InitializeStClient();

            using (var mutex = new Mutex(false, ST_Client.Instance.sMutex))
            {
                if (!mutex.WaitOne(0, false))
                    return;


                Application.Run(new Host());
            }
        }

        /// <summary>
        ///     Eccezioni non gestite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Application.Restart();
        }
        //-------------------------------

        /// <summary>
        ///     Eccezioni non gestite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Application.Restart();
        }
        //-----------------------------
    }
}