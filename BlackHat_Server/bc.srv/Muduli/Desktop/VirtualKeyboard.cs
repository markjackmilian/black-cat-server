using System.Windows.Forms;

namespace bc.srv.Muduli.Desktop
{
    internal class VirtualKeyboard
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