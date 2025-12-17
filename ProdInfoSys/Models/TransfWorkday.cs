using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models
{
    public class TransfWorkday
    {
        public DateTime FromDay { get; set; }

        public DateTime ToDay { get; set; }
    }
}
