using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using System.Runtime.InteropServices;

namespace CVTest2
{
    public partial class frmMain : Form
    {
        public Mat colorImage, image, thresholded, wthreshold; // thresholded: threshold & inverted, wthreshold: not inverted
        public Emgu.CV.Image<Emgu.CV.Structure.Bgr, Byte> convertedWThreshold; // Experimental
        // Bitmap thresholdBitmap;
        Point[] pageMarks = new Point[4];
        Emgu.CV.Structure.MCvScalar redColor = new Emgu.CV.Structure.MCvScalar(0, 0, 255);

        public frmMain()
        {
            InitializeComponent();
        }

        private void loadImgBtn_Click(object sender, EventArgs e)
        {
            colorImage = CvInvoke.Imread("C:\\Users\\hoang\\My Stuffs\\Miscellaneous\\Filled Form trac nghiem.jpg");
            image = CvInvoke.Imread("C:\\Users\\hoang\\My Stuffs\\Miscellaneous\\Filled Form trac nghiem.jpg", Emgu.CV.CvEnum.ImreadModes.Grayscale);
            CvInvoke.Resize(colorImage, colorImage, new Size(2159, 3060));
            CvInvoke.Resize(image, image, new Size(2159, 3060));
            thresholded = new Mat();
            CvInvoke.Threshold(image, thresholded, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            wthreshold = thresholded.Clone();
            CvInvoke.BitwiseNot(thresholded, thresholded);

            imageBox1.Image = colorImage;
        }

        private void startDetectBtn_Click(object sender, EventArgs e)
        {
            DetectionFields fields = new DetectionFields();
            DetectionField f = new DetectionField();
            f.TopLeft = new Point(212, 359);
            f.Size = new Size(233, 531);
            f.NumOfRows = 10;
            f.NumOfCols = 5;
            f.Ident = "SoBaoDanh";
            fields.addField(f);
            // drawDetectionField(f);

            List<DataHolder> data = new List<DataHolder>();

            for (int i = 0; i < f.NumOfRows; i++)
            {
                for (int j = 0; j < f.NumOfCols; j++)
                {
                    DataHolder dh = new DataHolder();
                    dh.Col = j;
                    dh.Row = i;
                    dh.Ident = f.Ident;
                    dh.Score = recognizeCell(f.TopLeft.X + f.Size.Width * j / f.NumOfCols, f.TopLeft.X + f.Size.Width * (j + 1) / f.NumOfCols, f.TopLeft.Y + f.Size.Height * i / f.NumOfRows, f.TopLeft.Y + f.Size.Height * (i + 1) / f.NumOfRows);
                    data.Add(dh);
                }
            }
            Console.WriteLine("Detection of field completed!");
            imageBox1.Image = colorImage;
        }

        private float recognizeCell(int Xi, int Xf, int Yi, int Yf)
        {
            CvInvoke.Rectangle(colorImage, new Rectangle(Xi,Yi,Xf-Xi,Yf-Yi), redColor);
            float blackPix = 0;
            for (int i = Xi; i<=Xf; i++)
            {
                for (int j = Yi; j<=Yf; j++)
                {
                    // if (thresholdBitmap.GetPixel(i,j).R == 0)
                    if (convertedWThreshold[j,i].Blue == 0)
                    {
                        blackPix = blackPix + 1.0f;
                    }
                }
            }
            return blackPix;
        }

        private void drawDetectionField(DetectionField f)
        {
            CvInvoke.Rectangle(colorImage, new Rectangle(f.TopLeft, f.Size), redColor);
            for (int i=1; i < f.NumOfCols; i++)
            {
                CvInvoke.Line(colorImage, new Point(f.TopLeft.X + f.Size.Width * i / f.NumOfCols, f.TopLeft.Y), new Point(f.TopLeft.X + f.Size.Width * i / f.NumOfCols, f.TopLeft.Y+f.Size.Height), redColor);
            }
            for (int i = 1; i < f.NumOfRows; i++)
            {
                CvInvoke.Line(colorImage, new Point(f.TopLeft.X, f.TopLeft.Y + f.Size.Height * i / f.NumOfRows), new Point(f.TopLeft.X+f.Size.Width, f.TopLeft.Y + f.Size.Height * i / f.NumOfRows), redColor);
            }
        }

        private void findPaperMarksBtn_Click(object sender, EventArgs e)
        {
            double maxArea = 2500;
            double minArea = 2200;
            double mostLeftOffset = 0.25;
            double mostRightOffset = 0.75;
            double mostTopOffset = 0.25;
            double mostBottomOffset = 0.75;


            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(thresholded, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple, new Point(0,0));

            for (int i=0; i<contours.Size; i++)
            {
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                double k = (rect.Height + 0.0) / rect.Width;

                if (0.95 < k && k < 1.05 && rect.Height*rect.Width > minArea && rect.Height*rect.Width < maxArea)
                {
                    // Console.WriteLine("Found contour! "+rect.X+" "+rect.Y);
                    CvInvoke.DrawContours(colorImage, contours, i, redColor);

                    if(rect.X < image.Width*mostLeftOffset && rect.Y < image.Height*mostTopOffset)
                    {
                        // Top-left mark
                        pageMarks[0] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found top-left mark at "+rect.X+" "+rect.Y);
                    } else if (rect.X>image.Width*mostRightOffset && rect.Y < image.Height*mostTopOffset)
                    {
                        // Top-right mark
                        pageMarks[1] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found top-right mark at " + rect.X + " " + rect.Y);
                    } else if (rect.X < image.Width * mostLeftOffset && rect.Y > image.Height * mostBottomOffset)
                    {
                        // Bottom-left mark
                        pageMarks[2] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found bottom-left mark at " + rect.X + " " + rect.Y);
                    } else if (rect.X > image.Width * mostRightOffset && rect.Y > image.Height * mostBottomOffset)
                    {
                        // Bottom right mark
                        pageMarks[3] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found bottom-right mark at " + rect.X + " " + rect.Y);
                    }
                }

            }

            imageBox1.Image = colorImage;
        }

        private void deSkewBtn_Click(object sender, EventArgs e)
        {
            double phi1 = Math.Atan((pageMarks[1].Y - pageMarks[0].Y) / (pageMarks[1].X - pageMarks[0].X));
            Console.WriteLine("Deskew angle phi: "+phi1);
            double phi2 = Math.Atan((pageMarks[2].Y - pageMarks[3].Y) / (pageMarks[2].X - pageMarks[3].X));
            double phi = (phi1 + phi2) / 2;
            RotationMatrix2D rotMat = new RotationMatrix2D();
            CvInvoke.GetRotationMatrix2D(pageMarks[0], phi, 1, rotMat);
            CvInvoke.WarpAffine(thresholded, thresholded, rotMat, thresholded.Size);
            CvInvoke.WarpAffine(image, image, rotMat, thresholded.Size);
            CvInvoke.WarpAffine(colorImage, colorImage, rotMat, thresholded.Size);
            CvInvoke.WarpAffine(wthreshold, wthreshold, rotMat, thresholded.Size);
            cropImage();
        }

        private void cropImage()
        {
            colorImage = new Mat(colorImage, new Rectangle(pageMarks[0].X, pageMarks[0].Y, pageMarks[1].X - pageMarks[0].X, pageMarks[2].Y - pageMarks[0].Y));
            thresholded = new Mat(thresholded, new Rectangle(pageMarks[0].X, pageMarks[0].Y, pageMarks[1].X - pageMarks[0].X, pageMarks[2].Y - pageMarks[0].Y));
            image = new Mat(image, new Rectangle(pageMarks[0].X, pageMarks[0].Y, pageMarks[1].X - pageMarks[0].X, pageMarks[2].Y - pageMarks[0].Y));
            wthreshold = new Mat(wthreshold, new Rectangle(pageMarks[0].X, pageMarks[0].Y, pageMarks[1].X - pageMarks[0].X, pageMarks[2].Y - pageMarks[0].Y));
            convertedWThreshold = wthreshold.ToImage<Emgu.CV.Structure.Bgr, Byte>();
            // thresholdBitmap = wthreshold.Bitmap;
            imageBox1.Image = convertedWThreshold;
        }

        private void resizeToNew()
        {
            CvInvoke.Resize(colorImage, colorImage, new Size(2159, 3060));
            CvInvoke.Resize(image, image, new Size(2159, 3060));
            CvInvoke.Resize(thresholded, thresholded, new Size(2159, 3060));
            CvInvoke.Resize(wthreshold, wthreshold, new Size(2159, 3060));
            CvInvoke.Imwrite("cv_test.jpg", wthreshold);
        }
    }
}
