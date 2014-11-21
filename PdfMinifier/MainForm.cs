using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PdfMinifier
{
    public partial class MainForm : Form
    {

        PdfMinifier Minifier;
        long PdfFileSizeBefore;
        long PdfFileSizeAfter;

        public MainForm()
        {
            InitializeComponent();
            Minifier = new PdfMinifier();
            Minifier.OnCompressorProgressUpdate += OnCompressorProgressUpdate;
            Minifier.OnReaderProgressUpdate += OnReaderProgressUpdate;
        }

        public void setFileName(string fileName)
        {
            fileNameTextBox.Text = fileName;
        }

        private void jpegQualityTrackBar_Scroll(object sender, EventArgs e)
        {
            compressionDropDown.Value = (sender as TrackBar).Value;
        }

        private void jpegQualityDropDown_ValueChanged(object sender, EventArgs e)
        {
            compressionTrackBar.Value = (int) (sender as NumericUpDown).Value;
        }

        private void reduceBtn_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            BackgroundWorker m_oWorker = new BackgroundWorker();

            m_oWorker.DoWork += new DoWorkEventHandler(PdfMinifyDoWork);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnPdfMinifyCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.RunWorkerAsync();
        }

        private void PdfMinifyDoWork(object sender, EventArgs e)
        {
            int compression = 4;
            int dpi = 150;
            var decreaseColorCount = true;
            base.Invoke((Action)delegate
            {
                compression = (int)compressionDropDown.Value;
                dpi = Convert.ToInt32(dpiComboBox.Text);
                decreaseColorCount = decreaseColorsCheckBox.Checked;
            });
            compression = 50 + compression * 50;
            Minifier.Process(dpi, compression, decreaseColorCount);
            Minifier.Save(fileNameTextBox.Text.Replace(".pdf", "-small.pdf"));
        }

        private void OnCompressorProgressUpdate(int value, int max)
        {
            base.Invoke((Action)delegate
            {
                progressBar1.Value = value;
                progressBar1.Maximum = max;
            });
        }

        private void OnReaderProgressUpdate(int value, int max)
        {
            base.Invoke((Action)delegate
            {
                progressBar1.Value = value;
                progressBar1.Maximum = max;
            });
        }

        private void OnPdfMinifyCompleted(object sender, EventArgs e)
        {
            ToggleControls(true);
            PdfFileSizeAfter = new System.IO.FileInfo(fileNameTextBox.Text.Replace(".pdf", "-small.pdf")).Length;
            var decreasePercent = 100 - Math.Round((double)((PdfFileSizeAfter * 100) / PdfFileSizeBefore), 2);
            string message = String.Format("Viskas!{0}Pdf dydis buvo: {1}{0}Pdf dydis dabar: {2}{0}Pdf failo dydis sumažėjo {3}%", Environment.NewLine, FormatBytes(PdfFileSizeBefore), FormatBytes(PdfFileSizeAfter), decreasePercent);
            MessageBox.Show(message, "PDF Sumažinimas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Multiselect = false;
            d.Filter = "PDF dokumentas (.pdf)|*.pdf";
            DialogResult dr = d.ShowDialog(this);

            if (dr != DialogResult.Cancel && d.FileNames.Length > 0)
            {
                fileNameTextBox.Text = d.FileName;
                LoadFile(d.FileName);

            }

        }

        public void LoadFile(string fileName)
        {
            PdfFileSizeBefore = new System.IO.FileInfo(fileName).Length;

            ToggleControls(false);
            BackgroundWorker m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler((sender, e) => 
            {
                Minifier.LoadFile(fileName);
            });
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    ((sender, e) => 
                    {

                        if (e.Error != null)
                        {
                            MessageBox.Show("Įvyko klaida bandant atydaryti PDF failą", "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            messageBox.Text = GenerateImageInfo();
                        }
                        ToggleControls(true);

                    });
            m_oWorker.RunWorkerAsync();
        }

        public string GenerateImageInfo()
        {
            string message = "";
            string NL = Environment.NewLine;
            var pages = Minifier.GetPages();

            foreach (var pagePair in pages)
            {
                var page = pagePair.Value;

                message += String.Format("Puslapis #{0} {1}x{2}mm", page.Number, page.Width, page.Height) + NL;
                message += String.Format("Puslapyje yra {0} paveikslėlis(-ių)", page.Images.Count) + NL;

                for (int i = 0; i < page.Images.Count; ++i)
                {
                    var image = page.Images[i];
                    message += String.Format("#{0}: {1} {2} dpi {3}x{4}   {5}", i + 1, image.Format, image.Dpi, image.Width, image.Height, FormatBytes(image.Size)) + NL;
                }
                message += "---------------------------------------------------------------------------" + NL;
            }
            return message;
        }

        private void ToggleControls(bool enabled)
        {
            reduceButton.Enabled = enabled;
            reduceButton.Visible = enabled;
            dpiComboBox.Enabled = enabled;
            browseButton.Enabled = enabled;
            compressionDropDown.Enabled = enabled;
            compressionTrackBar.Enabled = enabled;
            progressBar1.Visible = !enabled;
        }

        private string FormatBytes(Int64 value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0) { return "-" + FormatBytes(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n2} {1}", adjustedSize, SizeSuffixes[mag]);
        }

    }
}
