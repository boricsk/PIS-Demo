using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class Manual
    {
        public string Workcenter { get; set; }
        public List<ManualFollowupDocument> MaualFollowupDocuments { get; set; }
    }
}
