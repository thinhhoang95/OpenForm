﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenForm
{
    public partial class frmMain : Form
    {
        Detection.DetectionTemplate detectionTemplate;
        DataTable progressToShow;
        Result.ResultPresenter presenter;

        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtTemplateFile.Text = openFileDialog1.FileName;
                this.detectionTemplate = deserializeTemplateFile(openFileDialog1.FileName);
                MessageBox.Show("The template has been loaded successfully.");
            }
        }
        private Detection.DetectionTemplate deserializeTemplateFile(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                object obj;
                try
                {
                    obj = binaryFormatter.Deserialize(streamReader.BaseStream);
                }
                catch (SerializationException ex)
                {
                    throw new SerializationException(((object)ex).ToString() + "\n" + ex.Source);
                }
                return (Detection.DetectionTemplate)obj;
            }
        }

        private void detectNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool saveRecognitionResult;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Do you want to save the recognition results along with the file? Usually, the answer is NO.", "Save recognition results", MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    saveRecognitionResult = true;
                } else
                {
                    saveRecognitionResult = false;
                }
                Engine.DetectionThread dThread = new Engine.DetectionThread(detectionTemplate, int.Parse(txtThreshold.Text), openFileDialog2.FileNames, updateGrid, updatePresenter, saveRecognitionResult);
                ThreadStart detectThreadStart = new ThreadStart(dThread.startDetectionProcess);
                Thread detectThread = new Thread(detectThreadStart);

                progressToShow = new DataTable();
                progressToShow.Columns.Add("File name");
                progressToShow.Columns.Add("Status");

                detectThread.Start();
                detectThread.Join();
                MessageBox.Show("The detection process has completed successfully! Results saved in form.xlsx in the application folder.", "Process completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void updateGrid (string file, string result)
        {
            // On receive callback
            DataRow r = progressToShow.NewRow();
            r["File name"] = file; r["Status"] = result;
            progressToShow.Rows.Add(r);
            Action updateGridView = () =>
            {
                dataGridView1.DataSource = progressToShow;
            };
            this.BeginInvoke(updateGridView);
        }

        private void updatePresenter(Result.ResultPresenter presenter)
        {
            this.presenter = presenter;
        }

        private void dataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.FillWeight = 2;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("OpenForm BETA  (a parody of OpenFOAM). Created by Hoang Dinh Thinh (hoangdinhthinh@live.com). All rights reserved.");
        }

        private void showDetailOfSelectedRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("Please select a row in the grid first");
            else
            {
                DataRow r = presenter.table.Rows[dataGridView1.SelectedRows[0].Index];
                DataColumnCollection columns = presenter.table.Columns;
                frmViewResponse responseView = new frmViewResponse(r, columns);
                responseView.Show();
            }
        }

        private void openApplicationFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            System.Diagnostics.Process.Start(path);
        }

        private void templateCreateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TemplateGenerator tplGen = new TemplateGenerator();
            tplGen.Show();
        }
    }
}
