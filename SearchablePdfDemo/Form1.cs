using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Atalasoft.Imaging;
using Atalasoft.Ocr;
using Atalasoft.Ocr.GlyphReader;
using Atalasoft.Imaging.ImageProcessing;
using Atalasoft.Imaging.ImageProcessing.Document;
using Atalasoft.Imaging.Codec;

namespace SearchablePDFDemo
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnSelectImage;
		private System.Windows.Forms.Button btnProcessImage;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblFilename;
		private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.CheckBox chkAutoInvertText;
		private System.Windows.Forms.CheckBox chkFixPageOrientation;
		private System.Windows.Forms.CheckBox chkCropBorders;
		private System.Windows.Forms.CheckBox chkRemoveLines;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		string _filename;
		GlyphReaderEngine _engine;
		PdfTranslator _pdfTrans;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Button btnAbout;
		int _pageNum;

		static Form1()
		{
			//load the OCR resources
			GlyphReaderLoader loader = new GlyphReaderLoader();

            AtalaDemos.HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders);
        }

		public Form1()
		{
			if (CheckLicenseFile())
			{
				InitializeComponent();
				_engine = new GlyphReaderEngine();
				_pdfTrans = new PdfTranslator();
				_engine.Translators.Add(_pdfTrans);
				_engine.PageProgress += new OcrPageProgressEventHandler(_engine_PageProgress);
				_engine.DocumentProgress += new OcrDocumentProgressEventHandler(_engine_DocumentProgress);
				_engine.ImageTransformation += new OcrImagePreprocessingEventHandler(_engine_ImageTransformation);
				_engine.ImageSendOff += new OcrImagePreprocessingEventHandler(_engine_ImageSendOff);
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.btnProcessImage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkRemoveLines = new System.Windows.Forms.CheckBox();
            this.chkCropBorders = new System.Windows.Forms.CheckBox();
            this.chkFixPageOrientation = new System.Windows.Forms.CheckBox();
            this.chkAutoInvertText = new System.Windows.Forms.CheckBox();
            this.lblFilename = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnAbout = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectImage
            // 
            this.btnSelectImage.Location = new System.Drawing.Point(8, 16);
            this.btnSelectImage.Name = "btnSelectImage";
            this.btnSelectImage.Size = new System.Drawing.Size(80, 23);
            this.btnSelectImage.TabIndex = 0;
            this.btnSelectImage.Text = "Select Image";
            this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
            // 
            // btnProcessImage
            // 
            this.btnProcessImage.Location = new System.Drawing.Point(8, 184);
            this.btnProcessImage.Name = "btnProcessImage";
            this.btnProcessImage.Size = new System.Drawing.Size(152, 23);
            this.btnProcessImage.TabIndex = 1;
            this.btnProcessImage.Text = "Generate Searchable PDF";
            this.btnProcessImage.Click += new System.EventHandler(this.btnProcessImage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkRemoveLines);
            this.groupBox1.Controls.Add(this.chkCropBorders);
            this.groupBox1.Controls.Add(this.chkFixPageOrientation);
            this.groupBox1.Controls.Add(this.chkAutoInvertText);
            this.groupBox1.Location = new System.Drawing.Point(8, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 128);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pre-processing Options";
            // 
            // chkRemoveLines
            // 
            this.chkRemoveLines.Location = new System.Drawing.Point(16, 96);
            this.chkRemoveLines.Name = "chkRemoveLines";
            this.chkRemoveLines.Size = new System.Drawing.Size(104, 24);
            this.chkRemoveLines.TabIndex = 3;
            this.chkRemoveLines.Text = "Remove Lines";
            // 
            // chkCropBorders
            // 
            this.chkCropBorders.Location = new System.Drawing.Point(16, 72);
            this.chkCropBorders.Name = "chkCropBorders";
            this.chkCropBorders.Size = new System.Drawing.Size(104, 24);
            this.chkCropBorders.TabIndex = 2;
            this.chkCropBorders.Text = "Crop Borders";
            // 
            // chkFixPageOrientation
            // 
            this.chkFixPageOrientation.Checked = true;
            this.chkFixPageOrientation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFixPageOrientation.Location = new System.Drawing.Point(16, 48);
            this.chkFixPageOrientation.Name = "chkFixPageOrientation";
            this.chkFixPageOrientation.Size = new System.Drawing.Size(128, 24);
            this.chkFixPageOrientation.TabIndex = 1;
            this.chkFixPageOrientation.Text = "Fix Page Orientation";
            // 
            // chkAutoInvertText
            // 
            this.chkAutoInvertText.Location = new System.Drawing.Point(16, 24);
            this.chkAutoInvertText.Name = "chkAutoInvertText";
            this.chkAutoInvertText.Size = new System.Drawing.Size(104, 24);
            this.chkAutoInvertText.TabIndex = 0;
            this.chkAutoInvertText.Text = "Auto Invert Text";
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(96, 24);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(37, 13);
            this.lblFilename.TabIndex = 3;
            this.lblFilename.Text = "(none)";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(8, 216);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(51, 13);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Text = "Progress:";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "pdf";
            this.saveFileDialog1.Filter = "Searchable PDF|*.pdf";
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(205, 184);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(75, 23);
            this.btnAbout.TabIndex = 5;
            this.btnAbout.Text = "About ...";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 238);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnProcessImage);
            this.Controls.Add(this.btnSelectImage);
            this.Name = "Form1";
            this.Text = "Image to Searchable PDF";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		void _engine_ImageTransformation(object sender, OcrImagePreprocessingEventArgs e)
		{
			//this is where we do things to the image that we want to be viewed in the searchable PDF
			//but change the size or coordinates of the image
			if (chkCropBorders.Checked)
			{
				if (e.ImageIn.PixelFormat == PixelFormat.Pixel1bppIndexed)
				{
					//only apply if source image is bitonal
					AdvancedBorderRemovalCommand ab = new AdvancedBorderRemovalCommand();
					ImageResults res = ab.Apply(e.ImageIn);
					e.ImageOut = res.Image;
				}
			}
		}

		void _engine_ImageSendOff(object sender, OcrImagePreprocessingEventArgs e)
		{
			AtalaImage image = e.ImageIn;
			if (e.ImageIn.PixelFormat != PixelFormat.Pixel1bppIndexed)
			{
				//threshold the image here (OCR engine will automatically threshold otherwise)
				AdaptiveThresholdCommand threshold = new AdaptiveThresholdCommand();
				ImageResults res = threshold.Apply(image);
				image = res.Image;
                
			}
			//this is where we cleanup the document that is OCR'ed but not included in the searchable PDF
			if (chkAutoInvertText.Checked)
			{

				AutoInvertTextCommand at = new AutoInvertTextCommand();
				ImageResults res = at.Apply(image);
				if (!res.IsImageSourceImage && image != e.ImageIn)
					image.Dispose();
				image = res.Image;
			}

			//this is where we cleanup the document that is OCR'ed but not included in the searchable PDF
			if (chkRemoveLines.Checked)
			{
				LineRemovalCommand at = new LineRemovalCommand();
				ImageResults res = at.Apply(image);
				if (!res.IsImageSourceImage && image != e.ImageIn)
					image.Dispose();
				image = res.Image;
			}
			if (image != e.ImageIn)
				e.ImageOut = image;
		}

		void _engine_DocumentProgress(object sender, OcrDocumentProgressEventArgs e)
		{
			if (e.Stage == OcrDocumentStage.BeginPage)
				_pageNum++; // increment the current page count
		}

		void _engine_PageProgress(object sender, OcrPageProgressEventArgs e)
		{
			lblProgress.Text = e.Stage.ToString() + " Page " + _pageNum + ": " + e.Progress + "%...";
			lblProgress.Refresh();
		}

		private void btnSelectImage_Click(object sender, EventArgs e)
		{
            this.openFileDialog1.Filter = AtalaDemos.HelperMethods.CreateDialogFilter(true);

            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
			{
				_filename = openFileDialog1.FileName;
				lblFilename.Text = _filename;
			}
		}

		private void btnProcessImage_Click(object sender, EventArgs e)
		{
            saveFileDialog1.Filter = "Portable Document Format (PDF)|*.Pdf";

			if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
			{
				_engine.Initialize();
				try
				{
					_pageNum = 0;
					this.Cursor = Cursors.WaitCursor;
					_engine.PreprocessingOptions.AutoRotate = chkFixPageOrientation.Checked;
					_pdfTrans.AutoPageRotation = chkFixPageOrientation.Checked;
					FileSystemImageSource fs = new FileSystemImageSource(new string[] { _filename }, true);
					_engine.Translate(fs, "application/pdf", saveFileDialog1.FileName);
					System.Diagnostics.Process.Start(saveFileDialog1.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.ToString());
				}
				finally
				{
					_engine.ShutDown();
					this.Cursor = Cursors.Default;
				}
			}
		}

		#region Check for license code

		private bool CheckGRLicense()
		{
			try
			{
				GlyphReaderEngine gr = new GlyphReaderEngine(); // does not throw
				gr.Initialize(); // will throw on no license
				gr.Dispose();
				return true;
			}
			catch(AtalasoftLicenseException)
			{
				return false;
			}
		}

		private bool CheckLicenseFile()
		{
			// Make sure a license for DotImage and OCR exist.
			try
			{
				AtalaImage img = new AtalaImage();
				img.Dispose();
			}
			catch (Atalasoft.Imaging.AtalasoftLicenseException ex1)
			{
				LicenseCheckFailure(ex1.Message);
				return false;
			}
			
			if (AtalaImage.Edition != LicenseEdition.Document)
			{
				LicenseCheckFailure("This demo requires a Document Imaging License.\r\nYour current license is for '" + AtalaImage.Edition.ToString() + "'.");
				return false;
			}

			try
			{
				TranslatorCollection t = new TranslatorCollection();
			}
			catch(AtalasoftLicenseException)
			{
				LicenseCheckFailure("This demo requires an OCR license.");
				return false;
			}

			if (CheckGRLicense())							
				return true;
			else			
			{
				LicenseCheckFailure("GlyphReader is not licensed on your system.  Please request an evaluation license for it before running this demo.");
				return false;
			}
		}

		private void LicenseCheckFailure(string message)
		{
			this.Load += new EventHandler(LoadFailure);
			if (MessageBox.Show(this, message + "\r\n\r\nWould you like to request an evaluation license?", "License Required", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				System.Reflection.Assembly asm = System.Reflection.Assembly.Load("Atalasoft.dotImage");
				if (asm != null)
				{
					string version = asm.GetName().Version.ToString(2);

					// Locate the activation utility.
					string path = "";
					Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Atalasoft\dotImage\" + version);
					if (key != null)
					{
						path = Convert.ToString(key.GetValue("AssemblyBasePath"));
						if (path != null && path.Length > 5)
							path = path.Substring(0, path.Length - 3) + "AtalasoftToolkitActivation.exe";
						else
							path = Path.GetFullPath(@"..\..\..\..\..\AtalasoftToolkitActivation.exe");

						key.Close();
					}

					if (System.IO.File.Exists(path))
						System.Diagnostics.Process.Start(path);
					else
						MessageBox.Show(this, "We were unable to location the DotImage activation utility.\r\nPlease run it from the Start menu shortcut.", "File Not Found");
				}
				else
					MessageBox.Show(this, "Unable to load the DotImage assembly.", "Load Error");
			}
		}

		private void LoadFailure(object sender, EventArgs e)
		{
			Application.Exit();
		}
		#endregion

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AtalaDemos.AboutBox.About aboutBox = new AtalaDemos.AboutBox.About("About Atalasoft Searchable PDF Demo", "Searchable PDF Demo");
            aboutBox.Description = "This demo uses our OCR engine to convert an input image (single or multi-page) into a searchable PDF using a GlyphReaderEngine and our PdfTranslator class.\r\n\r\n" +
                                   "Pre-Processing options (deskewing, border removal, text inversion and line removal) are also provided.\r\n\r\n" +
                                   "This winforms application is fairly bare-bones, but the concepts covered can easily be applied to your console app, windows service, web service, or can even be used server-side in your ASP.NET or Silverlight web application.";
            aboutBox.ShowDialog();
        }

	}
}
