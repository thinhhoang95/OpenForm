﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenForm.Engine
{
    public class DetectionThread
    {
        Detection.DetectionTemplate detectionTemplate;
        int threshold;
        string[] FileNames;
        public delegate void DetectionCallback(string filename, string result);
        public delegate void PresentationCallback(Result.ResultPresenter presenter);
        DetectionCallback dCall;
        PresentationCallback pCall;
        bool saveRecognitionResult;

        public DetectionThread(Detection.DetectionTemplate dtpl, int threshold, string[] FileNames, DetectionCallback caller, PresentationCallback pCall, bool saveRecognitionResult)
        {
            this.detectionTemplate = dtpl;
            this.threshold = threshold;
            this.FileNames = FileNames;
            this.dCall = caller;
            this.pCall = pCall;
            this.saveRecognitionResult = saveRecognitionResult;
        }

        public void startDetectionProcess()
        {
            Result.ResultPresenter presenter = new Result.ResultPresenter();
            foreach (string file in FileNames)
            {
                Result.ResultManager resMan = new Result.ResultManager(threshold);
                try
                {
                    Template.Preprocessor preprocessor = new Template.Preprocessor(new Template.Preprocessor.PageMarksConfig() { MaxArea = detectionTemplate.MaxMarkArea, MinArea = detectionTemplate.MinMarkArea, MostBottomOffset = detectionTemplate.SearchBottomOffset, MostLeftOffset = detectionTemplate.SearchLeftOffset, MostRightOffset = detectionTemplate.SearchRightOffset, MostTopOffset = detectionTemplate.SearchTopOffset, RatioLowerBound = detectionTemplate.RatioLBound, RatioUpperBound = detectionTemplate.RatioUBound });
                    preprocessor.beginProcess(file, true);
                    preprocessor.saveProcessedImage();
                    Template.Detector detector = new Template.Detector(detectionTemplate, preprocessor.originalThresholded, resMan, presenter);
                    // Allow saving recognition results along with original image file
                    if(saveRecognitionResult)
                    {
                        detector.saveRecognitionResult = true;
                        detector.thresholdValue = threshold;
                        detector.destinationPath = Path.GetDirectoryName(file)+"\\_"+Path.GetFileName(file);
                    }
                    detector.run();
                    detector.Dispose();
                    preprocessor.finalise();
                    // Call back successful
                    dCall?.Invoke(file, "SUCCESSFUL");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: ");
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(System.Environment.StackTrace);
                    // Call back failed
                    dCall?.Invoke(file, "FAILED: " + ex.ToString());
                }
            }

            // Write to Excel file
            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(presenter.table);
            wb.SaveAs("form.xlsx");

            pCall?.Invoke(presenter);
        }
    }
}
