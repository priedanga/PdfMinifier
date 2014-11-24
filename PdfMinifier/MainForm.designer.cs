namespace PdfMinifier
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dpiComboBox = new System.Windows.Forms.ComboBox();
            this.dpiLabel = new System.Windows.Forms.Label();
            this.compressionTrackBar = new System.Windows.Forms.TrackBar();
            this.compressionDropDown = new System.Windows.Forms.NumericUpDown();
            this.compressionLabel = new System.Windows.Forms.Label();
            this.reduceButton = new System.Windows.Forms.Button();
            this.fileLabel = new System.Windows.Forms.Label();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.decreaseColorsCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.compressionTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compressionDropDown)).BeginInit();
            this.SuspendLayout();
            // 
            // dpiComboBox
            // 
            this.dpiComboBox.FormattingEnabled = true;
            this.dpiComboBox.Items.AddRange(new object[] {
            "50",
            "100",
            "150",
            "200",
            "300"});
            this.dpiComboBox.Location = new System.Drawing.Point(93, 49);
            this.dpiComboBox.Name = "dpiComboBox";
            this.dpiComboBox.Size = new System.Drawing.Size(121, 21);
            this.dpiComboBox.TabIndex = 0;
            this.dpiComboBox.Text = "150";
            // 
            // dpiLabel
            // 
            this.dpiLabel.AutoSize = true;
            this.dpiLabel.Location = new System.Drawing.Point(45, 52);
            this.dpiLabel.Name = "dpiLabel";
            this.dpiLabel.Size = new System.Drawing.Size(40, 13);
            this.dpiLabel.TabIndex = 1;
            this.dpiLabel.Text = "Raiška";
            this.dpiLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // compressionTrackBar
            // 
            this.compressionTrackBar.AutoSize = false;
            this.compressionTrackBar.LargeChange = 1;
            this.compressionTrackBar.Location = new System.Drawing.Point(91, 78);
            this.compressionTrackBar.Maximum = 13;
            this.compressionTrackBar.Name = "compressionTrackBar";
            this.compressionTrackBar.Size = new System.Drawing.Size(194, 20);
            this.compressionTrackBar.SmallChange = 10;
            this.compressionTrackBar.TabIndex = 2;
            this.compressionTrackBar.Value = 4;
            this.compressionTrackBar.Scroll += new System.EventHandler(this.jpegQualityTrackBar_Scroll);
            // 
            // compressionDropDown
            // 
            this.compressionDropDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.compressionDropDown.Location = new System.Drawing.Point(291, 78);
            this.compressionDropDown.Maximum = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.compressionDropDown.Name = "compressionDropDown";
            this.compressionDropDown.Size = new System.Drawing.Size(48, 20);
            this.compressionDropDown.TabIndex = 3;
            this.compressionDropDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.compressionDropDown.ValueChanged += new System.EventHandler(this.jpegQualityDropDown_ValueChanged);
            // 
            // compressionLabel
            // 
            this.compressionLabel.AutoSize = true;
            this.compressionLabel.Location = new System.Drawing.Point(15, 79);
            this.compressionLabel.Name = "compressionLabel";
            this.compressionLabel.Size = new System.Drawing.Size(70, 13);
            this.compressionLabel.TabIndex = 1;
            this.compressionLabel.Text = "Suspaudimas";
            this.compressionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // reduceButton
            // 
            this.reduceButton.Enabled = false;
            this.reduceButton.Location = new System.Drawing.Point(139, 320);
            this.reduceButton.Name = "reduceButton";
            this.reduceButton.Size = new System.Drawing.Size(75, 23);
            this.reduceButton.TabIndex = 4;
            this.reduceButton.Text = "Sumažinti";
            this.reduceButton.UseVisualStyleBackColor = true;
            this.reduceButton.Click += new System.EventHandler(this.reduceBtn_Click);
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(51, 22);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(34, 13);
            this.fileLabel.TabIndex = 1;
            this.fileLabel.Text = "Failas";
            this.fileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Location = new System.Drawing.Point(93, 19);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.ReadOnly = true;
            this.fileNameTextBox.Size = new System.Drawing.Size(179, 20);
            this.fileNameTextBox.TabIndex = 5;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(278, 17);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(61, 23);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Pasirinkti";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // messageBox
            // 
            this.messageBox.BackColor = System.Drawing.SystemColors.Control;
            this.messageBox.Location = new System.Drawing.Point(18, 150);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.ReadOnly = true;
            this.messageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageBox.Size = new System.Drawing.Size(321, 164);
            this.messageBox.TabIndex = 7;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(18, 320);
            this.progressBar1.MarqueeAnimationSpeed = 30;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(321, 23);
            this.progressBar1.TabIndex = 8;
            this.progressBar1.Visible = false;
            // 
            // decreaseColorsCheckBox
            // 
            this.decreaseColorsCheckBox.AutoSize = true;
            this.decreaseColorsCheckBox.Checked = true;
            this.decreaseColorsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.decreaseColorsCheckBox.Location = new System.Drawing.Point(18, 127);
            this.decreaseColorsCheckBox.Name = "decreaseColorsCheckBox";
            this.decreaseColorsCheckBox.Size = new System.Drawing.Size(278, 17);
            this.decreaseColorsCheckBox.TabIndex = 9;
            this.decreaseColorsCheckBox.Text = "Bandyti sumažinti spalvų kiekį paveikslėliuose (lėčiau)";
            this.decreaseColorsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label1.Location = new System.Drawing.Point(90, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Mažas";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label2.Location = new System.Drawing.Point(251, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Didėlis";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 355);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.decreaseColorsCheckBox);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.fileNameTextBox);
            this.Controls.Add(this.reduceButton);
            this.Controls.Add(this.compressionDropDown);
            this.Controls.Add(this.compressionTrackBar);
            this.Controls.Add(this.compressionLabel);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.dpiLabel);
            this.Controls.Add(this.dpiComboBox);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PdfMinifier – PDF sumažinimas";
            ((System.ComponentModel.ISupportInitialize)(this.compressionTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compressionDropDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox dpiComboBox;
        private System.Windows.Forms.Label dpiLabel;
        private System.Windows.Forms.TrackBar compressionTrackBar;
        private System.Windows.Forms.NumericUpDown compressionDropDown;
        private System.Windows.Forms.Label compressionLabel;
        private System.Windows.Forms.Button reduceButton;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox decreaseColorsCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}