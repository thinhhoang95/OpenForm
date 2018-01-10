using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Template
{
    class Preprocessor
    {
        public class PageMarksConfig
        {
            public int MaxArea { get; set; }
            public int MinArea { get; set; }
            public double RatioUpperBound { get; set; }
            public double RatioLowerBound { get; set; }
            public double MostLeftOffset { get; set; }
            public double MostRightOffset { get; set;}
            public double MostTopOffset { get; set; }
            public double MostBottomOffset { get; set; }
            public static PageMarksConfig getDefaultSampleConfig()
            {
                return new PageMarksConfig() { MaxArea = 2500, MinArea = 2200, RatioLowerBound = 0.95, RatioUpperBound = 1.05, MostLeftOffset = 0.2, MostRightOffset = 0.8, MostTopOffset = 0.2, MostBottomOffset = 0.8 };
            }
        }

        // Parameters for preprocessor
        public Mat OriginalImage { get; set; }
        private Mat thresholded; // Thresholded and inverted
        public Mat originalThresholded;  // Thresholded only, for displaying purposes
        private Point[] pageMarks = new Point[4];
        public PageMarksConfig pageMarksConfig;
        private Emgu.CV.Structure.MCvScalar redColor = new Emgu.CV.Structure.MCvScalar(0, 0, 255);
        Image<Emgu.CV.Structure.Bgr, Byte> originalThresholdedImg;
        Image<Emgu.CV.Structure.Bgr, Byte> convertedWThreshold;

        // Event reception
        public event GeneralEventHandlers.VoidHandler OnInvalidPageMarksDetected;
        public event GeneralEventHandlers.VoidHandler OnRequestDisplayUpdate;

        public Preprocessor(PageMarksConfig pmc)
        {
            this.pageMarksConfig = pmc;
        }
        
        public void beginProcess(string ImageURL)
        {
            loadImage(ImageURL);
            int count = detectPageMarks();
            if (count!=4)
            {
                if (OnInvalidPageMarksDetected != null)
                {
                    OnInvalidPageMarksDetected.Invoke();
                }
                throw new Exception("Invalid page marks detected");
            } else
            finalDeskewCropAndResize();
        }

        private void loadImage(string ImageURL)
        {
            OriginalImage = CvInvoke.Imread(ImageURL, Emgu.CV.CvEnum.ImreadModes.Grayscale);
            CvInvoke.Resize(OriginalImage, OriginalImage, new Size(2159, 3060));
            originalThresholded = new Mat();
            CvInvoke.Threshold(OriginalImage, originalThresholded, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            thresholded = originalThresholded.Clone();
            CvInvoke.BitwiseNot(thresholded, thresholded);
        }

        private int detectPageMarks()
        {
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            CvInvoke.FindContours(thresholded, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple, new Point(0, 0));
            convertedWThreshold = originalThresholded.ToImage<Emgu.CV.Structure.Bgr, Byte>();

            int pageMarksCount = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                double k = (rect.Height + 0.0) / rect.Width;

                if (pageMarksConfig.RatioLowerBound < k && k < pageMarksConfig.RatioUpperBound && rect.Height * rect.Width > pageMarksConfig.MinArea && rect.Height * rect.Width < pageMarksConfig.MaxArea)
                {
                    // Console.WriteLine("Found contour! "+rect.X+" "+rect.Y);
                    CvInvoke.DrawContours(originalThresholded, contours, i, redColor);



                    if (rect.X < thresholded.Width * pageMarksConfig.MostLeftOffset && rect.Y < thresholded.Height * pageMarksConfig.MostTopOffset && checkIfPageMarkIsFilled(rect))
                    {
                        // Top-left mark
                        pageMarks[0] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found top-left mark at " + rect.X + " " + rect.Y);
                        Console.WriteLine("Size is: " + rect.Size.Width + " x " + rect.Size.Height);
                        pageMarksCount++;
                    }
                    else if (rect.X > thresholded.Width * pageMarksConfig.MostRightOffset && rect.Y < thresholded.Height * pageMarksConfig.MostTopOffset && checkIfPageMarkIsFilled(rect))
                    {
                        // Top-right mark
                        pageMarks[1] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found top-right mark at " + rect.X + " " + rect.Y);
                        Console.WriteLine("Size is: " + rect.Size.Width + " x " + rect.Size.Height);
                        pageMarksCount++;
                    }
                    else if (rect.X < thresholded.Width * pageMarksConfig.MostLeftOffset && rect.Y > thresholded.Height * pageMarksConfig.MostBottomOffset && checkIfPageMarkIsFilled(rect))
                    {
                        // Bottom-left mark
                        pageMarks[2] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found bottom-left mark at " + rect.X + " " + rect.Y);
                        Console.WriteLine("Size is: " + rect.Size.Width + " x " + rect.Size.Height);
                        pageMarksCount++;
                    }
                    else if (rect.X > thresholded.Width * pageMarksConfig.MostRightOffset && rect.Y > thresholded.Height * pageMarksConfig.MostBottomOffset && checkIfPageMarkIsFilled(rect))
                    {
                        // Bottom right mark
                        pageMarks[3] = new Point(rect.X, rect.Y);
                        Console.WriteLine("Found bottom-right mark at " + rect.X + " " + rect.Y);
                        Console.WriteLine("Size is: " + rect.Size.Width + " x " + rect.Size.Height);
                        pageMarksCount++;
                    }
                }
            }
            // CvInvoke.Imwrite("test_run.jpg",thresholded);
            convertedWThreshold.Dispose();
            return pageMarksCount;
        }

        private Boolean checkIfPageMarkIsFilled(Rectangle r)
        {
            return recognizeCell(r.X, r.X + r.Size.Width, r.Y, r.Y + r.Size.Height) > 0.9 * r.Size.Width * r.Size.Height;
        }

        private float recognizeCell(int Xi, int Xf, int Yi, int Yf)
        {
            // Console.WriteLine("Recognizing cell " + Xi + " " + Yi + " " + " (" + (Xf - Xi) + " " + (Yf - Yi) + ")");
            // CvInvoke.Rectangle(colorImage, new Rectangle(Xi, Yi, Xf - Xi, Yf - Yi), redColor);
            float blackPix = 0;
            for (int i = Xi; i <= Xf; i++)
            {
                for (int j = Yi; j <= Yf; j++)
                {
                    // if (thresholdBitmap.GetPixel(i,j).R == 0)
                    if (convertedWThreshold[j, i].Blue == 0)
                    {
                        blackPix = blackPix + 1.0f;
                    }
                }
            }
            return blackPix;
        }

        private void finalDeskewCropAndResize()
        {
            // Deskewing
            double phi1 = Math.Atan((pageMarks[1].Y - pageMarks[0].Y) / (pageMarks[1].X - pageMarks[0].X));
            Console.WriteLine("Deskew angle phi: " + phi1);
            double phi2 = Math.Atan((pageMarks[2].Y - pageMarks[3].Y) / (pageMarks[2].X - pageMarks[3].X));
            double phi = (phi1 + phi2) / 2;
            RotationMatrix2D rotMat = new RotationMatrix2D();
            CvInvoke.GetRotationMatrix2D(pageMarks[0], phi, 1, rotMat);
            CvInvoke.WarpAffine(thresholded, thresholded, rotMat, thresholded.Size);
            CvInvoke.WarpAffine(originalThresholded, originalThresholded, rotMat, thresholded.Size);

            // Cropping
            originalThresholded = new Mat(originalThresholded, new Rectangle(pageMarks[0].X, pageMarks[0].Y, pageMarks[1].X - pageMarks[0].X, pageMarks[2].Y - pageMarks[0].Y));
            thresholded = new Mat(thresholded, new Rectangle(pageMarks[0].X, pageMarks[0].Y, pageMarks[1].X - pageMarks[0].X, pageMarks[2].Y - pageMarks[0].Y));
            

            // Final resizing
            CvInvoke.Resize(originalThresholded, originalThresholded, new Size(2159, 3060));
            CvInvoke.Resize(thresholded, thresholded, new Size(2159, 3060));

            originalThresholdedImg = originalThresholded.ToImage<Emgu.CV.Structure.Bgr, Byte>();
            if (OnRequestDisplayUpdate != null) OnRequestDisplayUpdate.Invoke();
        }

        public void saveProcessedImage()
        {
            CvInvoke.Imwrite("cv_template.jpg", originalThresholded);
        }

        public void finalise()
        {
            try
            {
                originalThresholdedImg.Dispose();
                originalThresholded.Dispose();
                thresholded.Dispose();
                OriginalImage.Dispose();
                OnInvalidPageMarksDetected = null;
                OnRequestDisplayUpdate = null;
            } catch (Exception e)
            {

            }
        }
    }
}
