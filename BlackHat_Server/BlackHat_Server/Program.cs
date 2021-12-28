using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace BlackHat_Server
{
    static class Program
    {
       
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


            // INIZIALIZZO LE INFO DEL SERVER
            CreateStClient cc = new CreateStClient();
            cc.InitializeStClient();

            using (Mutex mutex = new Mutex(false, ST_Client.Instance.sMutex))
            {
                if (!mutex.WaitOne(0,false))                    
                    return;
                

                Application.Run(new Host());              
            }        
        }

        /// <summary>
        /// Eccezioni non gestite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Application.Restart();
        }
        //-------------------------------

        /// <summary>
        /// Eccezioni non gestite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Application.Restart();
        }
        //-----------------------------
     


    }
}
