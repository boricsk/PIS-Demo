using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pojector.Models
{
    /// <summary>
    /// Represents a sales turnover record, including details such as document numbers, customer information, product
    /// data, pricing, amounts, and related financial and logistical fields.
    /// </summary>
    /// <remarks>This class is typically used to model individual sales line items or transactions for
    /// reporting, analysis, or integration with financial systems. It includes properties for both base and calculated
    /// values, such as amounts with and without VAT, currency conversion factors, and cost information. All properties
    /// are intended to be set and retrieved as part of the turnover data lifecycle.</remarks>
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
