using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents planning and cost data for a status report, including sales plans, material costs, and plan changes.
    /// </summary>
    /// <remarks>This class aggregates various financial and planning metrics used in status reporting
    /// scenarios. It includes properties for tracking plan changes, material and repack costs, and different sales plan
    /// values. The data is intended to support reporting and analysis workflows.</remarks>
    public class StatusReportPlanData
    {
        public decimal TtlPlan => OutputPlanSales + Sample;

        public ObservableCollection<PlanningMasterData> PlanChanges { get; set; }
        public decimal RepackMaterialCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal Sample { get; set; }
        public decimal OutputPlanSales { get; set; }
        public decimal OutputPlanDc { get; set; }        
        public decimal SalesPlan { get; set; }
        public decimal SalesPlanDC { get; set; }
        public decimal DCMovement { get; set; }
    }
}
