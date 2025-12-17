using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Interfaces
{
    public interface IHasFieldHCFollowupDoc
    {
        public DateOnly Workday { get; set; }
        public int ShiftNum { get; set; }
        public decimal ShiftLen { get; set; }
        
    }
}
