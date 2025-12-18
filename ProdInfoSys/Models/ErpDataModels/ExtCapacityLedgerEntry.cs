namespace ProdInfoSys.Models.ErpDataModels
{
    /// <summary>
    /// Represents an extended capacity ledger entry sourced from the ERP system.
    /// </summary>
    /// <remarks>
    /// This model captures operational, timing, quantity, costing, and dimension
    /// details for a single capacity ledger record used in reporting and analysis.
    /// </remarks>
    public class ExtCapacityLedgerEntry
    {
        public int EntryNo { get; set; }
        public string Workcenter { get; set; }
        public DateTime? PostingDate { get; set; }
        public string DocumentNo { get; set; }
        public string Description { get; set; }
        public string OperationNo { get; set; }
        public string WorkCenterNo { get; set; }
        public decimal TTLTime { get; set; }
        public decimal SetupTime { get; set; }
        public decimal RunTime { get; set; }
        public decimal StopTime { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public decimal OutputQuantity { get; set; }
        public decimal ScrapQuantity { get; set; }

        public string Division { get; set; }
        public string Division2 { get; set; }

        public string StartingTime { get; set; }
        public string EndingTime { get; set; }
        public string OpStartTime { get; set; }
        public string OpFinishTime { get; set; }
        public decimal OpDuraation { get; set; }

        public string RoutingNo { get; set; }
        public string Item { get; set; }
        public string UnitCode { get; set; }

        public DateTime DocumentDate { get; set; }
        public string ExternalDocumentNo { get; set; }
        public string StopCode { get; set; }
        public string ScrapCode { get; set; }
        public string ShiftCode { get; set; }

        public decimal DirectCost { get; set; }
        public decimal OverheadCost { get; set; }
        public decimal DirectCostACY { get; set; }
        public decimal OverheadCostACY { get; set; }

        public string Dimension3Code { get; set; }
        public string Dimension4Code { get; set; }
        public string Dimension5Code { get; set; }
        public string Dimension6Code { get; set; }
        public string Dimension7Code { get; set; }
        public string Dimension8Code { get; set; }

        public string EmplName { get; set; }
        public decimal ShValueRoutig { get; set; }
        public decimal StandardHour { get; set; }
        public decimal Perf { get; set; }

        public DateTime RqpStartDate { get; set; }
        public DateTime RqpEndDate { get; set; }
        public string RqpLocation { get; set; }
    }
}
