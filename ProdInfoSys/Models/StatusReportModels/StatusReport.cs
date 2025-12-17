using ProdInfoSys.Models.ErpDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models.StatusReportModels
{
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
