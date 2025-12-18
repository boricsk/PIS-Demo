using LiveCharts;
using MongoDB.Bson.Serialization.Attributes;

namespace ProdInfoSys.Models.StatusReportModels
{
    /// <summary>
    /// Represents a summary of production and efficiency metrics for a specific work center, including cumulative
    /// plans, outputs, reject counts, and efficiency data.
    /// </summary>
    /// <remarks>This class is typically used to aggregate and report machine-level status information within
    /// a manufacturing or production environment. It provides both overall and detailed metrics, such as cumulative
    /// values and per-period data, to support reporting and analysis scenarios. Properties marked with [BsonIgnore] are
    /// excluded from MongoDB persistence and are intended for use in presentation or charting contexts.</remarks>
    public class StatusReportMachineList
    {
        public int Diff => ComulatedOut - ComulatedPlan;
        public string Workcenter { get; set; }
        public string WorkcenterType { get; set; }
        public int ComulatedPlan { get; set; }
        public int ComulatedOut { get; set; }
        public int Reject { get; set; }
        public int SupplierReject { get; set; }
        public decimal AvgEfficiency { get; set; }
        public decimal AvgEfficiencySubcon { get; set; }
        public double AvgRejectRatio { get; set; }
        public List<int> Output { get; set; }
        public List<int> ComulatedPlans { get; set; }
        public List<decimal> Efficiency { get; set; }
        public List<decimal> EfficiencySubcon { get; set; }

        [BsonIgnore]
        public SeriesCollection ChartData { get; set; }
        [BsonIgnore]
        public List<string> XAxisLabel { get; set; }

    }
}
