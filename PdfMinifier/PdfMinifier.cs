using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using System.IO;

namespace PdfMinifier
{
    class PdfMinifier
    {
        string FileName;
        PdfReader Reader;
        Dictionary<int, SPage> Pages;
        List<SPdfImage> ProcessedImages;

        public delegate void ProgressUpdate(int value, int max);
        public event ProgressUpdate OnCompressorProgressUpdate;
        public event ProgressUpdate OnReaderProgressUpdate;

        public PdfMinifier(string fileName = "")
        {
            if (fileName != "")
            {
                LoadFile(fileName);
            }
        }

        public Dictionary<int, SPage> GetPages()
        {
            return Pages;
        }

        public void Process(int dpi, int compression, bool decreaseColors)
        {
            var compressor = new PdfImageCompression();
            compressor.OnProgressUpdate += (int value, int max) =>
                {
                    ProgressUpdate handler = OnCompressorProgressUpdate;
                    if (handler != null)
                    {
                        handler(value, max);
                    }
                };
            compressor.TargetDpi = dpi;
            compressor.JpegCompression = compression;
            compressor.DecreaseColors = decreaseColors;
            ProcessedImages = compressor.ProcessImages(Pages);
        }

        public void Save(string fileName)
        {
            PdfImageWriter.Write(Reader, ProcessedImages, fileName);
        }

        public void LoadFile(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("Pdf dokumentas nerastas: " + fileName);
            }

            Reader = new PdfReader(fileName);
            PdfImageReader imageReader = new PdfImageReader();

            imageReader.OnProgressUpdate += (int value, int max) =>
            {
                ProgressUpdate handler = OnReaderProgressUpdate;
                if (handler != null)
                {
                    handler(value, max);
                }
            };


            Pages = imageReader.Read(Reader, fileName);
            FileName = fileName;
        }

    }
}
