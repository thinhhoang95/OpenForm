using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Detection
{
    [Serializable()]
    public class DetectionField
    {
        public Point TopLeft { get; set; }
        public Size Size { get; set; }
        public int NumOfRows { get; set; }
        public int NumOfCols { get; set; }
        public string Ident { get; set; }
        public string FieldType { get; set; }
        public string ResultType { get; set; }
    }
}
