using Dapper;
using Microsoft.Data.SqlClient;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.StatusReportModels;
using System.Collections.ObjectModel;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides methods for retrieving ERP-related data collections, such as turnover, item costs, planning master
    /// data, production plans, and machine centers, using Dapper for database access.
    /// </summary>
    /// <remarks>This class serves as a data access layer for common ERP queries, returning results as
    /// observable collections suitable for data binding in UI applications. All methods establish a new database
    /// connection for each call and return the requested data as an ObservableCollection. Thread safety is not
    /// guaranteed for instances of this class.</remarks>
    public class DapperFunctions
    {
        public DapperFunctions() { }

        /// <summary>
        /// Retrieves a collection of turnover records for the specified year and month.
        /// </summary>
        /// <param name="yearMonth">A string representing the year and month for which to retrieve turnover records, formatted as "yyyyMM" (for
        /// example, "202406" for June 2024).</param>
        /// <returns>An observable collection of <see cref="Turnover"/> objects corresponding to the specified year and month.
        /// The collection is empty if no records are found.</returns>
        public ObservableCollection<Turnover> GetTurnover(string yearMonth)
        {
            ObservableCollection<Turnover> ret = new ObservableCollection<Turnover>();

            using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
            {
                connection.Open();
                ret = new ObservableCollection<Turnover>(connection
                    .Query<Turnover>(SqlQueries.QueryTurnover(yearMonth))
                    .ToList());

                connection.Close();
            }
            return ret;
        }

        /// <summary>
        /// Retrieves a collection of item distribution center cost records from the ERP database.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{ItemDcCost}"/> containing the item distribution center cost records. The
        /// collection will be empty if no records are found.</returns>
        public ObservableCollection<ItemDcCost> GetItemDc()
        {
            ObservableCollection<ItemDcCost> ret = new ObservableCollection<ItemDcCost>();

            using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
            {
                connection.Open();
                ret = new ObservableCollection<ItemDcCost>(connection
                    .Query<ItemDcCost>(SqlQueries.QueryStdCost())
                    .ToList());

                connection.Close();
            }

            return ret;
        }

        /// <summary>
        /// Retrieves a collection of planning master data records associated with the specified plan name.
        /// </summary>
        /// <param name="planName">The name of the plan for which to retrieve planning master data. Cannot be null or empty.</param>
        /// <returns>An observable collection of <see cref="PlanningMasterData"/> objects that match the specified plan name. The
        /// collection is empty if no records are found.</returns>
        public ObservableCollection<PlanningMasterData> GetPlanningMasterData(string planName)
        {
            ObservableCollection<PlanningMasterData> ret = new ObservableCollection<PlanningMasterData>();
            using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
            {
                connection.Open();
                ret = new ObservableCollection<PlanningMasterData>(connection
                    .Query<PlanningMasterData>(SqlQueries.QueryProductionPlan(planName))
                    .ToList());

                connection.Close();
            }

            return ret;
        }

        /// <summary>
        /// Retrieves the complete production plan from the ERP system.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{ErpProdPlan}"/> containing all production plan entries, ordered by plan
        /// name in descending order. The collection will be empty if no production plans are found.</returns>
        public ObservableCollection<ErpProdPlan> GetProductionPlan()
        {
            ObservableCollection<ErpProdPlan> ret = new ObservableCollection<ErpProdPlan>();
            using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
            {
                ret = new ObservableCollection<ErpProdPlan>(connection
                .Query<ErpProdPlan>(SqlQueries.QueryAllProductionPlan())
                .OrderByDescending(p => p.Plan_Name)
                .ToList());
            }

            return ret;
        }

        /// <summary>
        /// Retrieves a collection of machine centers from the ERP system, ordered by machine type.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{ErpMachineCenter}"/> containing all machine centers retrieved from the
        /// ERP database. The collection is empty if no machine centers are found.</returns>
        public ObservableCollection<ErpMachineCenter> GetErpMachineCenters()
        {
            ObservableCollection<ErpMachineCenter> ret = new ObservableCollection<ErpMachineCenter>();
            using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
            {
                connection.Open();
                ret = new ObservableCollection<ErpMachineCenter>(connection
                    .Query<ErpMachineCenter>(SqlQueries.QueryMachineCenters())
                    .OrderBy(m => m.MachineType)
                    .ToList());

                connection.Close();
            }

            return ret;
        }
    }
}
