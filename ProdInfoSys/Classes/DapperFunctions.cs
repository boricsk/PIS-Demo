using Dapper;
using Microsoft.Data.SqlClient;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.StatusReportModels;
using ProdInfoSys.ViewModels;
using ProdInfoSys.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;


namespace ProdInfoSys.Classes
{
    public class DapperFunctions
    {
        public DapperFunctions() { }

        public ObservableCollection<Turnover> GetTurnover(string yearMonth)
        {
            ObservableCollection<Turnover> ret = new ObservableCollection<Turnover>();
            try
            {
                using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
                {
                    connection.Open();
                    ret = new ObservableCollection<Turnover>(connection
                        .Query<Turnover>(SqlQueries.QueryTurnover(yearMonth))
                        .ToList());

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt : {ex.Message}", "LoadDataWithDapper", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return ret;
        }
        public ObservableCollection<ItemDcCost> GetItemDc()
        {
            ObservableCollection<ItemDcCost> ret = new ObservableCollection<ItemDcCost>();
            try
            {
                using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
                {
                    connection.Open();
                    ret = new ObservableCollection<ItemDcCost>(connection
                        .Query<ItemDcCost>(SqlQueries.QueryStdCost())
                        .ToList());

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt : {ex.Message}", "LoadDataWithDapper", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ret;
        }

        public ObservableCollection<PlanningMasterData> GetPlanningMasterData(string planName)
        {
            //SqlQueries _sqlQueryForPlan = new SqlQueries(planName: planName);
            ObservableCollection<PlanningMasterData> ret = new ObservableCollection<PlanningMasterData>();
            try
            {
                using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
                {
                    connection.Open();
                    ret = new ObservableCollection<PlanningMasterData>(connection
                        .Query<PlanningMasterData>(SqlQueries.QueryProductionPlan(planName))
                        .ToList());

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt : {ex.Message}", "LoadDataWithDapper", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ret;
        }

        public ObservableCollection<ErpProdPlan> GetProductionPlan()
        {
            ObservableCollection<ErpProdPlan> ret = new ObservableCollection<ErpProdPlan>();
            try
            {
                using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
                {
                    ret = new ObservableCollection<ErpProdPlan>(connection
                    .Query<ErpProdPlan>(SqlQueries.QueryAllProductionPlan())
                    .OrderByDescending(p => p.Plan_Name)
                    .ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt : {ex.Message}", "LoadDataWithDapper", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ret;
        }
        public ObservableCollection<ErpMachineCenter> GetErpMachineCenters()
        {
            ObservableCollection<ErpMachineCenter> ret = new ObservableCollection<ErpMachineCenter>();
            try
            {
                using (var connection = new SqlConnection(SetupManagement.GetErpConString()))
                {
                    connection.Open();
                    ret = new ObservableCollection<ErpMachineCenter>(connection
                        .Query<ErpMachineCenter>(SqlQueries.QueryMachineCenters())
                        .OrderBy(m => m.MachineType)
                        .ToList());

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Hiba történt : {ex.Message}", "LoadDataWithDapper", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return ret;
        }
    }
}
