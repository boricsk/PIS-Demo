using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models
{
    public class TransferredWorkday
    {
        public DateOnly FromDay { get; set; }
        public DateOnly ToDay { get; set; }
    }
}
