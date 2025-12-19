using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a QRQC (Quick Response Quality Control) status report, including production data, machine status,
    /// plan information, and performance ratios for a specific reporting period.
    /// </summary>
    /// <remarks>This class aggregates key metrics and details used in QRQC reporting, such as daily plans,
    /// machine data, and production completion ratios. It is typically used to transfer or display comprehensive status
    /// information for manufacturing or quality control processes.</remarks>
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
        public List<DailyPlan> KftDailyPlans { get; set; } = new List<DailyPlan>();
        public List<DailyPlan> RepackDailyPlans { get; set; } = new List<DailyPlan>();
        public List<StopTime> StopTimes { get; set; }
    }
}
