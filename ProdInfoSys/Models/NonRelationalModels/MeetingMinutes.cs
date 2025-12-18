using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProdInfoSys.Models.NonRelationalModels
{
    /// <summary>
    /// Represents the minutes of a meeting, including details such as the date, assigned tasks, responsible person,
    /// deadlines, comments, and status.
    /// </summary>
    /// <remarks>This class is typically used to store and retrieve meeting minutes information from a data
    /// store, such as a MongoDB collection. Each instance corresponds to a single meeting record and includes
    /// properties for tracking action items and their progress.</remarks>
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
