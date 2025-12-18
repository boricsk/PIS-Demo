using ProdInfoSys.Models.ErpDataModels;
using System.Collections.ObjectModel;

namespace ProdInfoSys.Models.StatusReportModels
{
    /// <summary>
    /// Represents planning and cost data for a status report, including sales plans, material costs, and plan changes.
    /// </summary>
    /// <remarks>This class aggregates various planning metrics and cost components relevant to status
    /// reporting. It is typically used to encapsulate the results of planning calculations or to provide a data model
    /// for reporting interfaces.</remarks>
    public class StatusReportPlanData
    {
        public decimal TtlPlan => OutputPlanSales + Sample;

        public ObservableCollection<PlanningMasterData> PlanChanges { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal RepackMaterialCost { get; set; }
        public decimal Sample { get; set; }
        public decimal OutputPlanSales { get; set; }
        public decimal OutputPlanDc { get; set; }
        public decimal SalesPlan { get; set; }
        public decimal SalesPlanDC { get; set; }
        public decimal DCMovement { get; set; }
    }
}
