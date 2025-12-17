using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Interfaces
{
    public interface IHasFieldMachineFollowupDoc
    {
        public DateOnly Workday { get; set; }
        public double AvailOperatingHour { get; set; }        
    }
}
