using LiveCharts;
using MongoDB.Bson.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a summary of production and efficiency metrics for a group of machines or work centers in a status
    /// report.
    /// </summary>
    /// <remarks>This class aggregates planned and actual output, efficiency, and reject statistics for
    /// reporting purposes. It is typically used to present or analyze production performance data for a specific work
    /// center or machine group. The properties include both cumulative totals and per-period series data to support
    /// detailed reporting and visualization scenarios.</remarks>
    public class StatusReportMachineList
    {
        public int Diff => ComulatedOut - ComulatedPlan;        
        public string Workcenter {get; set;}
        public string WorkcenterType {get; set;}
        public int ComulatedPlan {get; set;}
        public int ComulatedOut {get; set;}
        public int KftReject {get; set;}
        public int SupplierReject {get; set;}
        public decimal AvgEfficiency {get; set;}
        public decimal AvgEfficiencySubcon {get; set;}
        public double AvgRejectRatio {get; set;}
        public List<int> Output {get; set;}
        public List<int> ComulatedPlans {get; set;}
        public List<decimal> Efficiency {get; set;}
        public List<decimal> EfficiencySubcon {get; set;}

        [BsonIgnore]
        public SeriesCollection ChartData {get; set;}     

    }
}
