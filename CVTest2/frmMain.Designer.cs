namespace CVTest2
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.loadImgBtn = new System.Windows.Forms.Button();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.findPaperMarksBtn = new System.Windows.Forms.Button();
            this.RectSize = new System.Windows.Forms.TextBox();
            this.RectMaxSize = new System.Windows.Forms.TextBox();
            this.deSkewBtn = new System.Windows.Forms.Button();
            this.startDetectBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // loadImgBtn
            // 
            this.loadImgBtn.Location = new System.Drawing.Point(525, 13);
            this.loadImgBtn.Name = "loadImgBtn";
            this.loadImgBtn.Size = new System.Drawing.Size(177, 23);
            this.loadImgBtn.TabIndex = 1;
            this.loadImgBtn.Text = "Load Image";
            this.loadImgBtn.UseVisualStyleBackColor = true;
            this.loadImgBtn.Click += new System.EventHandler(this.loadImgBtn_Click);
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(13, 13);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(490, 508);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // findPaperMarksBtn
            // 
            this.findPaperMarksBtn.Location = new System.Drawing.Point(525, 43);
            this.findPaperMarksBtn.Name = "findPaperMarksBtn";
            this.findPaperMarksBtn.Size = new System.Drawing.Size(177, 23);
            this.findPaperMarksBtn.TabIndex = 3;
            this.findPaperMarksBtn.Text = "Find Paper Marks";
            this.findPaperMarksBtn.UseVisualStyleBackColor = true;
            this.findPaperMarksBtn.Click += new System.EventHandler(this.findPaperMarksBtn_Click);
            // 
            // RectSize
            // 
            this.RectSize.Location = new System.Drawing.Point(708, 15);
            this.RectSize.Name = "RectSize";
            this.RectSize.Size = new System.Drawing.Size(50, 20);
            this.RectSize.TabIndex = 4;
            // 
            // RectMaxSize
            // 
            this.RectMaxSize.Location = new System.Drawing.Point(708, 43);
            this.RectMaxSize.Name = "RectMaxSize";
            this.RectMaxSize.Size = new System.Drawing.Size(50, 20);
            this.RectMaxSize.TabIndex = 5;
            // 
            // deSkewBtn
            // 
            this.deSkewBtn.Location = new System.Drawing.Point(525, 73);
            this.deSkewBtn.Name = "deSkewBtn";
            this.deSkewBtn.Size = new System.Drawing.Size(177, 23);
            this.deSkewBtn.TabIndex = 6;
            this.deSkewBtn.Text = "Deskew";
            this.deSkewBtn.UseVisualStyleBackColor = true;
            this.deSkewBtn.Click += new System.EventHandler(this.deSkewBtn_Click);
            // 
            // startDetectBtn
            // 
            this.startDetectBtn.Location = new System.Drawing.Point(525, 102);
            this.startDetectBtn.Name = "startDetectBtn";
            this.startDetectBtn.Size = new System.Drawing.Size(177, 23);
            this.startDetectBtn.TabIndex = 7;
            this.startDetectBtn.Text = "Start Detection";
            this.startDetectBtn.UseVisualStyleBackColor = true;
            this.startDetectBtn.Click += new System.EventHandler(this.startDetectBtn_Click);
            // 
            // startDetectionBtn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 553);
            this.Controls.Add(this.startDetectBtn);
            this.Controls.Add(this.deSkewBtn);
            this.Controls.Add(this.RectMaxSize);
            this.Controls.Add(this.RectSize);
            this.Controls.Add(this.findPaperMarksBtn);
            this.Controls.Add(this.imageBox1);
            this.Controls.Add(this.loadImgBtn);
            this.Name = "startDetectionBtn";
            this.Text = "CVTest";
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button loadImgBtn;
        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button findPaperMarksBtn;
        private System.Windows.Forms.TextBox RectSize;
        private System.Windows.Forms.TextBox RectMaxSize;
        private System.Windows.Forms.Button deSkewBtn;
        private System.Windows.Forms.Button startDetectBtn;
    }
}

