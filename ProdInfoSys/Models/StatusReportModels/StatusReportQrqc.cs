namespace ProdInfoSys.Models.StatusReportModels
{
    /// <summary>
    /// Represents a QRQC (Quick Response Quality Control) status report, including production data, machine status,
    /// plan information, and related metrics for a specific reporting period.
    /// </summary>
    /// <remarks>This class aggregates various data points relevant to a QRQC status report, such as
    /// production completion ratios, machine data, daily plans, and stop times. It is typically used to transfer or
    /// display comprehensive status information for manufacturing or quality control processes. All properties should
    /// be populated with valid data before use in reporting or analytics scenarios.</remarks>
    public class StatusReportQrqc
    {
        public DateOnly IssueDate { get; set; }
        public string ReportName { get; set; }
        public int WokdaysNum { get; set; }
        public List<StatusReportMachineList> MachineData { get; set; }
        public StatusReportPlanData PlansData { get; set; }
        public string KftProdCompleteRatio { get; set; }
        public string RepackProdCompleteRatio { get; set; }
        public string KftProdTimePropRatio { get; set; }
        public string RepackProdTimePropRatio { get; set; }
        public int ActualWorkday { get; set; }
        public List<DailyPlan> DailyPlans { get; set; } = new List<DailyPlan>();
        public List<DailyPlan> RepackDailyPlans { get; set; } = new List<DailyPlan>();
        public List<StopTime> StopTimes { get; set; }
    }
}
