using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class PlanningMasterData
    {
        public string Plan_Name { get; set; }
        public int Plan_Version { get; set; }
        public decimal Plan_PlannedQty { get; set; }
        public decimal Plan_FinishedQty { get; set; }
        public decimal Plan_RemainingQty { get; set; }
        public byte isProductionDone { get; set; }
        public decimal Plan_UnitPrice { get; set; }
        public decimal Plan_PriceOfOriginal { get; set; }
        public decimal Plan_PriceOfPlanned { get; set; }
        public decimal Plan_PriceOfRemaining { get; set; }
        public decimal Plan_SH_Unit { get; set; }
        public decimal Plan_SHOriginal { get; set; }
        public decimal Plan_SHPlan { get; set; }
        public decimal Plan_SHRemaining { get; set; }
        public DateTime Plan_ETD { get; set; }
        public string Plan_Comment { get; set; }
        public string Plan_ItemFG { get; set; }
        public string Plan_Area { get; set; }
        public string Plan_SalesCode { get; set; }
        public string Plan_CustomerName { get; set; }
        public decimal Plan_FGDCPrice { get; set; }
        public decimal Plan_FGDCforOrig { get; set; }
        public decimal Plan_FGDCforPlanned { get; set; }
        public decimal Plan_FGDCforRemain { get; set; }

        [Column("Plan_DC-SalesDiff")] // mivel a név nem érvényes C# szimbólumként
        public decimal Plan_DCSalesDiff { get; set; }

        public decimal Plan_RMCostPlanned { get; set; }
        public string Plan_YearMonth { get; set; }
        public string Plan_YearWeek { get; set; }
        public string Plan_Currency { get; set; }
        public decimal Plan_ExchangeRate { get; set; }
        public decimal Plan_HCReq { get; set; }
        public byte Plan_isFinished { get; set; }
        public decimal Plan_InspectionTime { get; set; }
        public decimal Plan_MachineTime { get; set; }
        public decimal Plan_ManualTime { get; set; }
        public DateTime Plan_IndItemAdded { get; set; }
        public DateTime Plan_StartPeriod { get; set; }
        public DateTime Plan_FinishPeriod { get; set; }
        public byte Plan_isSample { get; set; }
    }
}
