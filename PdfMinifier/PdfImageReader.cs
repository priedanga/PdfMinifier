using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Drawing.Imaging;
using SystemImage = System.Drawing.Image;
using PointF = System.Drawing.PointF;
using iTextSharp.text;

namespace PdfMinifier
{
    public struct SPdfImage
    {
        // image file format (jpeg, gif, png)
        public string Format;

        // number of a page where this image is found
        public int Page;

        // Size in bytes of an image
        public long Size;

        // width and height in pixels
        public int Width;
        public int Height;

        // actual image data
        public byte[] Bytes;

        // reference number of image in pdf page
        public int RefNumber;

        // size of image on a page in points (72 points = 1 inch)
        // this is for calculating image dpi
        // and for placing image back to pdf
        public Rectangle SizeOnPage;

        // the image position on a page
        public PointF Position;

        // Resolution of an image
        // WidthOfImageInPixels / WidthOfImageOnAPageInInches
        public int Dpi;
    }

    public struct SPage
    {
        // images on a page
        public List<SPdfImage> Images;

        // width and height of a page in mm
        public float Width;
        public float Height;

        // number of a page
        public int Number;
    };

    class PdfImageReader
    {

        public delegate void ProgressUpdate(int value, int max);
        public event ProgressUpdate OnProgressUpdate;

        public Dictionary<int, SPage> Read(PdfReader reader, string fileName)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            MyImageRenderListener listener = new MyImageRenderListener();
            int pagesCount = reader.NumberOfPages;

            for (int pageNum = 1; pageNum <= pagesCount; pageNum++)
            {

                PdfDictionary page = reader.GetPageN(pageNum);
                iTextSharp.text.Rectangle pageSize = reader.GetPageSizeWithRotation(page);
                var pageWidth = (int)Utilities.PointsToMillimeters(pageSize.Width);
                var pageHeight = (int)Utilities.PointsToMillimeters(pageSize.Height);

                listener.SetPage(pageNum, pageWidth, pageHeight);
                parser.ProcessContent(pageNum, listener);
                if (OnProgressUpdate != null)
                {
                    OnProgressUpdate(pageNum, pagesCount);
                }
            }

            return listener.Pages;

        }

        public bool IsReusable { get { return false; } }
        /*
         * see: TextRenderInfo & RenderListener classes here:
         * http://api.itextpdf.com/itext/
         * 
         * and Google "itextsharp extract images"
         */
        public class MyImageRenderListener : IRenderListener
        {
            public void RenderText(TextRenderInfo renderInfo) { }
            public void BeginTextBlock() { }
            public void EndTextBlock() { }

            public Dictionary<int, SPage> Pages = new Dictionary<int, SPage>();
            private int PageNum = 1;

            public void SetPage(int pageNum, float pageWidth, float pageHeight)
            {
                PageNum = pageNum;
                SPage pg;
                pg.Width = pageWidth;
                pg.Height = pageHeight;
                pg.Images = new List<SPdfImage>();
                pg.Number = pageNum;
                Pages.Add(pageNum, pg);

            }

            public void RenderImage(ImageRenderInfo renderInfo)
            {
                PdfImageObject image = renderInfo.GetImage();
                Matrix matrix = renderInfo.GetImageCTM();
                Rectangle sizeOnPage = new Rectangle(Math.Abs(matrix[0]), Math.Abs(matrix[4]));
                PointF positionOnPage = new PointF(matrix[6], matrix[7]);

                if (renderInfo.GetRef() == null) return;

                PdfName filter = image.Get(PdfName.FILTER) as PdfName;
                if (filter == null)
                {
                    PdfArray pa = (PdfArray)image.Get(PdfName.FILTER);
                    if (pa == null) return;
                    for (int i = 0; i < pa.Size; ++i)
                    {
                        filter = (PdfName)pa[i];
                    }
                }

                switch (filter.ToString())
                {
                    case "/FlateDecode":
                    case "/DCTDecode":
                        //case "/CCITTFaxDecode":
                        using (SystemImage dotnetImg = image.GetDrawingImage())
                        {
                            if (dotnetImg != null)
                            {
                                var format = GetImageFormat(dotnetImg);

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    dotnetImg.Save(ms, dotnetImg.RawFormat);
                                    SPdfImage pdfImage;
                                    pdfImage.RefNumber = renderInfo.GetRef().Number;
                                    pdfImage.Bytes = ms.ToArray();
                                    pdfImage.Format = format;
                                    pdfImage.Page = PageNum;
                                    pdfImage.Size = ms.Length;
                                    pdfImage.Width = dotnetImg.Width;
                                    pdfImage.Height = dotnetImg.Height;
                                    pdfImage.SizeOnPage = sizeOnPage;
                                    pdfImage.Position = positionOnPage;
                                    var widthInInches = Utilities.PointsToInches(sizeOnPage.Width);
                                    pdfImage.Dpi = (int)Math.Round(dotnetImg.Width / widthInInches);
                                    Pages[PageNum].Images.Add(pdfImage);
                                }
                            }
                        }
                        break;
                }
            }

            private string GetImageFormat(SystemImage image)
            {
                string format = "unknown";

                if (ImageFormat.Jpeg.Equals(image.RawFormat)) format = "jpg";
                else if (ImageFormat.Png.Equals(image.RawFormat)) format = "png";
                else if (ImageFormat.Gif.Equals(image.RawFormat)) format = "gif";
                else if (ImageFormat.Bmp.Equals(image.RawFormat)) format = "bmp";
                else if (ImageFormat.Tiff.Equals(image.RawFormat)) format = "tif";

                return format;
            }

        }
    }
}