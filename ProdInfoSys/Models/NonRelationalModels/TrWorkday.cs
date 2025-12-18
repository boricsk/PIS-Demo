using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProdInfoSys.Models.NonRelationalModels
{
    /// <summary>
    /// Represents a workday record that includes an identifier and a collection of transferred workday entries.
    /// </summary>
    public class TrWorkday
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("Workday from")]
        public List<TransferredWorkday> TransferredWorkdays { get; set; }
    }
}
