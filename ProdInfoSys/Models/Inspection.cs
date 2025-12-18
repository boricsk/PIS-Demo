using ProdInfoSys.Models.FollowupDocuments;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents an inspection, including its associated work center and follow-up documents.
    /// </summary>
    /// <remarks>This class provides a way to store and manage information about an inspection,  including the
    /// work center where the inspection is conducted and any related follow-up documents.</remarks>
    public class Inspection
    {
        public string Workcenter { get; set; }
        public List<InspectionFollowupDocument> InspectionFollowupDocuments { get; set; }
    }
}
