using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class Inspection
    {
        public string Workcenter { get; set; }
        public List<InspectionFollowupDocument> InspectionFollowupDocuments { get; set; }
    }
}
