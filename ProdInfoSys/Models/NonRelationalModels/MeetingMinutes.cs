using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProdInfoSys.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models.NonRelationalModels
{
    public class MeetingMinutes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("date")]
        public DateTime date { get; set; }

        [BsonElement("task")]
        public string task { get; set; }

        [BsonElement("pic")]
        public string pic { get; set; }

        [BsonElement("deadline")]
        public DateTime deadline { get; set; }

        [BsonElement("comment")]
        public string comment { get; set; }

        [BsonElement("status")]
        public string status { get; set; }
    }
}
