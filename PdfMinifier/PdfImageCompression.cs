using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using JpegEncoder;
using SimplePaletteQuantizer.Helpers;
using SimplePaletteQuantizer.Quantizers.Uniform;
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Ditherers;

namespace PdfMinifier
{

    class PdfImageCompression
    {

        public delegate void ProgressUpdate(int value, int max);
        public event ProgressUpdate OnProgressUpdate;

        public int TargetDpi { get; set; }
        public int JpegCompression { get; set; }
        public bool DecreaseColors { get; set; }

        int ProgressMax = 0;
        int Progress = 0;

        public List<SPdfImage> ProcessImages(Dictionary<int,SPage> pages)
        {
            List<SPdfImage> outImages = new List<SPdfImage>();

            int imageCount = pages.Sum(pair => pair.Value.Images.Count);
            long imagesSize = pages.Sum(pair => pair.Value.Images.Sum(image => image.Size));
            int imageNum = 0;

            ProgressMax = imageCount * (DecreaseColors == true ? 2 : 1);

            foreach (var pagePair in pages)
            {

                var page = pagePair.Value;

                foreach (var image in page.Images)
                {
                    ++imageNum;
                    SPdfImage newImage = image;

                    using (var ms = new MemoryStream(image.Bytes))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                        int newWidth = (int)Math.Round(iTextSharp.text.Utilities.PointsToInches(image.SizeOnPage.Width * TargetDpi));
                        int newHeight = (int)Math.Round(iTextSharp.text.Utilities.PointsToInches(image.SizeOnPage.Height * TargetDpi));
                        bool imageChanged = false;

                        if (image.Dpi > TargetDpi)
                        {
                            newImage.Width = newWidth;
                            newImage.Height = newHeight;
                            img = PdfImageCompression.ResizeImage(img, newWidth, newHeight);
                            imageChanged = true;
                        }

                        MemoryStream outStream = new MemoryStream();
                        var colorPercent = PdfImageCompression.CountColors(img);
                        if (DecreaseColors == true && colorPercent > 80)
                        {
                            long gifSize, jpegSize;
                            var gifMs = PdfImageCompression.ConvertToGif(img, out gifSize);
                            UpdateProgress();
                            var jpegMs = PdfImageCompression.ConvertToJpeg(img, JpegCompression, out jpegSize);

                            imageChanged = true;
                            outStream = gifSize < jpegSize ? gifMs : jpegMs;
                            imageChanged = true;
                        }
                        else if (imageChanged)
                        {
                            long jpegSize = 0;
                            outStream = PdfImageCompression.ConvertToJpeg(img, JpegCompression, out jpegSize);
                        }

                        if (imageChanged)
                        {
                            newImage.Size = outStream.Length;
                            newImage.Bytes = outStream.ToArray();
                            outImages.Add(newImage);
                        }
                    }

                    UpdateProgress();

                }
            }
            return outImages;
        }

        private void UpdateProgress()
        {
            ++Progress;
            if (OnProgressUpdate != null)
            {
                OnProgressUpdate(Progress, ProgressMax);
            }
        }

        public static Image ResizeImage(Image image, int newWidth, int newHeight)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(image, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));
            }
            return newImage;
        }

        public static MemoryStream ConvertToJpeg(Image image, int quality, out long estimatedSize)
        {
            var outputStream = new MemoryStream();

            ImageCodecInfo codecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality, Convert.ToInt64(quality));

            //                                newImage.Save(ms, codecInfo, parameters);
            //newImage.Save(ms, ImageFormat.Png);
            Bitmap newImage = new Bitmap(image);
            Utils.ProgressUpdater progressObj = new Utils.ProgressUpdater();
            Utils.CurrentOperationUpdater currentOperationObj = new Utils.CurrentOperationUpdater();
            byte[, ,] image_array = Utils.Fill_Image_Buffer(newImage, progressObj, currentOperationObj);

            Point originalDimension = new Point(image.Width, image.Height);
            Point actualDimension = Utils.GetActualDimension(originalDimension);

            BinaryWriter bw = new BinaryWriter(outputStream);

            JpegEncoder.BaseJPEGEncoder encoder = new BaseJPEGEncoder();
            encoder.LuminanceTable = Tables.Standard_Luminance_Quantization_Table;
            encoder.ChromianceTable = Tables.Standard_Chromiance_Quantization_Table;

            encoder.EncodeImageBufferToJpg(image_array, originalDimension, actualDimension,
                bw, float.Parse(quality.ToString()), // Lower quality value better Image
                progressObj, currentOperationObj);

            //var fs = new FileStream(String.Format(@"out\image-{0}.jpg", imageNum), FileMode.Create, FileAccess.Write);
            //fs.Write(outputStream.ToArray(), 0, (int)outputStream.Length);
            outputStream.Position = 0;
            estimatedSize = outputStream.Length;
            return outputStream;
        }

        public static MemoryStream ConvertToGif(Image image, out long estimatedSize)
        {
/*            MemoryStream mss = new MemoryStream();
            image.Save(mss, ImageFormat.Gif);
            estimatedSize = mss.Length;
            mss.Position = 0;
            return mss;
*/            Int32 colorCount = 256;
            IColorQuantizer activeQuantizer = new UniformQuantizer();
            IColorDitherer activeDitherer = null;


            // disables all the controls and starts running
            DateTime before = DateTime.Now;
            var targetImage = ImageBuffer.QuantizeImage(image, activeQuantizer, activeDitherer, colorCount);

            var ms = new MemoryStream(); // estimatedLength can be original fileLength
            {
                targetImage.Save(ms, ImageFormat.Gif); // save image to stream in Jpeg format
                ms.Position = 0;
                estimatedSize = ms.Length;
                return ms;
            }

            // retrieves a GIF image based on our HSB-quantized one
            //                targetImage.Save(outputStream, ImageFormat.Gif);
            //targetImage.Save(String.Format(@"out\image-{0}.gif", imageNum), ImageFormat.Gif);
        }

        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().ToList().Find(delegate(ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }

        public static int CountColors(Image image)
        {
            Dictionary<Color, int> colors = new Dictionary<Color, int>();

            Bitmap bmpCrop = new Bitmap(image);
            for (int h = 0; h < image.Height; h++)
            {
                BitmapData bmpData = bmpCrop.LockBits(new Rectangle(0, h, bmpCrop.Width, 1), System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
                byte[] imgData = new byte[bmpData.Stride];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgData, 0
                                , imgData.Length);
                bmpCrop.UnlockBits(bmpData);
                for (var n = 0; n <= imgData.Length - 3; n += 3)
                {
                    Color col = Color.FromArgb((int)imgData[n], (int)imgData[n + 1], (int)imgData[n + 2]);
                    if (colors.ContainsKey(col) == false)
                    {
                        colors.Add(col, 1);
                    }
                    else
                    {
                        colors[col] += 1;
                    }
                    //cnt.Add(col);
                }
            }
            var tmp = from entry in colors orderby entry.Value descending select entry;
            var colors2 = tmp.ToDictionary(pair => pair.Key, pair => pair.Value);
            var colorsIn256 = colors2.Take(256).Sum(pair => pair.Value);
            var totalPixels = image.Width * image.Height;
            var percent = (colorsIn256 * 100) / totalPixels;
            return percent;
        }

        public static Bitmap Sharpen(Bitmap image)
        {
            Bitmap sharpenImage = (Bitmap)image.Clone();

            int filterWidth = 3;
            int filterHeight = 3;
            int width = image.Width;
            int height = image.Height;

            // Create sharpening filter.
            double[,] filter = new double[filterWidth, filterHeight];
            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.5;
            double bias = 0.0;

            Color[,] result = new Color[image.Width, image.Height];

            // Lock image bits for read/write.
            BitmapData pbits = sharpenImage.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Declare an array to hold the bytes of the bitmap.
            int bytes = pbits.Stride * height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

            int rgb;
            // Fill the color array with the new sharpened color values.
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + width) % width;
                            int imageY = (y - filterHeight / 2 + filterY + height) % height;

                            rgb = imageY * pbits.Stride + 3 * imageX;

                            red += rgbValues[rgb + 2] * filter[filterX, filterY];
                            green += rgbValues[rgb + 1] * filter[filterX, filterY];
                            blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }

            // Update the image with the sharpened pixels.
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    rgb = y * pbits.Stride + 3 * x;

                    rgbValues[rgb + 2] = result[x, y].R;
                    rgbValues[rgb + 1] = result[x, y].G;
                    rgbValues[rgb + 0] = result[x, y].B;
                }
            }

            // Copy the RGB values back to the bitmap.
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
            // Release image bits.
            sharpenImage.UnlockBits(pbits);

            return sharpenImage;
        }


    }
}
