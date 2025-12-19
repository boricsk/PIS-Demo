using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a manual containing information about a work center and its associated follow-up documents.
    /// </summary>
    public class Manual
    {
        public string Workcenter { get; set; }
        public List<ManualFollowupDocument> MaualFollowupDocuments { get; set; }
    }
}
