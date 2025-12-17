using ProdInfoSys.Models.ErpDataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models.StatusReportModels
{
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
