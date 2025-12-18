namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides predefined SQL query strings for retrieving data from various tables in the PIS database.
    /// </summary>
    /// <remarks>This static class contains commonly used SQL queries as string-returning methods for use in
    /// data access operations. The queries are intended for internal application use and may require parameterization
    /// to prevent SQL injection when used with user-supplied input. All queries are specific to the database schema and
    /// may need to be updated if the underlying tables or columns change.</remarks>
    public static class SqlQueries
    {
        /// <summary>
        /// Generates a SQL query string to retrieve turnover records for a specified year and month.
        /// </summary>
        /// <remarks>The returned query filters records where the [Order No_] field is not empty and the
        /// YearMonth column matches the provided value. The caller is responsible for executing the query and handling
        /// any database interactions.</remarks>
        /// <param name="_yearmonth">The year and month to filter turnover records by, formatted as a string (for example, "202406" for June
        /// 2024).</param>
        /// <returns>A SQL query string that selects turnover records from the database where the year and month match the
        /// specified value.</returns>
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

        /// <summary>
        /// Returns the SQL query string used to retrieve information about unblocked machine centers from the database.
        /// </summary>
        /// <remarks>The returned query includes columns such as Workcenter, Name, Area, cost and capacity
        /// metrics, and other machine center attributes. Only machine centers that are not blocked are included in the
        /// result set.</remarks>
        /// <returns>A SQL query string that selects details for all machine centers where the Blocked flag is set to 0.</returns>
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

        /// <summary>
        /// Returns the SQL query string used to retrieve all production plan names from the database.
        /// </summary>
        /// <returns>A SQL query string that selects the Plan_Name column from the ProdPlan table.</returns>
        public static string QueryAllProductionPlan() =>
        @"
        SELECT  
                [Plan_Name]      
            FROM [PIS].[dbo].[ProdPlan]
        ";

        /// <summary>
        /// Generates a SQL query string to retrieve production plan details for the specified plan name and version 0.
        /// </summary>
        /// <remarks>The returned SQL query is intended for use with the [PIS].[dbo].[ProdPlan] table. The
        /// query filters results to only include records where the plan name matches the specified value and the plan
        /// version is 0. Ensure that the input is properly sanitized to prevent SQL injection if used in a dynamic
        /// context.</remarks>
        /// <param name="_planName">The name of the production plan to query. This value is used to filter the results by plan name. Cannot be
        /// null.</param>
        /// <returns>A SQL query string that selects all columns for the specified production plan name with version 0.</returns>
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

        /// <summary>
        /// Returns the SQL query string used to retrieve item numbers and their standard costs from the SEIBC20_LIVE
        /// database.
        /// </summary>
        /// <returns>A SQL query string that selects item numbers and standard costs for items whose numbers start with '07'.</returns>
        public static string QueryStdCost() => @"SELECT [No_] as Item, [Standard Cost] as StdCost FROM [SEIBC20_LIVE].[dbo].[SEI Interconnect ÉLES-ne haszn$Item$437dbf0e-84ff-417a-965d-ed2bb9650972] where No_ like '07%'";

    }
}
