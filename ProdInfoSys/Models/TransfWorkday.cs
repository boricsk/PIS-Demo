namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a range of workdays with a specified start and end date.
    /// </summary>
    public class TransfWorkday
    {
        public DateTime FromDay { get; set; }

        public DateTime ToDay { get; set; }
    }
}
