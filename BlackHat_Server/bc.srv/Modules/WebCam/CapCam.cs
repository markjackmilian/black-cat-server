﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using bc.srv.Classes.Image_Classes;

namespace bc.srv.Modules.WebCam
{
    public class CapCam
    {
        /// <summary>
        ///     Trovo i drivers presenti per la webcam
        /// </summary>
        /// <returns></returns>
        public List<string> GetWcDrivers()
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
            SendMessage(this.hCaptureWnd, WmCapGetFrame, 0, 0);
            SendMessage(this.hCaptureWnd, WmCapCopy, 0, 0);


            var bitmap = (Bitmap) Clipboard.GetDataObject()?.GetData(DataFormats.Bitmap);
            if (bitmap == null)
                return null;

            Clipboard.Clear();

            return this.iw.ImageResizeToJpg(bitmap, xResize, yResize, quality);
        }
        //-----------------------------------------------------


        /// <summary>
        ///     Avvio Connessione con Device
        /// </summary>
        public bool TryConnect(int iDevice)
        {
            try
            {
                this.hCaptureWnd = capCreateCaptureWindowA("", 0, 0, 0, 350, 350, 0, 0);
                //int res = SendMessage(hCaptureWnd, WM_CAP_CONNECT, 0, 0);
                var res = SendMessage(this.hCaptureWnd, WmCapConnect, iDevice, 0);

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
                var res = SendMessage(this.hCaptureWnd, WmCapDisconnect, 0, 0);
                DestroyWindow(this.hCaptureWnd);
            }
            catch
            {
            }
        }

        #region PINVOKE

        [DllImport("user32", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, uint msg, int wParam, int lParam);

        [DllImport("avicap32.dll", EntryPoint = "capCreateCaptureWindowA")]
        private static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle,
            int x, int y, int nWidth, int nHeight, int hwndParent, int nId);

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

        private const int WmCapConnect = 1034;
        private const int WmCapDisconnect = 1035;
        private const int WmCapCopy = 1054;
        private const int WmCapGetFrame = 1084;

        #endregion


        #region MEMBRERS

        private int hCaptureWnd = -1; // HANDLE
        private readonly ImageWorker iw = new ImageWorker();

        #endregion

        //-----------------------------
    }
}