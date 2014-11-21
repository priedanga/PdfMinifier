using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using System.IO;
using iTextImage = iTextSharp.text.Image;

namespace PdfMinifier
{
    class PdfImageWriter
    {

        public static void Write(PdfReader reader, List<SPdfImage> images, string outputFileName)
        {
            FileStream fs = new FileStream(outputFileName, FileMode.Create);
            PdfStamper stp = new PdfStamper(reader, fs);
            PdfWriter writer = stp.Writer;

            int pageCount = reader.NumberOfPages;
            for (int pageNum = 1; pageNum <= pageCount; pageNum++)
            {
                PdfDictionary page = reader.GetPageN(pageNum);
                PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
                PdfDictionary xObjects = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));

                if (xObjects == null)
                {
                    continue;
                }

                bool skip = false;
                foreach (PdfName name in xObjects.Keys)
                {
                    if (skip == true) break;
                    
                    PdfObject xObject = xObjects.Get(name);
                    if (xObject.IsIndirect() == false)
                    {
                        continue;
                    }

                    int refNum = ((PRIndirectReference)xObject).Number;

                    var imageObj = GetImageByReferenceId(refNum, images);
                    if (imageObj == null)
                    {
                        continue;
                    }

                    var image = imageObj.Value;
                    iTextImage pdfImage = iTextImage.GetInstance(image.Bytes);
                    pdfImage.ScaleToFit(image.SizeOnPage);
                    pdfImage.SetAbsolutePosition(image.Position.X, image.Position.Y);
                    writer.AddDirectImageSimple(pdfImage, (PRIndirectReference)xObject);
                    PdfReader.KillIndirect(xObject);

                }


            }

            stp.Writer.CloseStream = false;
            stp.Close();
            fs.Close();
            return;
        }

        private static SPdfImage? GetImageByReferenceId(int refId, List<SPdfImage> images)
        {
            foreach (var image in images)
            {
                if (image.RefNumber == refId)
                    return image;
            }

            return null;
        }
    }
}
