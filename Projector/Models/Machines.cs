using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a collection of machine follow-up documents associated with a specific work center.
    /// </summary>
    public class Machines
    {
        public string Workcenter {get; set;}
        public List<MachineFollowupDocument> MachineFollowupDocuments {get; set;}
    }
}
