using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.ObjectModel;

namespace ProdInfoSys.Models.NonRelationalModels
{
    /// <summary>
    /// Represents the configuration settings for a Production Information System (PIS) setup, including email lists and
    /// workcenter groupings.
    /// </summary>
    /// <remarks>This class is typically used to store and retrieve PIS setup data from a MongoDB database.
    /// Each property corresponds to a specific aspect of the PIS configuration, such as notification recipients and
    /// categorized workcenters.</remarks>
    public class PisSetup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("Email list")]
        public ObservableCollection<string> EmailList { get; set; }

        [BsonElement("Leader Email list")]
        public ObservableCollection<string> LeaderEmailList { get; set; }

        [BsonElement("Production meeting workcenters")]
        public ObservableCollection<string> ProdMeetingWorkcenters { get; set; }

        [BsonElement("Manual workcenters for avg")]
        public ObservableCollection<string> AvgManualWorkcenters { get; set; }

        [BsonElement("Projector workcenters")]
        public ObservableCollection<string> ProjectorWorkcenters { get; set; }

        [BsonElement("Actual followup")]
        public string ActualFollowup { get; set; }

    }
}
