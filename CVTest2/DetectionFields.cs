using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVTest2
{
    class DetectionFields
    {
        List<DetectionField> detectionFields = new List<DetectionField>();
        public void addField(DetectionField d)
        {
            detectionFields.Add(d);
        }
        public List<DetectionField> getFields()
        {
            return detectionFields;
        }
    }
}
