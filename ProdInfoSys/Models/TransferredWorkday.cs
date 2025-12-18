namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a workday that has been transferred from one date to another.
    /// </summary>
    public class TransferredWorkday
    {
        public DateOnly FromDay { get; set; }
        public DateOnly ToDay { get; set; }
    }
}
