using ProdInfoSys.Models.FollowupDocuments;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a manual that includes information about a work center and its associated follow-up documents.
    /// </summary>
    public class Manual
    {
        public string Workcenter { get; set; }
        public List<ManualFollowupDocument> MaualFollowupDocuments { get; set; }
    }
}
