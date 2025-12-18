namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a daily production plan, including the planned and actual quantities for a specific workday.
    /// </summary>
    /// <remarks>This class provides properties to track the planned and actual quantities for a given
    /// workday,  as well as a calculated difference between the two.</remarks>
    public class DailyPlan
    {
        public decimal Diff => ActualQty - PlannedQty;
        public DateTime Workday { get; set; }
        public decimal PlannedQty { get; set; }
        public decimal ActualQty { get; set; }
    }
}
