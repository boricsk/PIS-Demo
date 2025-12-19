using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents the configuration settings for a Production Information System (PIS), including email distribution
    /// lists and workcenter groupings.
    /// </summary>
    /// <remarks>This class is typically used to store and retrieve PIS setup data from a MongoDB database.
    /// The properties correspond to various configuration elements such as email lists for notifications and
    /// collections of workcenters used in production meetings, manual average calculations, and projector
    /// displays.</remarks>
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
