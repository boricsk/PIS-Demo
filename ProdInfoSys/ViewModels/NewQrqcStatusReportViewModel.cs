using ClosedXML.Excel;
using Dapper;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Enums;
using ProdInfoSys.Models;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.NonRelationalModels;
using ProdInfoSys.Models.StatusReportModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
//using MessageBox = Xceed.Wpf.Toolkit.MessageBox;


namespace ProdInfoSys.ViewModels
{
    public class NewQrqcStatusReportViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        #endregion

        private MasterFollowupDocument _followupDocument;
        private ObservableCollection<PlanningMasterData> _plan;
        private ObservableCollection<ExtCapacityLedgerEntry> _stopTimes = new ObservableCollection<ExtCapacityLedgerEntry>();
        private List<StopTime> grouppedStopTimes = new List<StopTime>();
        //private ObservableCollection<Turnover> _turnover;
        //private ObservableCollection<ItemDcCost> _itemDcCost;
        private StatusReportPlanData _planData = new();
        private List<string> _reportNames = new List<string>();
        private readonly string _sampleItem = "07-2000-0002H";
        private readonly string _area = "FFC";
        private decimal _actualTurnoverInSalesWithoutSample = 0;
        private decimal _actualTurnoverInDcWithoutSample = 0;
        private decimal _remainingTurnoverInSalesWithoutSample = 0;
        private decimal _remainingTurnoverInDcWithoutSample = 0;
        private decimal __samplePrice = 0;
        private decimal __planPrice = 0;
        private decimal __planDcPrice = 0;
        private decimal __materialCost = 0;
        private string _yearMonth = string.Empty;
        private int _wordayNumber = 0;
        private List<DailyPlan> _kftDailyPlannedQty = new List<DailyPlan>();
        private List<DailyPlan> _repackDailyPlannedQty = new List<DailyPlan>();

        #region PropChangedInterface
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ShipoutPlan> _shipoutPlan = new();
        public ObservableCollection<ShipoutPlan> ShipoutPlan
        {
            get => _shipoutPlan;
            set { _shipoutPlan = value; OnPropertyChanged(); }
        }

        private string? _mustSave;
        public string? MustSave
        {
            get => _mustSave;
            set { _mustSave = value; OnPropertyChanged(); }
        }

        private string? _reportName;
        public string? ReportName
        {
            get => _reportName;
            set { _reportName = value; OnPropertyChanged(); }
        }


        private string? _parentName;
        public string? ParentName
        {
            get => _parentName;
            set { _parentName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<StatusReportMachineList> _statusReports;
        public ObservableCollection<StatusReportMachineList> StatusReports
        {
            get => _statusReports;
            set { _statusReports = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PlanningMasterData> _planChanges;
        public ObservableCollection<PlanningMasterData> PlanChanges
        {
            get => _planChanges;
            set { _planChanges = value; OnPropertyChanged(); }
        }

        private string _samplePrice;
        public string SamplePrice
        {
            get => _samplePrice;
            set { _samplePrice = value; OnPropertyChanged(); }
        }

        private int _currentWorkday;
        public int CurrentWorkday
        {
            get => _currentWorkday;
            set { _currentWorkday = value; OnPropertyChanged(); }
        }


        private string _kftProdCompleteRatio;
        public string KftProdCompleteRatio
        {
            get => _kftProdCompleteRatio;
            set { _kftProdCompleteRatio = value; OnPropertyChanged(); }
        }


        private string _repackProdCompleteRatio;
        public string RepackProdCompleteRatio
        {
            get => _repackProdCompleteRatio;
            set { _repackProdCompleteRatio = value; OnPropertyChanged(); }
        }

        private string _repackTimePropRatio;
        public string RepackTimePropRatio
        {
            get => _repackTimePropRatio;
            set { _repackTimePropRatio = value; OnPropertyChanged(); }
        }

        private string _kftTimePropRatio;
        public string KftTimePropRatio
        {
            get => _kftTimePropRatio;
            set { _kftTimePropRatio = value; OnPropertyChanged(); }
        }

        private string _planPrice;
        public string PlanPrice
        {
            get => _planPrice;
            set { _planPrice = value; OnPropertyChanged(); }
        }

        private string _planDcPrice;
        public string PlanDcPrice
        {
            get => _planDcPrice;
            set { _planDcPrice = value; OnPropertyChanged(); }
        }

        private string _materialCost;
        public string MaterialCost
        {
            get => _materialCost;
            set { _materialCost = value; OnPropertyChanged(); }
        }
        private string _ttlPlan;
        public string TtlPlan
        {
            get => _ttlPlan;
            set { _ttlPlan = value; OnPropertyChanged(); }
        }

        private string _salesPlan;
        public string SalesPlan
        {
            get => _salesPlan;
            set { _salesPlan = value; OnPropertyChanged(); }
        }

        private string _dcMovement;
        public string DcMovement
        {
            get => _dcMovement;
            set { _dcMovement = value; OnPropertyChanged(); }
        }


        private string _salesPlanDc;
        public string SalesPlanDc
        {
            get => _salesPlanDc;
            set { _salesPlanDc = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand SaveReportData => new ProjectCommandRelay(_ => SavingReportData());
        private void SavingReportData()
        {
            if (!ReportName.IsNullOrEmpty())
            {
                if (!_reportNames.Contains(ReportName))
                {
                    (bool isCompleted, string message) = SaveDocumentToDatabase();

                    if (isCompleted)
                    {
                        //MessageBox.Show($"A mentés sikeres", "SaveReport", MessageBoxButton.OK, MessageBoxImage.Information);
                        _dialogs.ShowInfo($"A mentés sikeres", "SaveReport");
                        _reportNames.Add(ReportName);
                        MustSave = string.Empty;
                    }
                    else
                    {
                        //MessageBox.Show($"Hiba történt a mentés alatt!: {message}", "SaveReport", MessageBoxButton.OK, MessageBoxImage.Error);
                        _dialogs.ShowErrorInfo($"Hiba történt a mentés alatt!: {message}", "SaveReport");
                    }
                }
                else
                {
                    //MessageBox.Show($"A megadott név már létezik!", "SaveReport", MessageBoxButton.OK, MessageBoxImage.Error);
                    _dialogs.ShowErrorInfo($"A megadott név már létezik!", "SaveReport");
                }
            }
            else
            {
                //MessageBox.Show($"Név megadása kötelező!", "SaveReport", MessageBoxButton.OK, MessageBoxImage.Error);
                _dialogs.ShowErrorInfo($"Név megadása kötelező!", "SaveReport");
            }
        }
        #endregion

        #region Constructor
        public NewQrqcStatusReportViewModel(IUserDialogService dialogs)
        {
            _dialogs = dialogs;
            StatusReports = new();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes the necessary data and state for the current instance based on the specified parent name.
        /// </summary>
        /// <remarks>This method performs several operations, including loading data, building reports,
        /// and preparing collections for further processing. It updates the state of the instance, including properties
        /// such as <c>ReportName</c>, <c>MustSave</c>, and <c>grouppedStopTimes</c>. The method also triggers property
        /// change notifications for <c>IsLoading</c> at the start and end of the operation.</remarks>
        /// <param name="parentName">The name of the parent entity used to initialize the instance.</param>
        /// <returns></returns>
        public async Task Init(string parentName)
        {
            _isLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            _parentName = parentName;
            GetFollowupDocument();
            LoadDataWithDapper();
            BuildReport();
            if (_followupDocument.StatusReportsQRQC != null)
            {
                _reportNames = _followupDocument.StatusReportsQRQC.Select(s => s.ReportName).ToList();
            }

            ReportName = $"{_yearMonth}-{DateTime.Today.Day}-QRQC-00";
            MustSave = "Nem mentett dokumentum";
            DataExchangeManagement dem = new DataExchangeManagement();
            //CS1503 miatt async-et kell használni
            
            _stopTimes = new ObservableCollection<ExtCapacityLedgerEntry>(await dem.GetExtCapacityLedgerEntriesIntervallStopCodes(_followupDocument.StartDate, _followupDocument.FinishDate));
            grouppedStopTimes = _stopTimes.GroupBy(s => new { s.StopCode, s.Workcenter})
                .Select(s => new StopTime
                {
                    Workcenter = s.Key.Workcenter, 
                    StopCode = s.Key.StopCode, 
                    TtlStopTime = s.Sum(s => s.StopTime) 
                }).ToList();
            _isLoading = false;
            OnPropertyChanged(nameof(IsLoading));
        }
        #endregion

        #region Private methods
        private void LoadDataWithDapper()
        {            
            DapperFunctions df = new DapperFunctions();
            _plan = df.GetPlanningMasterData(_followupDocument.PlanName);
            _yearMonth = $"{_plan.Select(p => p.Plan_StartPeriod).FirstOrDefault().Year.ToString()}{_plan.Select(p => p.Plan_StartPeriod).FirstOrDefault().Month.ToString("D2")}";
            //_itemDcCost = df.GetItemDc();

            //if (!_yearMonth.IsNullOrEmpty())
            //{
            //    _turnover = df.GetTurnover(_followupDocument.PlanName, _yearMonth);
            //}
        }
        private void GetFollowupDocument()
        {
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);
            var doc = databaseCollection.Find(x => x.DocumentName == _parentName).FirstOrDefault();
            _followupDocument = doc;
        }
        /// <summary>
        /// Processes follow-up data for various workcenter types, including machine, inspection, and manual processes, 
        /// and generates status reports for each workcenter.
        /// </summary>
        /// <remarks>This method iterates through the follow-up data for different workcenter types and
        /// calculates various metrics,  such as cumulative output, cumulative plan, reject ratios, and efficiency. It
        /// also generates chart data for  visualizing output and plan trends. The processed data is added to the <see
        /// cref="StatusReports"/> collection.</remarks>
        private void WorkcenterDataProcess()
        {

            foreach (var document in _followupDocument.MachineFollowups)
            {
                StatusReportMachineList sr = new StatusReportMachineList();
                sr.WorkcenterType = EnumMachineType.FFCMachineProcess.ToString();
                sr.Workcenter = document.Workcenter;
                sr.ComulatedOut = document.MachineFollowupDocuments.Select(s => s.TTLOutput).Sum();
                sr.ComulatedPlan = document.MachineFollowupDocuments.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                sr.KftReject = document.MachineFollowupDocuments.Select(s => s.RejectSum).Sum();
                sr.SupplierReject = document.MachineFollowupDocuments.Select(s => s.SupplierReject).Sum();
                sr.Output = document.MachineFollowupDocuments.Select(x => x.ComulatedOutput).ToList();
                sr.ComulatedPlans = document.MachineFollowupDocuments.Select(x => x.ComulatedPlan).ToList();
                sr.Efficiency = document.MachineFollowupDocuments.Select(x => x.Efficiency).ToList();
                if (document.MachineFollowupDocuments.Where(x => x.Efficiency > 0).Select(x => x.Efficiency).ToList().Count != 0)
                {
                    sr.AvgEfficiency = document.MachineFollowupDocuments.Where(x => x.Efficiency > 0).Select(x => x.Efficiency).Average();
                }
                if (document.MachineFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).ToList().Count() != 0)
                {
                    sr.AvgRejectRatio = document.MachineFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).Average();
                    //sr.AvgRejectRatio = document.MachineFollowupDocuments.Select(x => x.CalcRejectRatio).Average();
                }

                sr.ChartData = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (document.MachineFollowupDocuments.Select(x => x.ComulatedOutput).ToList()),
                    },
                    new LineSeries
                    {
                        Title = "Comulated plan",
                        Values = new ChartValues<int>(document.MachineFollowupDocuments.Select(x => x.ComulatedPlan)),
                    },
                };
                StatusReports.Add(sr);
            }

            foreach (var document in _followupDocument.InspectionFollowups)
            {
                StatusReportMachineList sr = new StatusReportMachineList();
                sr.WorkcenterType = EnumMachineType.FFCInscpectionProcess.ToString();
                sr.Workcenter = document.Workcenter;
                sr.ComulatedOut = document.InspectionFollowupDocuments.Select(s => s.TTLOutput).Sum();
                sr.ComulatedPlan = document.InspectionFollowupDocuments.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                sr.KftReject = document.InspectionFollowupDocuments.Select(s => s.RejectSum).Sum();
                sr.SupplierReject = document.InspectionFollowupDocuments.Select(s => s.SupplierReject).Sum();
                sr.Output = document.InspectionFollowupDocuments.Select(x => x.ComulatedOutput).ToList();
                sr.ComulatedPlans = document.InspectionFollowupDocuments.Select(x => x.ComulatedPlan).ToList();
                if (document.InspectionFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).ToList().Count() != 0)
                {
                    sr.AvgRejectRatio = document.InspectionFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).Average();
                    //sr.AvgRejectRatio = document.InspectionFollowupDocuments.Select(x => x.CalcRejectRatio).Average();
                }
                sr.ChartData = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (document.InspectionFollowupDocuments.Select(x => x.ComulatedOutput).ToList()),
                    },
                    new LineSeries
                    {
                        Title = "Comulated plan",
                        Values = new ChartValues<int>(document.InspectionFollowupDocuments.Select(x => x.ComulatedPlan)),
                    },
                };
                StatusReports.Add(sr);
            }

            foreach (var document in _followupDocument.ManualFollowups)
            {
                _wordayNumber = document.MaualFollowupDocuments.Select(c => c.Workday).ToList().Count();
                StatusReportMachineList sr = new StatusReportMachineList();
                sr.WorkcenterType = EnumMachineType.FFCManualProcess.ToString();
                sr.Workcenter = document.Workcenter;
                sr.ComulatedOut = document.MaualFollowupDocuments.Select(s => s.TTLOutput).Sum();
                sr.ComulatedPlan = document.MaualFollowupDocuments.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                sr.KftReject = document.MaualFollowupDocuments.Select(s => s.RejectSum).Sum();
                sr.SupplierReject = document.MaualFollowupDocuments.Select(s => s.SupplierReject).Sum();
                sr.Output = document.MaualFollowupDocuments.Select(x => x.ComulatedOutput).ToList();
                sr.ComulatedPlans = document.MaualFollowupDocuments.Select(x => x.ComulatedPlan).ToList();
                sr.Efficiency = document.MaualFollowupDocuments.Select(x => x.ProductivityKft).ToList();
                sr.EfficiencySubcon = document.MaualFollowupDocuments.Select(x => x.ProductivitySubcon).ToList();
                if (document.MaualFollowupDocuments.Where(x => x.ProductivityKft > 0).Select(x => x.ProductivityKft).ToList().Count() != 0)
                {
                    sr.AvgEfficiency = document.MaualFollowupDocuments.Where(x => x.ProductivityKft > 0).Select(x => x.ProductivityKft).Average();
                }
                if (document.MaualFollowupDocuments.Where(x => x.ProductivitySubcon > 0).Select(x => x.ProductivitySubcon).ToList().Count() != 0)
                {
                    sr.AvgEfficiencySubcon = document.MaualFollowupDocuments.Where(x => x.ProductivitySubcon > 0).Select(x => x.ProductivitySubcon).Average();
                }
                if (document.MaualFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).ToList().Count() != 0)
                {
                    sr.AvgRejectRatio = document.MaualFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).Average();
                    //sr.AvgRejectRatio = document.MaualFollowupDocuments.Select(x => x.CalcRejectRatio).Average();
                }

                sr.ChartData = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (document.MaualFollowupDocuments.Select(x => x.ComulatedOutput).ToList()),
                    },
                    new LineSeries
                    {
                        Title = "Comulated plan",
                        Values = new ChartValues<int>(document.MaualFollowupDocuments.Select(x => x.ComulatedPlan)),
                    },
                };

                StatusReports.Add(sr);
            }
        }
        /// <summary>
        /// Processes planning data to calculate and update various metrics, including planned quantities, prices, and
        /// material costs.
        /// </summary>
        /// <remarks>This method filters and transforms the planning data based on specific rules, such as
        /// handling comments that start with "V"  and adjusting quantities and prices for entries marked with a "-" in
        /// their comments. It calculates and updates several  aggregated values, including sample prices, planned
        /// sales, distribution center costs, and material costs. The results  are stored in the associated data model
        /// and formatted for display in the user interface.</remarks>
        private void PlanningDataProcess()
        {
            #region SideEffect módosítás

            //var planChangeList = _plan.Where(p => p.Plan_Comment != null && p.Plan_Comment.StartsWith("V")).ToList().OrderBy(p => p.Plan_Comment);

            //PlanChanges = new ObservableCollection<PlanningMasterData>(planChangeList);

            //Terv módosítás házi szabály VXX+ -> hozzáadás, VYY- kivétel
            //foreach (var i in PlanChanges)
            //{
            //    if (i.Plan_Comment.Contains("-"))
            //    {
            //        i.Plan_PlannedQty = i.Plan_PlannedQty * -1;
            //        i.Plan_PriceOfPlanned = i.Plan_PriceOfPlanned * -1;
            //    }
            //}
            #endregion

            PlanChanges = PlanChanges = new ObservableCollection<PlanningMasterData>(
                _plan
                    .Where(p => p.Plan_Comment != null && p.Plan_Comment.StartsWith("V"))
                    .OrderBy(p => p.Plan_Comment)
                    .Select(p => new PlanningMasterData
                    {
                        // csak a szükséges mezők – ÚJ példány!
                        Plan_Comment = p.Plan_Comment,
                        Plan_PlannedQty = p.Plan_Comment.Contains("-") ? -p.Plan_PlannedQty : p.Plan_PlannedQty,
                        Plan_PriceOfPlanned = p.Plan_Comment.Contains("-") ? -p.Plan_PriceOfPlanned : p.Plan_PriceOfPlanned,
                        // ... további mezők, amik kellenek a UI-hoz
                    })
            );

            _planData.PlanChanges = PlanChanges;

            __samplePrice = _plan.Where(p => p.Plan_isSample == 1).Select(p => p.Plan_PriceOfPlanned).Sum();
            _planData.Sample = __samplePrice;
            SamplePrice = __samplePrice.ToString("N2");                    
           
            __planPrice = _plan.Where(p => p.Plan_isSample == 0).Select(p => p.Plan_PriceOfPlanned).Sum();           
            _planData.OutputPlanSales = __planPrice;            

            PlanPrice = __planPrice.ToString("N2");

            __planDcPrice = _plan.Where(p => p.Plan_isSample == 0).Select(p => p.Plan_FGDCforPlanned).Sum();
            _planData.OutputPlanDc = __planDcPrice;
            PlanDcPrice = __planDcPrice.ToString("N2");

            __materialCost = _plan.Where(p => p.Plan_isSample == 0).Select(p => p.Plan_RMCostPlanned).Sum();
            _planData.MaterialCost = __materialCost;
            MaterialCost = __materialCost.ToString("N2");

            TtlPlan = _planData.TtlPlan.ToString("N2");
        }
        /// <summary>
        /// Calculates and updates various production and sales metrics, including sales plans,  DC movements,
        /// production completion ratios, and daily planned quantities.
        /// </summary>
        /// <remarks>This method aggregates and processes data related to sales, production, and workdays 
        /// to compute key performance indicators such as sales plans, DC movements, production  completion ratios, and
        /// time-proportional ratios. The results are stored in internal  fields and properties for further use.  The
        /// method assumes that the underlying data structures, such as sales and production  plans, are properly
        /// initialized and populated. It also relies on the availability of  workday and headcount information to
        /// calculate time-proportional metrics.</remarks>
        private void ResultCalculation()
        {
            _planData.SalesPlan = _actualTurnoverInSalesWithoutSample + _remainingTurnoverInSalesWithoutSample;
            _planData.SalesPlanDC = _actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample;
            _planData.DCMovement = __planDcPrice - (_actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample);

            SalesPlan = (_actualTurnoverInSalesWithoutSample + _remainingTurnoverInSalesWithoutSample).ToString("N2");
            DcMovement = (__planDcPrice - (_actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample)).ToString("N2");
            SalesPlanDc = (_actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample).ToString("N2");

            _currentWorkday = _followupDocument.Headcount.Where(s => s.ActualHC > 0).Select(s => s.ActualHC).ToList().Count();
            var _completedKftProduction = _plan.Where(s => s.Plan_isFinished == 0).Select(s => s.Plan_FinishedQty).Sum();
            var _ttlPlanKftProduction = _plan.Where(s => s.Plan_isFinished == 0).Select(s => s.Plan_PlannedQty).Sum();
            
            var _completeRepackProduction = _plan.Where(s => s.Plan_isFinished == 1).Select(s => s.Plan_FinishedQty).Sum();
            var _ttlPlanRepackProduction = _plan.Where(s => s.Plan_isFinished == 1).Select(s => s.Plan_PlannedQty).Sum();

            _kftProdCompleteRatio = (_completedKftProduction / _ttlPlanKftProduction).ToString("P2");
            _repackProdCompleteRatio = (_completeRepackProduction / _ttlPlanRepackProduction).ToString("P2");

            var _repackDailyPlan = _ttlPlanRepackProduction / _followupDocument.Workdays;
            _repackTimePropRatio = (_completeRepackProduction / (_repackDailyPlan * (_currentWorkday))).ToString("P2");

            var _kftDailyPlan = _ttlPlanKftProduction / _followupDocument.Workdays;
            _kftTimePropRatio = (_completedKftProduction / (_kftDailyPlan * (_currentWorkday))).ToString("P2");

            _kftDailyPlannedQty = _plan.Where(p => p.Plan_isFinished == 0)
                .GroupBy(p => p.Plan_ETD.Date) // csak a dátum része számít
                .Select(g => new DailyPlan
                {
                    Workday = g.Key,
                    PlannedQty = g.Sum(x => x.Plan_PlannedQty),
                    ActualQty = g.Sum(x => x.Plan_FinishedQty)
                })
                .OrderBy(x => x.Workday)
                .ToList();

            _repackDailyPlannedQty = _plan.Where(p => p.Plan_isFinished == 1)
                .GroupBy(p => p.Plan_ETD.Date) // csak a dátum része számít
                .Select(g => new DailyPlan
                {
                    Workday = g.Key,
                    PlannedQty = g.Sum(x => x.Plan_PlannedQty),
                    ActualQty = g.Sum(x => x.Plan_FinishedQty)
                })
                .OrderBy(x => x.Workday)
                .ToList();

        }

        private void BuildReport()
        {            
            //Order is important!!
            WorkcenterDataProcess();
            PlanningDataProcess();
            ResultCalculation();
        }
        /// <summary>
        /// Saves the current document to the database.
        /// </summary>
        /// <remarks>This method updates the database with the current state of the follow-up document. 
        /// If the document does not already exist in the database, it will be added.  If an error occurs during the
        /// operation, the method returns a failure status along with an error message.</remarks>
        /// <returns>A tuple containing two values: <list type="bullet"> <item> <term><c>isCompleted</c></term> <description><see
        /// langword="true"/> if the operation was successful; otherwise, <see langword="false"/>.</description> </item>
        /// <item> <term><c>message</c></term> <description>A string containing an error message if the operation fails;
        /// otherwise, an empty string.</description> </item> </list></returns>
        private (bool isCompleted, string message) SaveDocumentToDatabase()
        {
            (bool isCompleted, string message) ret = (false, string.Empty);

            StatusReportQrqc statusReport = new StatusReportQrqc()
            {
                IssueDate = DateOnly.FromDateTime(DateTime.Now),
                WokdaysNum = _wordayNumber,
                ReportName = ReportName,
                MachineData = StatusReports.ToList(),
                PlansData = _planData,
                KftProdCompleteRatio = _kftProdCompleteRatio,
                RepackProdCompleteRatio = _repackProdCompleteRatio,
                KftProdTimePropRatio = _kftTimePropRatio,
                RepackProdTimePropRatio = _repackTimePropRatio,
                ActualWorkday = _currentWorkday,
                KftDailyPlans = _kftDailyPlannedQty,
                RepackDailyPlans = _repackDailyPlannedQty,
                StopTimes = grouppedStopTimes,
                //Turnover = _turnover.ToList(),
                //ShipoutPlan = _shipoutPlan.ToList(),
            };
            if (_followupDocument.StatusReportsQRQC == null)
            {
                _followupDocument.StatusReportsQRQC = new List<StatusReportQrqc>();
                _followupDocument.StatusReportsQRQC.Add(statusReport);
            }
            else
            {
                _followupDocument.StatusReportsQRQC.Add(statusReport);
            }

            try
            {
                ConnectionManagement conMgmnt = new ConnectionManagement();
                conMgmnt.ConnectToDatabase();

                var collection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);
                var filter = Builders<MasterFollowupDocument>.Filter.Eq(x => x.id, _followupDocument.id);
                collection.ReplaceOne(filter, _followupDocument);
                ret.isCompleted = true;
                return ret;
            }
            catch (Exception ex)
            {
                ret.isCompleted = false;
                ret.message = ex.Message;
                return ret;
            }
        }
        #endregion
    }
}
