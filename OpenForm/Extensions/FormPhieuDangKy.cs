using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Extensions
{
    public class FormPhieuDangKy
    {
        public Detection.DetectionTemplate template { get; }
        public FormPhieuDangKy()
        {
            template = new Detection.DetectionTemplate();

            template.MaxMarkArea = 9000;
            template.MinMarkArea = 3000;
            template.SearchTopOffset = 0.15;
            template.SearchBottomOffset = 0.85;
            template.SearchLeftOffset = 0.25;
            template.SearchRightOffset = 0.75;
            template.RatioUBound = 1.2;
            template.RatioLBound = 0.8;

            // Page 1, left column
            fieldWithMarginVertical(4, 901, 2842, 479, 110, 17, "LNgay", 10, 2);
            fieldWithMarginVertical(514, 901, 2842, 232, 106, 17, "LMonHoc", 5, 2);
            fieldWithMarginVertical(768, 901, 2842, 272, 106, 17, "LSoTietHoanThanh", 6, 2);

            // Page 1, right column
            fieldWithMarginVertical(1089, 901, 2842, 479, 110, 17, "RNgay", 10, 2);
            fieldWithMarginVertical(1590, 901, 2842, 232, 106, 17, "RMonHoc", 5, 2);
            fieldWithMarginVertical(1839, 901, 2842, 272, 106, 17, "RSoTietHoanThanh", 6, 2);

            // Identification number
            addIdentificationNumberField();

            // Save template
            saveTemplate();
        }

        private void fieldWithMarginVertical(int topX, int topY, int bottomY, int width, int height, int numOfFields, string identPrefix, int numOfCols, int numOfRows)
        {
            for (int i = 1; i <= numOfFields; i++)
            {
                Detection.DetectionField f = new Detection.DetectionField();
                f.FieldType = "QUESTION_ROW";
                f.Ident = identPrefix+"_"+i;
                f.NumOfCols = numOfCols;
                f.NumOfRows = numOfRows;
                f.ResultType = "NUMERIC";
                // First item @4, 893, Last item @4, 2842
                int topYCurrent = topY + (i-1) * (bottomY - topY) / (numOfFields-1);
                f.TopLeft = new System.Drawing.Point(topX, topYCurrent);
                f.Size = new System.Drawing.Size(width, height);
                template.addField(f);
            }
        }

        private void addIdentificationNumberField()
        {
            Detection.DetectionField f = new Detection.DetectionField();
            f.FieldType = "QUESTION_COL";
            f.Ident = "MSSV";
            f.NumOfCols = 8;
            f.NumOfRows = 10;
            f.ResultType = "NUMERIC";
            f.TopLeft = new System.Drawing.Point(1674, 266);
            f.Size = new System.Drawing.Size(425, 501);
            template.addField(f);
        }

        private void saveTemplate()
        {
            try
            {
                using (Stream stream = File.Open("phieudangky.ofr", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, template);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("An error has occurred while trying to save the file: " + e.ToString());
            }
        }
    }
}
