namespace ProdInfoSys.Models.ErpDataModels
{
    /// <summary>
    /// Represents a sales turnover record, including pricing, customer, and invoice details for a single transaction
    /// line.
    /// </summary>
    /// <remarks>The Turnover class encapsulates information related to a single line item in a sales
    /// transaction, such as product details, pricing, tax, customer references, and posting dates. It is typically used
    /// for reporting, analysis, or integration scenarios where detailed turnover data is required.</remarks>
    public class Turnover
    {

        public decimal LineTTLDcPrice => DCPrice * Quantity;

        public string DocumentNo { get; set; }
        public int LineNum { get; set; }
        public string SellToCustomerNo { get; set; }
        public string Type { get; set; }
        public string No { get; set; }
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VAT { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountIncludingVAT { get; set; }
        public string ShortcutDimension1Code { get; set; }
        public string ShortcutDimension2Code { get; set; }
        public string BillToCustomerNo { get; set; }
        public DateTime PostingDate { get; set; }
        public string OrderNo { get; set; }
        public string CurrencyCode { get; set; }
        public decimal SalesInvoiceCurrencyFactor { get; set; }
        public DateTime SalesInvoicePostingDate { get; set; }
        public DateTime? SalesInvoiceShipmentDate { get; set; }
        public string SalesInvoiceShipmentMethod { get; set; }
        public DateTime? SalesInvoiceDocumentDate { get; set; }
        public string SalesInvoiceExtDocNo { get; set; }
        public string ShipToName { get; set; }
        public string ShipToCity { get; set; }
        public decimal AmountEUR { get; set; }
        public decimal BookedCostStd { get; set; }
        public decimal CostAmountEUR { get; set; }
        public string YearMonth { get; set; }
        public int Year { get; set; }
        public string ProductCategory { get; set; }
        public decimal DCPrice { get; set; }
    }
}
