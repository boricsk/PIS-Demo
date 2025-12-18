using ProdInfoSys.Models.ErpDataModels;

namespace ProdInfoSys.Models.StatusReportModels
{
    /// <summary>
    /// Represents a comprehensive production status report, including machine data, workday counts, production ratios,
    /// and related planning information for a specific reporting period.
    /// </summary>
    /// <remarks>The StatusReport class aggregates various metrics and planning data relevant to production
    /// and operational reporting. It is typically used to encapsulate the results of a reporting process for a given
    /// date or period, and may include details such as machine status, daily plans, turnover, and production completion
    /// ratios. All properties are intended to be populated as part of the report generation workflow.</remarks>
    public class StatusReport
    {
        public DateOnly IssueDate { get; set; }
        public string ReportName { get; set; }
        public int WokdaysNum { get; set; }
        public List<StatusReportMachineList> MachineData { get; set; }
        public StatusReportPlanData PlansData { get; set; }
        public List<Turnover> Turnover { get; set; }
        public List<ShipoutPlan> ShipoutPlan { get; set; }
        public int ActualWorkday { get; set; }
        public List<DailyPlan> DailyPlans { get; set; } = new List<DailyPlan>();
        public List<DailyPlan> RepackDailyPlans { get; set; } = new List<DailyPlan>();
        public string ProdCompleteRatio { get; set; }
        public string RepackProdCompleteRatio { get; set; }
        public string ProdTimePropRatio { get; set; }
        public string RepackProdTimePropRatio { get; set; }
        public List<StopTime> StopTimes { get; set; }
    }
}
