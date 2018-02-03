using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileShare {

    class ImageAdapter {

        /// <summary>
        /// Given a byte array, returns an ImageSource coresponding to a BitmapImage 
        /// obtained by this byte array.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ImageSource IconFromByteArray(byte[] value) {
            // if no byte: no icon set
            if (value == null || value.Length == 0)
                return null;

            // Sets byte icon
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(value);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            return biImg as ImageSource;
        }

        /// <summary>
        /// Returns a byte array coresponding to the image passed as first parameter.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static byte[] ByteArrayFromImage(Bitmap icon, int maxSize) {
            byte[] bytes = null;
            using (var stream = new MemoryStream()) {
                Image b = GetThumbnailImage(icon, new Size(128, 128));
                b.Save(stream, ImageFormat.Jpeg);
                bytes = stream.ToArray();
                while (bytes.Length > maxSize)
                    bytes = Reduce(b, 80);
                stream.Close();
                b.Dispose();
            }
            return bytes;
        }

        /// <summary>
        /// Returns a byte array coresponding to the image passed as first parameter
        /// and adapted to the size specified as secon parameter.
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] ByteArrayFromImage(BitmapSource icon, int maxSize) {
            byte[] bytes = null;
            if (icon != null) {
                //var stream = GetMemoryStream(icon);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(icon));
                using (var stream = new MemoryStream()) {
                    encoder.Save(stream);
                    Image tmpBitmap = new System.Drawing.Bitmap(stream);
                    Image b = GetThumbnailImage(tmpBitmap, new Size(128, 128));
                    tmpBitmap.Dispose();
                    b.Save(stream, ImageFormat.Jpeg);
                    bytes = stream.ToArray();
                    while (bytes.Length > maxSize)
                        bytes = Reduce(b, 80);
                    stream.Close();
                    b.Dispose();
                }
            }
            return bytes;
        }

        private static MemoryStream GetMemoryStream(BitmapSource bitmap) {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = new MemoryStream()) {
                encoder.Save(stream);
                return stream;
            }
        }

        /// <summary>
        /// Given an Image and a Size, returns the same image adapted to the specified size
        /// mantaining the best possible quality.
        /// If the has not a ration equals to the ration of the given size, the method returns
        /// an image which has the smaller edge of the grather of the dimensions of the given size.
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <param name="ThumbSize"></param>
        /// <returns></returns>
        public static Image GetThumbnailImage(Image OriginalImage, Size ThumbSize) {
            Int32 thWidth = ThumbSize.Width;
            Int32 thHeight = ThumbSize.Height;
            Image i = OriginalImage;
            Int32 w = i.Width;
            Int32 h = i.Height;
            Int32 th = thHeight;
            Int32 tw = thWidth;
            if (h < w) {
                Double ratio = (Double)w / (Double)h;
                th = thHeight < h ? thHeight : h;
                tw = thWidth < w ? (Int32)(ratio * thWidth) : w;
            } else {
                Double ratio = (Double)h / (Double)w;
                th = thHeight < h ? (Int32)(ratio * thHeight) : h;
                tw = thWidth < w ? thWidth : w;
            }
            
            Bitmap target = new Bitmap(tw, th);
            Graphics g = Graphics.FromImage(target);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;

            int t = 0, l = 0;
            if (i.Height > i.Width)
                t = (i.Height - i.Width) / 2;
            else
                l = (i.Width - i.Height) / 2;

            Rectangle rect = new Rectangle(0, 0, tw, th);
            g.DrawImage(i, rect, l, t, (w - l * 2), (h - t * 2), GraphicsUnit.Pixel);
            //g.DrawImage(i, rect, 0, 0, w, h, GraphicsUnit.Pixel);
            g.Dispose();

            return (Image)target;
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType) {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }


        internal static byte[] Reduce(Image img, int quality) {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            // JPEG image codec 
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            using (var stream = new MemoryStream()) {
                img.Save(stream, jpegCodec, encoderParams);
                return stream.ToArray();
            }
        }
    }
}
