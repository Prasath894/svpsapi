using ImageMagick;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.CrossCutting.Helper
{

    public class ImageProcessingHelper
    {
        /// <summary>
        /// Resize and Convert the image as Jpeg format
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="imageSize"></param>
        /// <returns>Returns Jpeg Image Byte Array</returns>
        public static byte[] ConvertToJpegImage(byte[] imageArray, int imageSize)
        {
            return imageArray != null ? ResizeImage(imageArray, imageSize, MagickFormat.Jpeg) : null;
        }

        /// <summary>
        /// Resize and Convert the image as Png format
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="imageSize"></param>
        /// <returns>Returns Jpeg Image Byte Array</returns>
        public static byte[] ConvertToPngImage(byte[] imageArray, int imageSize)
        {
            return imageArray != null ? ResizeImage(imageArray, imageSize, MagickFormat.Png) : null;
        }

        /// <summary>
        /// Resize the Image using provided format
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="imageSize"></param>
        /// <param name="fileFormat"></param>
        /// <returns>Resized Image Byte Array</returns>
        private static byte[] ResizeImage(byte[] imageArray, int imageSize, MagickFormat fileFormat)
        {
            using (MagickImage image = new MagickImage(imageArray))
            {
                MagickGeometry size = new MagickGeometry(imageSize);
                image.Format = fileFormat;
                image.Resize(size);
                return image.ToByteArray();
            }
        }

        /// <summary>
        /// Resize the Image
        /// </summary>
        /// <param name="imageArray"></param>
        /// <param name="imageSize"></param>
        /// <returns>Resized Image Byte Array</returns>
        public static byte[] ResizeImage(byte[] imageArray, int imageSize)
        {
            if (imageArray != null)
            {
                using (MagickImage image = new MagickImage(imageArray))
                {
                    MagickGeometry size = new MagickGeometry(imageSize);
                    image.Resize(size);
                    return image.ToByteArray();
                }
            }
            return null;
        }
    }
}