using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlackHat_Server
{
    class VirtualKeyboard
    {
        ////TROVA INTPTR DELLA FINESTRA ATTIVA
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetForegroundWindow();

        public void WriteKey(string keyboard)
        {
            SendKeys.Send(keyboard);
        }
    }
}
