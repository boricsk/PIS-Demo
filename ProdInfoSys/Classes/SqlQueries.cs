using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Classes
{
    public static class SqlQueries
    {
        public static string QueryTurnover(string _yearmonth) =>        
        $@"
            SELECT [EntryNo]
                  ,[DocumentNo]
                  ,[LineNum]
                  ,[SellToCustomerNo]
                  ,[Type]
                  ,[No]
                  ,[LocationCode]
                  ,[Description]
                  ,[UnitOfMeasure]
                  ,[Quantity]
                  ,[UnitPrice]
                  ,[VAT]
                  ,[Amount]
                  ,[AmountIncludingVAT]
                  ,[Shortcut Dimension 1 Code]
                  ,[ShortcutDimension1Code]
                  ,[ShortcutDimension2Code]
                  ,[BillToCustomerNo]
                  ,[PostingDate]
                  ,[OrderNo]
                  ,[CurrencyCode]
                  ,[SalesInvoiceCurrencyFactor]
                  ,[SalesInvoicePostingDate]
                  ,[SalesInvoiceShipmentDate]
                  ,[SalesInvoiceShipmentMethod]
                  ,[SalesInvoiceDocumentDate]
                  ,[SalesInvoiceExtDocNo]
                  ,[ShipToName]
                  ,[ShipToCity]
                  ,[AmountEUR]
                  ,[BookedCostStd]
                  ,[CostAmountEUR]
                  ,[YearMonth]
                  ,[Year]
                  ,[ProductCategory]
                  ,[DCPrice]
              FROM [PIS].[dbo].[Turnover]
            where [Order No_] <>'' and YearMonth ='{_yearmonth}'
        ";
        
        public static string QueryMachineCenters() =>
        @"
            SELECT [Workcenter]
                  ,[Name]
                  ,[Area]
                  ,[DirectUnitCost]
                  ,[UnitCost]
                  ,[Capacity]
                  ,[Efficiency]
                  ,[Blocked]
                  ,[SetupTime]
                  ,[WaitTime]
                  ,[MoveTime]
                  ,[FixedScrapQty]
                  ,[Scrap]
                  ,[ParalellCaps]
                  ,[ShiftNum]
                  ,[MachineType]
                  ,[ShiftLength]
                  ,[IsPlanFixHc]
                  ,[FixHc]
                  ,[CostSheetUnitCost]
                  ,[ScrapRatio]
              FROM [PIS].[dbo].[Machines]            
            where [Blocked] = 0
            ";
        

        public static string QueryAllProductionPlan() =>
        @"
        SELECT  
                [Plan_Name]      
            FROM [PIS].[dbo].[ProdPlan]
        ";
        
        public static string QueryProductionPlan(string _planName) =>        
        @$"
        SELECT  
            SELECT [EntryNo]
                  ,[Plan_Name]
                  ,[Plan_Version]
                  ,[Plan_PlannedQty]
                  ,[Plan_FinishedQty]
                  ,[Plan_RemainingQty]
                  ,[isProductionDone]
                  ,[Plan_UnitPrice]
                  ,[Plan_PriceOfOriginal]
                  ,[Plan_PriceOfPlanned]
                  ,[Plan_PriceOfRemaining]
                  ,[Plan_SH_Unit]
                  ,[Plan_SHOriginal]
                  ,[Plan_SHPlan]
                  ,[Plan_SHRemaining]
                  ,[Plan_ETD]
                  ,[Plan_Comment]
                  ,[Plan_ItemFG]
                  ,[Plan_Area]
                  ,[Plan_SLSCode]
                  ,[Plan_CustomerName]
                  ,[Plan_FGDCPrice]
                  ,[Plan_FGDCforOrig]
                  ,[Plan_FGDCforPlanned]
                  ,[Plan_FGDCforRemain]
                  ,[Plan_DC-SalesDiff]
                  ,[Plan_RMCostPlanned]
                  ,[Plan_YearMonth]
                  ,[Plan_YearWeek]
                  ,[Plan_Currency]
                  ,[Plan_ExchangeRate]
                  ,[Plan_HCReq]
                  ,[Plan_isFinished]
                  ,[Plan_InspectionTime]
                  ,[Plan_MachineTime]
                  ,[Plan_ManualTime]
                  ,[Plan_IndItemAdded]
                  ,[Plan_StartPeriod]
                  ,[Plan_FinishPeriod]
                  ,[Plan_isSample]
              FROM [PIS].[dbo].[ProdPlan]
            Where [Plan_Name] = '{_planName}' and [Plan_Version] = 0
        ";
        
        public static string QueryStdCost() => @"SELECT [No_] as Item, [Standard Cost] as StdCost FROM [SEIBC20_LIVE].[dbo].[SEI Interconnect ÉLES-ne haszn$Item$437dbf0e-84ff-417a-965d-ed2bb9650972] where No_ like '07%'";

        //public static string QueryCapacityLedger(string _workcenter, string _documentDate) =>
        //    @$"
        //    SELECT 
        //          [No_] as Workcenter
        //          ,[Posting Date] as PostingDate
        //          ,[Quantity]
        //          ,[Setup Time] as SetupTime
        //          ,[Run Time] as RunTime
        //          ,[Stop Time] as StopTime
        //          ,[Output Quantity] as OutputQty
        //          ,[Scrap Quantity]	as ScrapQty  
        //          ,[Global Dimension 1 Code] as Division
        //          ,[Global Dimension 2 Code] as Division2	  
        //          ,[Routing No_] as Routing
        //          ,[Item No_] as Item
        //          ,[Unit of Measure Code] as UnitCode
        //          ,[Document Date] as DocumentDate
        //          ,[Stop Code] as StopCode
        //          ,[Scrap Code] as ScrapCode
        //          ,[Work Shift Code] as ShiftCode
        //      FROM [SEIBC20_LIVE].[dbo].[SEI Interconnect ÉLES-ne haszn$Capacity Ledger Entry$437dbf0e-84ff-417a-965d-ed2bb9650972]
        //      where [Document Date] = '{_documentDate.ToString()}'
        //      and No_ = '{_workcenter}'
        //    ";
    }
}
