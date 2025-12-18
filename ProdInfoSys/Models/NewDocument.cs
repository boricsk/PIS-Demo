using ProdInfoSys.Models.ErpDataModels;
using System.Collections.ObjectModel;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a production planning document containing scheduling, resource, and shift information for
    /// manufacturing operations.
    /// </summary>
    /// <remarks>Use this class to encapsulate all relevant data for a new production plan, including machine
    /// centers, shift details, headcount planning, and scheduling dates. The properties provide access to both
    /// high-level plan metadata and detailed scheduling parameters.</remarks>
    public class NewDocument
    {
        public ObservableCollection<ErpMachineCenter> MachineCenters { get; set; }
        public string PlanName { get; set; }
        public string DocumentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Workdays { get; set; }
        public decimal ManualShiftLength { get; set; }
        public decimal MachineShiftLength { get; set; }
        public int ManualShiftNumber { get; set; }
        public int MachineShiftNumber { get; set; }
        public double AbsenseRatio { get; set; }
        public int HeadcountPlanNet { get; set; }
        public int HeadcountPlanBr { get; set; }
        public string Description { get; set; }
        public List<DateOnly> WorkdayList { get; set; }

    }
}
