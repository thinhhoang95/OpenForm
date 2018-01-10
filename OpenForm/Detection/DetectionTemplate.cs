using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenForm.Detection
{
    [Serializable()]
    public class DetectionTemplate
    {
        public List<DetectionField> Fields { get; }
        public int MinMarkArea { get; set; }
        public int MaxMarkArea { get; set; }
        public double SearchLeftOffset { get; set; }
        public double SearchRightOffset { get; set; }
        public double SearchTopOffset { get; set; }
        public double SearchBottomOffset { get; set; }
        public double RatioLBound { get; set; }
        public double RatioUBound { get; set; }

        public DetectionTemplate()
        {
            Fields = new List<DetectionField>();
        }
        public void addField(DetectionField f)
        {
            Fields.Add(f);
        }
        public void deleteField(int pos)
        {
            Fields.RemoveAt(pos);
        }
    }
}
