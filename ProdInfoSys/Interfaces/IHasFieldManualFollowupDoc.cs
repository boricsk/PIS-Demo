using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Interfaces
{
    public interface IHasFieldManualFollowupDoc
    {
        public DateOnly Workday { get; set; }
    }
}
