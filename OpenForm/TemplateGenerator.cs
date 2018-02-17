using Emgu.CV;
using OpenForm.Detection;
using OpenForm.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenForm
{
    public partial class TemplateGenerator : Form
    {
        DetectionTemplate detectionTemplate = new DetectionTemplate();
        BindingSource templateBinding = new BindingSource();
        Preprocessor preprocessor = new Preprocessor(Preprocessor.PageMarksConfig.getDefaultSampleConfig());

        Mat previewImage = new Mat();

        public TemplateGenerator()
        {
            InitializeComponent();
            preprocessor.OnInvalidPageMarksDetected += Preprocessor_OnInvalidPageMarksDetected;
        }

        private void preprocessBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result==DialogResult.OK)
            {
                setRecognitionSettingsToDetectionTemplate();
                preprocessor.pageMarksConfig = new Preprocessor.PageMarksConfig() { MaxArea = detectionTemplate.MaxMarkArea, MinArea = detectionTemplate.MinMarkArea, MostLeftOffset = detectionTemplate.SearchLeftOffset, MostRightOffset = detectionTemplate.SearchRightOffset, MostTopOffset = detectionTemplate.SearchTopOffset, MostBottomOffset = detectionTemplate.SearchBottomOffset, RatioLowerBound = detectionTemplate.RatioLBound, RatioUpperBound = detectionTemplate.RatioUBound } ;
                string file = openFileDialog.FileName;
                preprocessor.beginProcess(file, false);
                preprocessor.saveProcessedImage();
                previewImage = preprocessor.originalThresholded.Clone();
                MessageBox.Show("Please check the application folder to see cv_template.jpg file. Open this file with IrfanView and add detection fields using the fields below.", "Process completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                preprocessor.finalise();
            }
        }

        private void Preprocessor_OnInvalidPageMarksDetected()
        {
            MessageBox.Show("Invalid page marks detected!", "Invalid page marks", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void TemplateGenerator_Load(object sender, EventArgs e)
        {
            templateBinding.DataSource = detectionTemplate.Fields;
            lstFields.DisplayMember = "Ident";
            lstFields.DataSource = templateBinding;
        }

        private void btnAddField_Click(object sender, EventArgs e)
        {
            bool checkNullTextBoxes = string.IsNullOrWhiteSpace(txtTopLeftX.Text) || string.IsNullOrWhiteSpace(txtTopLeftY.Text) || string.IsNullOrWhiteSpace(txtSizeX.Text) || string.IsNullOrWhiteSpace(txtSizeY.Text) || string.IsNullOrWhiteSpace(txtNumOfRows.Text) || string.IsNullOrWhiteSpace(txtNumOfCols.Text) || string.IsNullOrWhiteSpace(txtIdent.Text);
            if (checkNullTextBoxes)
            {
                MessageBox.Show("Please fill in all textboxes. All fields are mandatory.", "Add field", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                if (detectionTemplate.Fields.FindIndex(x => x.Ident == txtIdent.Text) == -1)
                {
                    DetectionField df = new DetectionField();
                    df.TopLeft = new Point(int.Parse(txtTopLeftX.Text), int.Parse(txtTopLeftY.Text));
                    df.Size = new Size(int.Parse(txtSizeX.Text), int.Parse(txtSizeY.Text));
                    df.NumOfRows = int.Parse(txtNumOfRows.Text);
                    df.NumOfCols = int.Parse(txtNumOfCols.Text);
                    df.FieldType = cbxFieldType.SelectedItem.ToString();
                    df.ResultType = cbxResponseType.SelectedItem.ToString();
                    df.Ident = txtIdent.Text;
                    detectionTemplate.addField(df);
                    // Refocus on the first textbox to enter the second field
                    txtTopLeftX.Clear();
                    txtTopLeftY.Clear();
                    txtSizeX.Clear();
                    txtSizeY.Clear();
                    txtNumOfRows.Clear();
                    txtNumOfCols.Clear();
                    txtIdent.Clear();
                    txtTopLeftX.Focus();
                    templateBinding.ResetBindings(false);
                } else
                {
                    MessageBox.Show("The indentification has already existed in the template. Please choose a different identification.","Duplicate field",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnDeleteField_Click(object sender, EventArgs e)
        {
            string selectedIdent = lstFields.GetItemText(lstFields.SelectedItem);
            detectionTemplate.Fields.Remove(detectionTemplate.Fields.Find(x => x.Ident == selectedIdent));
            templateBinding.ResetBindings(false);
        }
        private void setRecognitionSettingsToDetectionTemplate()
        {
            detectionTemplate.MaxMarkArea = (int)nmrMaxArea.Value;
            detectionTemplate.MinMarkArea = (int)nmrMinArea.Value;
            detectionTemplate.SearchLeftOffset = (double)nmrLeftOffset.Value;
            detectionTemplate.SearchRightOffset = (double)nmrRightOffset.Value;
            detectionTemplate.SearchTopOffset = (double)nmrTopOffset.Value;
            detectionTemplate.SearchBottomOffset = (double)nmrBottomOffset.Value;
            detectionTemplate.RatioLBound = (double)nmrRatioLBound.Value;
            detectionTemplate.RatioUBound = (double)nmrRatioUBound.Value;
        }
        private void btnSaveTpl_Click(object sender, EventArgs e)
        {
            setRecognitionSettingsToDetectionTemplate();
            if(saveFileDialog.ShowDialog()==DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    using (Stream stream = File.Open(filePath, FileMode.Create))
                    {
                        BinaryFormatter bin = new BinaryFormatter();
                        bin.Serialize(stream, detectionTemplate);
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("An error has occurred while trying to save the file: " + e.ToString());
                }
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {

            using (Mat drawingBoard = previewImage.Clone())
            { 
                Emgu.CV.Structure.MCvScalar redColor = new Emgu.CV.Structure.MCvScalar(0, 0, 255);
                // Draw detection fields on preview image and let user see it!
                foreach (DetectionField f in detectionTemplate.Fields)
                {
                    CvInvoke.Rectangle(drawingBoard, new Rectangle(f.TopLeft, f.Size), redColor);
                    for (int i = 1; i < f.NumOfCols; i++)
                    {
                        CvInvoke.Line(drawingBoard, new Point(f.TopLeft.X + f.Size.Width * i / f.NumOfCols, f.TopLeft.Y), new Point(f.TopLeft.X + f.Size.Width * i / f.NumOfCols, f.TopLeft.Y + f.Size.Height), redColor);
                    }
                    for (int i = 1; i < f.NumOfRows; i++)
                    {
                        CvInvoke.Line(drawingBoard, new Point(f.TopLeft.X, f.TopLeft.Y + f.Size.Height * i / f.NumOfRows), new Point(f.TopLeft.X + f.Size.Width, f.TopLeft.Y + f.Size.Height * i / f.NumOfRows), redColor);
                    }
                }
                Emgu.CV.UI.ImageViewer imv = new Emgu.CV.UI.ImageViewer(drawingBoard);
                imv.Show();
            }
        }

        private void TemplateGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            previewImage.Dispose();
        }

        private void btnExtension_Click(object sender, EventArgs e)
        {
            Extensions.FormPhieuDangKyB extForm = new Extensions.FormPhieuDangKyB();
            this.detectionTemplate = extForm.template;
            MessageBox.Show("Extension is completed");
        }
    }
}
