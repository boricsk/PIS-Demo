using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class DailyPlan
    {
        public decimal Diff => ActualQty - PlannedQty;
        public DateTime Workday {get; set;}
        public decimal PlannedQty {get; set;}
        public decimal ActualQty {get; set;}        
    }
}
