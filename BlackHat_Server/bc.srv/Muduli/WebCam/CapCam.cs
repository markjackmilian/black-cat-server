using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using bc.srv.Class.Image_Classes;

namespace bc.srv.Muduli.WebCam
{
    public class CapCam
    {
        /// <summary>
        ///     Trovo i drivers presenti per la webcam
        /// </summary>
        /// <returns></returns>
        public List<string> GetWCDrivers()
        {
            var listaDrivers = new List<string>();

            for (ushort i = 0; i < 10; ++i)
            {
                var capacity = 200;
                var name = new StringBuilder(capacity);
                var description = new StringBuilder(capacity);

                if (capGetDriverDescription(i, name, capacity, description, capacity).ToInt32() > 0)
                    listaDrivers.Add(name.ToString());
            }


            return listaDrivers;
        }
        //---------------------------------


        /// <summary>
        ///     Cattura un'immagine dal device a cui sono collegato.
        ///     NB PRIMA DEVO ESSERE COLEGATO AL DEVICE
        ///     NULL IN CASO DI ERRORE
        /// </summary>
        /// <returns></returns>
        public byte[] Capture(int quality, int xResize, int yResize)
        {
            //Clipboard.Clear();            
            SendMessage(hCaptureWnd, WM_CAP_GET_FRAME, 0, 0);
            SendMessage(hCaptureWnd, WM_CAP_COPY, 0, 0);


            var bitmap = (Bitmap) Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
            if (bitmap == null)
                return null;

            Clipboard.Clear();

            return iw.ImageResizeToJpg(bitmap, xResize, yResize, quality);


            //using (MemoryStream stream = new MemoryStream())
            //{
            //    bitmap.Save(stream, ImageFormat.Jpeg);
            //    return stream.ToArray();
            //}
        }
        //-----------------------------------------------------


        /// <summary>
        ///     Avvio Connessione con Device
        /// </summary>
        public bool TryConnect(int iDevice)
        {
            try
            {
                hCaptureWnd = capCreateCaptureWindowA("", 0, 0, 0, 350, 350, 0, 0);
                //int res = SendMessage(hCaptureWnd, WM_CAP_CONNECT, 0, 0);
                var res = SendMessage(hCaptureWnd, WM_CAP_CONNECT, iDevice, 0);

                Thread.Sleep(500); // warm up device

                if (res > 0)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        //-----------------------------

        /// <summary>
        ///     Disconnetto da Device
        ///     NB PRIMA DEVI ESSERE CONNESSO!
        /// </summary>
        public void Disconnect()
        {
            try
            {
                var res = SendMessage(hCaptureWnd, WM_CAP_DISCONNECT, 0, 0);
                DestroyWindow(hCaptureWnd);
            }
            catch
            {
            }
        }

        #region PINVOKE

        [DllImport("user32", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        private static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle,
            int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("avicap32.dll")]
        private static extern IntPtr capGetDriverDescription(
            ushort index,
            StringBuilder name,
            int nameCapacity,
            StringBuilder description,
            int descriptionCapacity
        );


        [DllImport("USER32.DLL")]
        private static extern bool DestroyWindow(int hwnd);

        #endregion


        #region COSTANTI

        private const int WM_CAP_CONNECT = 1034;
        private const int WM_CAP_DISCONNECT = 1035;
        private const int WM_CAP_COPY = 1054;
        private const int WM_CAP_GET_FRAME = 1084;

        #endregion


        #region MEMBRERS

        private int hCaptureWnd = -1; // HANDLE
        private readonly ImageWorker iw = new ImageWorker();

        #endregion

        //-----------------------------
    }
}