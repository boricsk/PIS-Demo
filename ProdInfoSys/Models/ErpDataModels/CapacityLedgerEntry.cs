namespace ProdInfoSys.Models.ErpDataModels
{
    /// <summary>
    /// Represents a single entry in the capacity ledger, containing production and operational data for a specific work
    /// center and date.
    /// </summary>
    /// <remarks>This class is typically used to record and transfer information about work center activity,
    /// including times, quantities, and classification codes, for manufacturing or production reporting purposes. All
    /// properties are plain data fields and do not perform validation or formatting.</remarks>
    public class CapacityLedgerEntry
    {
        public string Workcenter { get; set; }
        public string PostingDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal SetupTime { get; set; }
        public decimal RunTime { get; set; }
        public decimal StopTime { get; set; }
        public decimal OutputQty { get; set; }
        public decimal ScrapQty { get; set; }
        public string Division { get; set; }
        public string Division2 { get; set; }
        public string Routing { get; set; }
        public string Item { get; set; }
        public string UnitCode { get; set; }
        public string DocumentDate { get; set; }
        public string StopCode { get; set; }
        public string ScrapCode { get; set; }
        public string ShiftCode { get; set; }
    }
}
