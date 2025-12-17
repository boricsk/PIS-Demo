using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models.NonRelationalModels
{
    public class TrWorkday
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("Workday from")]
        public List<TransferredWorkday> TransferredWorkdays { get; set; }
    }
}
