using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Template
{
    class Detector : IDisposable
    {
        public Result.ResultManager resultManager { get; set; }
        public Detection.DetectionTemplate detectionTemplate { get; set; }
        public Result.ResultPresenter presenter { get; set; }
        private Image<Emgu.CV.Structure.Bgr, Byte> convertedWThreshold;

        
        // These parameters are for saving recognition result along with the original image file
        public bool saveRecognitionResult { get; set; }
        public int thresholdValue { get; set; }
        public string destinationPath { get; set; }


        public Detector(Detection.DetectionTemplate dtpl, Mat processingImage, Result.ResultManager resMan, Result.ResultPresenter presenter)
        {
            detectionTemplate = dtpl;
            convertedWThreshold = processingImage.ToImage<Emgu.CV.Structure.Bgr, Byte>();
            this.resultManager = resMan;
            this.presenter = presenter;
        }

        public void run()
        {
            presenter.addRow();
            foreach(Detection.DetectionField f in detectionTemplate.Fields)
            {
                // Recognize all cells if marked or not
                allCellsRecognize(f);
                // Begin interpreting recognition result into explanatory format
                switch (f.FieldType)
                {
                    case "QUESTION_ROW":
                        for(int i=0;i<f.NumOfRows;i++)
                        {
                            string colName = f.Ident + "_" + i;
                            presenter.addCol(colName);
                            string valToSet;
                            if (f.ResultType == "ALPHABETIC") valToSet = Result.ResultPresenter.toAlphabetic(resultManager.getRowAnswer(f.Ident, i));
                            else valToSet = resultManager.getRowAnswer(f.Ident, i).ToString();
                            presenter.setVal(colName, valToSet);
                        }
                        break;
                    case "QUESTION_COL":
                        for (int i = 0; i < f.NumOfCols; i++)
                        {
                            string colName = f.Ident + "_" + i;
                            presenter.addCol(colName);
                            string valToSet;
                            if (f.ResultType == "ALPHABETIC") valToSet = Result.ResultPresenter.toAlphabetic(resultManager.getColAnswer(f.Ident, i));
                            else valToSet = resultManager.getColAnswer(f.Ident, i).ToString();
                            presenter.setVal(colName, valToSet);
                        }
                        break;
                    case "CELL":
                        for (int i=0; i<f.NumOfRows;i++)
                        {
                            for (int k=0;k<f.NumOfCols;k++)
                            {
                                string colName = f.Ident + "_" + i + "_" + k;
                                presenter.addCol(colName);
                                string valToSet;
                                if (f.ResultType == "ALPHABETIC") valToSet = Result.ResultPresenter.toAlphabetic(resultManager.isFilled(f.Ident, i, k));
                                else valToSet = resultManager.isFilled(f.Ident, i, k).ToString();
                                presenter.setVal(colName, valToSet);
                            }
                        }
                        break;
                    case "COUNT":
                        for (int i = 0; i < f.NumOfRows; i++)
                        {
                            for (int k = 0; k < f.NumOfCols; k++)
                            {
                                string colName = f.Ident;
                                presenter.addCol(colName);
                                string valToSet;
                                valToSet = resultManager.getCount(colName).ToString();
                                presenter.setVal(colName, valToSet);
                            }
                        }
                        break;
                }
            }
            // Saving recognition result to file
            if (saveRecognitionResult) CvInvoke.Imwrite(destinationPath, convertedWThreshold);
        }

        private void allCellsRecognize(Detection.DetectionField f)
        {
            for (int i = 0; i < f.NumOfRows; i++)
            {
                for (int j = 0; j < f.NumOfCols; j++)
                {
                    Result.Response dh = new Result.Response();
                    dh.Col = j;
                    dh.Row = i;
                    dh.Ident = f.Ident;
                    dh.Score = (int)recognizeCell(f.TopLeft.X + f.Size.Width * j / f.NumOfCols, Math.Min(f.TopLeft.X + f.Size.Width * (j + 1) / f.NumOfCols, convertedWThreshold.Width - 1), f.TopLeft.Y + f.Size.Height * i / f.NumOfRows, Math.Min(f.TopLeft.Y + f.Size.Height * (i + 1) / f.NumOfRows, convertedWThreshold.Height - 1));

                    // Console.WriteLine("Cell " + dh.Ident + " @" + dh.Row + "/" + dh.Col + ": " + dh.Score);
                    resultManager.Result.Add(dh);
                }
            }
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
            // CAUTION! Please comment out these lines to prevent drawing on the picture
            if (saveRecognitionResult && blackPix > thresholdValue)
            {
                CvInvoke.Rectangle(convertedWThreshold, new System.Drawing.Rectangle(Xi, Yi, Xf - Xi, Yf - Yi), new Emgu.CV.Structure.MCvScalar(255, 0, 0));
            }
            return blackPix;
        }

        public void Dispose()
        {
            convertedWThreshold.Dispose();
        }
    }
}
