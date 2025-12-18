namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents information about a stop event, including the work center, stop code, and total stop time.
    /// </summary>
    public class StopTime
    {
        public string Workcenter { get; set; }
        public string StopCode { get; set; }
        public decimal TtlStopTime { get; set; }

    }
}
