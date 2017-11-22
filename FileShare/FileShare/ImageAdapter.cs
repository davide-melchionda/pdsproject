using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            if (value.Length == 0)
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
        public static byte[] ByteArrayFromImage(ImageSource icon) {
            // Then sets byteIcon
            byte[] bytes = null;
            if (icon != null) {
                BitmapSource bitmapSource = icon as BitmapSource;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using (var stream = new MemoryStream()) {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
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
        public static byte[] ByteArrayFromImage(ImageSource icon, Size size) {
            // Then sets byteIcon
            byte[] bytes = null;
            if (icon != null) {
                BitmapSource bitmapSource = icon as BitmapSource;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using (var stream = new MemoryStream()) {
                    encoder.Save(stream);
                    Image b = GetThumbnailImage(new System.Drawing.Bitmap(stream), size);
                    b.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bytes = stream.ToArray();
                }
            }
            return bytes;
        }

        /// <summary>
        /// Given an Image and a Size, returns the same image adapted to the specified size
        /// mantaining the best possible quality.
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
            Int32 th = thWidth;
            Int32 tw = thWidth;
            if (h > w) {
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
            Rectangle rect = new Rectangle(0, 0, tw, th);
            g.DrawImage(i, rect, 0, 0, w, h, GraphicsUnit.Pixel);
            return (Image)target;
        }
        
    }
}
