using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace bc.srv.Muduli.Desktop
{
    public class VirtualMouse
    {
        private const int MouseeventfLeftdown = 0x02;
        private const int MouseeventfLeftup = 0x04;
        private const int MouseeventfRightdown = 0x08;
        private const int MouseeventfRightup = 0x10;

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);


        public void MouseClick(int x, int y)
        {
            SetCursorPos(x, y);

            var mouseX = Control.MousePosition.X;
            var mouseY = Control.MousePosition.Y;

            mouse_event(MouseeventfLeftdown, mouseX, mouseY, 0, 0);
            mouse_event(MouseeventfLeftup, mouseX, mouseY, 0, 0);
        }


        public void MouseRightClick(int x, int y)
        {
            SetCursorPos(x, y);

            var X = Control.MousePosition.X;
            var Y = Control.MousePosition.Y;

            mouse_event(MouseeventfRightdown, X, Y, 0, 0);
            mouse_event(MouseeventfRightup, X, Y, 0, 0);
        }


        public void MouseDoubleCkick(int x, int y)
        {
            SetCursorPos(x, y);

            var X = Control.MousePosition.X;
            var Y = Control.MousePosition.Y;

            mouse_event(MouseeventfLeftdown, X, Y, 0, 0);
            mouse_event(MouseeventfLeftup, X, Y, 0, 0);
            mouse_event(MouseeventfLeftdown, X, Y, 0, 0);
            mouse_event(MouseeventfLeftup, X, Y, 0, 0);
        }

        public void MoveMouse(int x, int y)
        {
            SetCursorPos(x, y);
        }
    }
}