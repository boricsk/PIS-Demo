using Pojector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a comprehensive status report containing production, planning, and operational data for a specific
    /// reporting period.
    /// </summary>
    /// <remarks>The StatusReport class aggregates information such as issue date, report name, workday
    /// counts, machine data, production plans, turnover, shipout plans, daily plans, production completion ratios, and
    /// stop times. It is typically used to encapsulate all relevant data for generating or analyzing a production
    /// status report.</remarks>
    public class StatusReport
    {
        public DateOnly IssueDate {get;set;}
        public string ReportName {get;set;}
        public int WokdaysNum {get;set;}
        public List<StatusReportMachineList> MachineData {get;set;}        
        public StatusReportPlanData PlansData {get;set;}
        public List<Turnover> Turnover {get;set;}
        public List<ShipoutPlan> ShipoutPlan {get;set;}
        public int ActualWorkday { get; set; }
        public List<DailyPlan> KftDailyPlans { get; set; } = new List<DailyPlan>();
        public List<DailyPlan> RepackDailyPlans { get; set; } = new List<DailyPlan>();
        public string KftProdCompleteRatio { get; set; }
        public string RepackProdCompleteRatio { get; set; }
        public string KftProdTimePropRatio { get; set; }
        public string RepackProdTimePropRatio { get; set; }
        public List<StopTime> StopTimes { get; set; }
    }
}
