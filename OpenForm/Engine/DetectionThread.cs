using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
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
        DetectionCallback dCall;

        public DetectionThread(Detection.DetectionTemplate dtpl, int threshold, string[] FileNames, DetectionCallback caller)
        {
            this.detectionTemplate = dtpl;
            this.threshold = threshold;
            this.FileNames = FileNames;
            this.dCall = caller;
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
                    preprocessor.beginProcess(file);
                    Template.Detector detector = new Template.Detector(detectionTemplate, preprocessor.originalThresholded, resMan, presenter);
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
        }
    }
}
