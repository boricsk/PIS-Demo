using ProdInfoSys.Models.FollowupDocuments;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a collection of machines associated with a specific work center.
    /// </summary>
    /// <remarks>This class provides properties to access the work center identifier and a list of follow-up
    /// documents  related to the machines. It is typically used to manage and organize machine-related data within a 
    /// manufacturing or operational context.</remarks>
    public class Machines
    {
        public string Workcenter { get; set; }
        public List<MachineFollowupDocument> MachineFollowupDocuments { get; set; }
    }
}
