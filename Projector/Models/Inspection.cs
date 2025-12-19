using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents an inspection record, including the associated work center and any follow-up documents.
    /// </summary>
    public class Inspection
    {
        public string Workcenter { get; set; }
        public List<InspectionFollowupDocument> InspectionFollowupDocuments { get; set; }
    }
}
