using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class Machines
    {
        public string Workcenter {get; set;}
        public List<MachineFollowupDocument> MachineFollowupDocuments {get; set;}
    }
}
