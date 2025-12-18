using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.StatusReportModels;
using System.Collections.ObjectModel;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents metadata for follow-up processes, including production, quality, and planning data.
    /// </summary>
    /// <remarks>This class provides a comprehensive structure for managing and organizing various types of
    /// data  related to production status, quality reports, efficiency metrics, and planning details. It is  designed
    /// to support scenarios such as production monitoring, quality control, and planning analysis.</remarks>
    public class FollowupMetadata
    {
        public ObservableCollection<StatusReport>? AllStatusReportForProd { get; set; }
        public List<decimal> PlanOutputValues { get; set; }
        public List<decimal> RmCostValues { get; set; }
        public List<decimal> KftRmCostValues { get; set; }
        public List<decimal> RepackRmCostValues { get; set; }
        public List<decimal> SalesOutputValues { get; set; }
        public List<double> ManualRejectRatio { get; set; }
        public List<double> MachineRejectRatio { get; set; }
        public List<double> InspectionRejectRatio { get; set; }
        public ObservableCollection<StatusReportQrqc>? AllStatusReportForQrqc { get; set; }
        public StatusReport? SelectedStatusReportProd { get; set; }
        public StatusReportQrqc? SelectedStatusReportQrqc { get; set; }
        public ObservableCollection<StatusReportMachineList>? MachineStatusReports { get; set; }
        public ObservableCollection<StatusReportMachineList>? AffectedMachineStatusReportsForProdMeeting { get; set; }
        public ObservableCollection<StatusReportMachineList>? AffectedMachineStatusReportsForQrqcMeeting { get; set; }

        //A hatékonyság diagramhoz
        public ObservableCollection<decimal>? KftEfficiency { get; set; }
        public ObservableCollection<decimal>? SubconEfficiency { get; set; }


        public StatusReportPlanData? PlanningDataOfReport { get; set; }
        public ObservableCollection<PlanningMasterData>? PlanChangeDetails { get; set; }
        public ObservableCollection<ShipoutPlan>? ShipoutPlanDetails { get; set; }
        public ObservableCollection<StatusReportMachineList>? WorkcenterFollowupData { get; set; }
        public ObservableCollection<DailyPlan>? RepackDailyPlannedQty { get; set; }
        public ObservableCollection<DailyPlan>? KftDailyPlannedQty { get; set; }
        public ObservableCollection<StopTime>? StopTimes { get; set; }
        public string? PlannedSamplePrice { get; set; }
        public string? ProdPlanPrice { get; set; }
        public string? ProdPlanDcPrice { get; set; }
        public string? ProdPlanMaterialPrice { get; set; }
        public string? ProdPlanRepackMaterialPrice { get; set; }
        public string? ProdPlanKftMaterialPrice { get; set; }
        public string? ProdPlanTotalPrice { get; set; }
        public string? ShipoutPlanPrice { get; set; }
        public string? ShipoutPlanDcPrice { get; set; }
        public string? DcMovement { get; set; }
    }
}
