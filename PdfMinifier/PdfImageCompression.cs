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
                        bool imageChanged = false;

                        // resize image if calculated image dpi is larger than target dpi
                        if (image.Dpi > TargetDpi)
                        {

                            newImage.Width = (int)Math.Round(iTextSharp.text.Utilities.PointsToInches(image.SizeOnPage.Width) * TargetDpi);
                            newImage.Height = (int)Math.Round(iTextSharp.text.Utilities.PointsToInches(image.SizeOnPage.Height) * TargetDpi);
                            img = PdfImageCompression.ResizeImage(img, newImage.Width, newImage.Height);
                            imageChanged = true;
                        }

                        MemoryStream outStream = new MemoryStream();

                        // сумма пикселей 256-ти наиболее встречаемых оттенков
                        // 256 dažniausiai sutinkamų spalvų pikselių suma
                        // sum of pixels of 256 most common colors
                        var colorCount = PdfImageCompression.GetImageColorCount256(img);

                        // calculate what percent of total image pixels are those 256 most common colors
                        var colorPercent = (colorCount * 100) / (image.Width * image.Height);

                        // if 256 most common colors are in more than 85% of total image pixels
                        // then try to convert this image to gif
                        if (DecreaseColors == true && colorPercent >= 85)
                        {
                            long gifSize, jpegSize;
                            var gifMs = PdfImageCompression.ConvertToGif(img, out gifSize);
                            UpdateProgress();
                            var jpegMs = PdfImageCompression.ConvertToJpeg(img, JpegCompression, out jpegSize);
                            
                            // we convert image to gif and to jpeg to see which one is smaller in size
                            // because with strong jpeg compression it can be smaller than 256 color gif
                            outStream = gifSize < jpegSize ? gifMs : jpegMs;
                            imageChanged = true;
                        }
                        else if (imageChanged)
                        {
                            // convert to jpeg if image was resized and has
                            // less than 85% of pixels in 256 most common colors
                            long jpegSize = 0;
                            outStream = PdfImageCompression.ConvertToJpeg(img, JpegCompression, out jpegSize);
                        }

                        if (imageChanged)
                        {
                            // image was resized or converted to gif/jpeg then save it
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

            Bitmap newImage = new Bitmap(image);
            Utils.ProgressUpdater progressObj = new Utils.ProgressUpdater();
            Utils.CurrentOperationUpdater currentOperationObj = new Utils.CurrentOperationUpdater();
            byte[, ,] imageArray = Utils.Fill_Image_Buffer(newImage, progressObj, currentOperationObj);

            Point originalDimension = new Point(image.Width, image.Height);
            Point actualDimension = Utils.GetActualDimension(originalDimension);

            BinaryWriter bw = new BinaryWriter(outputStream);

            JpegEncoder.BaseJPEGEncoder encoder = new BaseJPEGEncoder();
            encoder.LuminanceTable = Tables.Standard_Luminance_Quantization_Table;
            encoder.ChromianceTable = Tables.Standard_Chromiance_Quantization_Table;

            encoder.EncodeImageBufferToJpg(
                imageArray, 
                originalDimension, 
                actualDimension,
                bw, 
                quality, // Lower quality value better Image
                progressObj, 
                currentOperationObj);

            outputStream.Position = 0;
            estimatedSize = outputStream.Length;
            return outputStream;
        }

        public static MemoryStream ConvertToGif(Image image, out long estimatedSize)
        {
            Int32 colorCount = 256;
            IColorQuantizer activeQuantizer = new UniformQuantizer();
            IColorDitherer activeDitherer = null;

            var targetImage = ImageBuffer.QuantizeImage(image, activeQuantizer, activeDitherer, colorCount);
            var ms = new MemoryStream();
            {
                targetImage.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                estimatedSize = ms.Length;
                return ms;
            }
        }

        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().ToList().Find(delegate(ImageCodecInfo codec)
            {
                return codec.FormatID == format.Guid;
            });
        }

        public static int GetImageColorCount256(Image image)
        {
            var colors = GetImageColors(image);
            return GetImageColorCount256(colors);
        }

        public static int GetImageColorCount256(Dictionary<Color, int> colors)
        {
            return colors.Take(256).Sum(pair => pair.Value);
        }

        public static Dictionary<Color, int> GetImageColors(Image image)
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
                }
            }
            var sortedColors = from entry in colors orderby entry.Value descending select entry;
            return (Dictionary<Color, int>)sortedColors.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

    }
}
