using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Enums;
using ProdInfoSys.Models;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.NonRelationalModels;
using ProdInfoSys.Models.StatusReportModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProdInfoSys.ViewModels
{
    public class NewStatusReportViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        #endregion

        private MasterFollowupDocument _followupDocument;
        private ObservableCollection<PlanningMasterData> _plan;
        private ObservableCollection<Turnover> _turnover;
        private ObservableCollection<ItemDcCost> _itemDcCost;
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
        private decimal __repackMaterialCost = 0;
        private string _yearMonth = string.Empty;
        private int _wordayNumber = 0;
        private List<DailyPlan> _kftDailyPlannedQty = new List<DailyPlan>();
        private List<DailyPlan> _repackDailyPlannedQty = new List<DailyPlan>();

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by the implementation of the INotifyPropertyChanged
        /// interface to notify subscribers that a property value has changed. Handlers attached to this event receive
        /// the name of the property that changed in the event data.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in a property's setter to notify subscribers that the property's
        /// value has changed. This is commonly used to support data binding in applications that implement the
        /// INotifyPropertyChanged interface.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and will be automatically set to the caller
        /// member name if not specified.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication
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

        private string _repackaterialCost;
        public string RepackaterialCost
        {
            get => _repackaterialCost;
            set { _repackaterialCost = value; OnPropertyChanged(); }
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

        #region Constructor
        public NewStatusReportViewModel(IUserDialogService dialogs)
        {
            _dialogs = dialogs;
            StatusReports = new();
        }
        #endregion

        #region ICommand
        public ICommand SaveReportData => new ProjectCommandRelay(_ => SavingReportData());
        /// <summary>
        /// Attempts to save the current report data to the database if the report name is valid and not already used.
        /// </summary>
        /// <remarks>Displays informational or error dialogs to notify the user of the result of the save
        /// operation. The method does not perform any action if the report name is empty or already exists.</remarks>
        private void SavingReportData()
        {
            if (!ReportName.IsNullOrEmpty())
            {
                if (!_reportNames.Contains(ReportName))
                {
                    (bool isCompleted, string message) = SaveDocumentToDatabase();

                    if (isCompleted)
                    {
                        _dialogs.ShowInfo($"A mentés sikeres", "SaveReport");
                        _reportNames.Add(ReportName);
                        MustSave = string.Empty;
                    }
                    else
                    {
                        _dialogs.ShowErrorInfo($"Hiba történt a mentés alatt!: {message}", "SaveReport");
                    }
                }
                else
                {
                    _dialogs.ShowErrorInfo($"A megadott név már létezik!", "SaveReport");
                }
            }
            else
            {
                _dialogs.ShowErrorInfo($"Név megadása kötelező!", "SaveReport");
            }
        }

        public ICommand ImportShipoutPlan => new ProjectCommandRelay(_ => ImportingShipoutPlan());

        private void ImportingShipoutPlan()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel file (*.xlsx)|*.xlsx";
            dialog.Title = "Kiszállítási terv importálás";

            if (string.IsNullOrEmpty(RegistryManagement.ReadStringRegistryKey("LastPath")))
            {
                dialog.InitialDirectory = Environment.CurrentDirectory;
            }
            else
            {
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(RegistryManagement.ReadStringRegistryKey("LastPath"));
            }

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _shipoutPlan.Clear();
                    RegistryManagement.WriteStringRegistryKey("LastPath", dialog.FileName);
                    ImportExcelShipoutPlan(dialog.FileName);
                    MustSave = "Nem mentett dokumentum";
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"{ex.Message}", "ImportShipoutPlan", MessageBoxButton.OK, MessageBoxImage.Error);
                    _dialogs.ShowErrorInfo($"{ex.Message}", "ImportShipoutPlan");
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes the report by setting the parent name and loading related data.
        /// </summary>
        /// <remarks>Call this method before accessing report data to ensure all required information is
        /// loaded and initialized.</remarks>
        /// <param name="parentName">The name of the parent entity to associate with the report. Cannot be null.</param>
        public void Init(string parentName)
        {
            _parentName = parentName;
            GetFollowupDocument();
            LoadDataWithDapper();
            BuildReport();
            if (_followupDocument.StatusReports != null)
            {
                _reportNames = _followupDocument.StatusReports.Select(s => s.ReportName).ToList();
            }
            ReportName = $"{_yearMonth}-{DateTime.Today.Day}-PROD-00";
            MustSave = "Nem mentett dokumentum";
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Imports a shipout plan from the specified Excel file and updates the current shipout plan data.
        /// </summary>
        /// <remarks>After importing, the method updates related data and recalculates results based on
        /// the imported shipout plan. If an error occurs during import, an error message is displayed to the
        /// user.</remarks>
        /// <param name="file">The path to the Excel file containing the shipout plan to import. The file must be accessible and in a
        /// supported format.</param>
        private void ImportExcelShipoutPlan(string file)
        {
            try
            {
                ExcelIO e = new ExcelIO();
                _shipoutPlan = e.ImportShipoutPlan(file);

                foreach (var plan in _shipoutPlan)
                {
                    var match = _itemDcCost.FirstOrDefault(c => c.Item == plan.Szam);
                    plan.ShipOutStd = match.StdCost * plan.NyitottMennyiseg;
                }

                OnPropertyChanged(nameof(ShipoutPlan));

                TurnoverDataProcess();
                ShipoutDataProcess();
                ResultCalculation();
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Kiszállítási terv import");
            }
        }

        /// <summary>
        /// Loads planning, item, and turnover data from the database using Dapper and updates the corresponding fields.
        /// </summary>
        /// <remarks>This method retrieves planning master data, item distribution center costs, and
        /// turnover information based on the current plan and period. If an error occurs during data retrieval, an
        /// error dialog is displayed to the user.</remarks>
        private void LoadDataWithDapper()
        {
            try
            {
                DapperFunctions df = new DapperFunctions();
                _plan = df.GetPlanningMasterData(_followupDocument.PlanName);
                _yearMonth = $"{_plan.Select(p => p.Plan_StartPeriod).FirstOrDefault().Year.ToString()}{_plan.Select(p => p.Plan_StartPeriod).FirstOrDefault().Month.ToString("D2")}";
                _itemDcCost = df.GetItemDc();
                if (!_yearMonth.IsNullOrEmpty())
                {
                    _turnover = df.GetTurnover(_yearMonth);
                }
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"Hiba történt : {ex.Message}", "Termelési terv betöltés");
            }
        }

        /// <summary>
        /// Retrieves the follow-up document associated with the current parent name from the database and assigns it to
        /// the internal field.
        /// </summary>
        /// <remarks>This method queries the database for a document matching the current parent name. If
        /// no matching document is found, the internal field is set to null.</remarks>
        private void GetFollowupDocument()
        {
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);
            var doc = databaseCollection.Find(x => x.DocumentName == _parentName).FirstOrDefault();
            _followupDocument = doc;
        }
        /// <summary>
        /// Processes follow-up data for various workcenter types and generates status reports.
        /// </summary>
        /// <remarks>This method iterates through follow-up data for machine, inspection, and manual
        /// processes, calculates aggregated metrics such as output, plan, reject ratios, and efficiency, and generates
        /// status reports for each workcenter. The reports include data for chart visualization and other metrics
        /// relevant to the workcenter's performance.</remarks>
        private void WorkcenterDataProcess()
        {

            foreach (var document in _followupDocument.MachineFollowups)
            {
                StatusReportMachineList sr = new StatusReportMachineList();
                sr.WorkcenterType = EnumMachineType.MachineProcess.ToString();
                sr.Workcenter = document.Workcenter;
                sr.ComulatedOut = document.MachineFollowupDocuments.Select(s => s.TTLOutput).Sum();
                sr.ComulatedPlan = document.MachineFollowupDocuments.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                sr.Reject = document.MachineFollowupDocuments.Select(s => s.RejectSum).Sum();
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
                sr.WorkcenterType = EnumMachineType.InscpectionProcess.ToString();
                sr.Workcenter = document.Workcenter;
                sr.ComulatedOut = document.InspectionFollowupDocuments.Select(s => s.TTLOutput).Sum();
                sr.ComulatedPlan = document.InspectionFollowupDocuments.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                sr.Reject = document.InspectionFollowupDocuments.Select(s => s.RejectSum).Sum();
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
                sr.WorkcenterType = EnumMachineType.ManualProcess.ToString();
                sr.Workcenter = document.Workcenter;
                sr.ComulatedOut = document.MaualFollowupDocuments.Select(s => s.TTLOutput).Sum();
                sr.ComulatedPlan = document.MaualFollowupDocuments.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                sr.Reject = document.MaualFollowupDocuments.Select(s => s.RejectSum).Sum();
                sr.SupplierReject = document.MaualFollowupDocuments.Select(s => s.SupplierReject).Sum();
                sr.Output = document.MaualFollowupDocuments.Select(x => x.ComulatedOutput).ToList();
                sr.ComulatedPlans = document.MaualFollowupDocuments.Select(x => x.ComulatedPlan).ToList();
                sr.Efficiency = document.MaualFollowupDocuments.Select(x => x.Productivity).ToList();
                sr.EfficiencySubcon = document.MaualFollowupDocuments.Select(x => x.ProductivitySubcon).ToList();
                if (document.MaualFollowupDocuments.Where(x => x.Productivity > 0).Select(x => x.Productivity).ToList().Count() != 0)
                {
                    sr.AvgEfficiency = document.MaualFollowupDocuments.Where(x => x.Productivity > 0).Select(x => x.Productivity).Average();
                }
                if (document.MaualFollowupDocuments.Where(x => x.ProductivitySubcon > 0).Select(x => x.ProductivitySubcon).ToList().Count() != 0)
                {
                    sr.AvgEfficiencySubcon = document.MaualFollowupDocuments.Where(x => x.ProductivitySubcon > 0).Select(x => x.ProductivitySubcon).Average();
                }
                if (document.MaualFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).ToList().Count() != 0)
                {
                    sr.AvgRejectRatio = document.MaualFollowupDocuments.Where(x => x.CalcRejectRatio > 0).Select(x => x.CalcRejectRatio).Average();
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
        /// Processes planning data to calculate and update various metrics, including plan changes, sample prices,
        /// planned sales, distribution center costs, and material costs. The results are stored in the associated data
        /// model and exposed through relevant properties.
        /// </summary>
        /// <remarks>This method filters and transforms the planning data based on specific conditions,
        /// such as comments starting with "V" or whether the data represents a sample. It calculates aggregated values
        /// for different metrics and updates the corresponding fields in the data model. The method ensures that only
        /// the necessary fields are included in the processed data.</remarks>
        private void PlanningDataProcess()
        {
            #region SideEffect módosítás
            //PlanChanges = new ObservableCollection<PlanningMasterData>(
            //    _plan.Where(p => p.Plan_Comment != null && p.Plan_Comment.StartsWith("V")).ToList().OrderBy(p => p.Plan_Comment)
            //);

            ////Terv módosítás házi szabály VXX+ -> hozzáadás, VYY- kivétel
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

            __repackMaterialCost = _plan.Where(p => p.Plan_isSample == 0 && p.Plan_isFinished == 1).Select(p => p.Plan_RMCostPlanned).Sum();
            _planData.RepackMaterialCost = __repackMaterialCost;
            RepackaterialCost = __repackMaterialCost.ToString("N2");

            TtlPlan = _planData.TtlPlan.ToString("N2");
        }

        private void TurnoverDataProcess()
        {
            _actualTurnoverInSalesWithoutSample = _turnover.Where(t => t.No != _sampleItem && t.ShortcutDimension1Code == _area)
                .Select(t => t.AmountEUR).Sum();

            _actualTurnoverInDcWithoutSample = _turnover.Where(t => t.No != _sampleItem && t.ShortcutDimension1Code == _area)
                .Select(t => t.LineTTLDcPrice).Sum();
        }

        private void ShipoutDataProcess()
        {
            _remainingTurnoverInSalesWithoutSample = _shipoutPlan.Where(t => t.Szam != _sampleItem)
                .Select(t => t.NyitottOsszeg).Sum();

            _remainingTurnoverInDcWithoutSample = _shipoutPlan.Where(t => t.Szam != _sampleItem)
                .Select(t => t.ShipOutStd).Sum();
        }
        /// <summary>
        /// Updates various production and sales plan metrics based on the current turnover, planned values, and actual
        /// production data.
        /// </summary>
        /// <remarks>This method calculates and updates several key metrics, including sales plans, DC
        /// movements, production completion ratios,  and time-proportional ratios for both KFT and repack production.
        /// It also generates daily planned quantities for KFT and  repack production, grouped by workdays.  The
        /// calculations rely on the current turnover, remaining turnover, planned values, and actual production data. 
        /// Ensure that the relevant data fields, such as turnover values, planned quantities, and actual quantities,
        /// are properly initialized  before invoking this method.</remarks>
        private void ResultCalculation()
        {
            _planData.SalesPlan = _actualTurnoverInSalesWithoutSample + _remainingTurnoverInSalesWithoutSample;
            _planData.SalesPlanDC = _actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample;
            _planData.DCMovement = __planDcPrice - (_actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample);

            SalesPlan = (_actualTurnoverInSalesWithoutSample + _remainingTurnoverInSalesWithoutSample).ToString("N2");
            DcMovement = (__planDcPrice - (_actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample)).ToString("N2");
            SalesPlanDc = (_actualTurnoverInDcWithoutSample + _remainingTurnoverInDcWithoutSample).ToString("N2");
            //Currentworkday 0 ha nincs beírva a létszám, ami a QRQC reportok létrehozásakor fog hibára futni
            _currentWorkday = _followupDocument.Headcount.Where(s => s.ActualHC > 0).Select(s => s.ActualHC).ToList().Count();
            if (_currentWorkday == 0)
            {
                _currentWorkday = 1;
            }
            var _completedKftProduction = _plan.Where(s => s.Plan_isFinished == 0).Select(s => s.Plan_FinishedQty).Sum();
            var _ttlPlanKftProduction = _plan.Where(s => s.Plan_isFinished == 0).Select(s => s.Plan_PlannedQty).Sum();

            var _completeRepackProduction = _plan.Where(s => s.Plan_isFinished == 1).Select(s => s.Plan_FinishedQty).Sum();
            var _ttlPlanRepackProduction = _plan.Where(s => s.Plan_isFinished == 1).Select(s => s.Plan_PlannedQty).Sum();

            _kftProdCompleteRatio = (_completedKftProduction / _ttlPlanKftProduction).ToString("P2");
            _repackProdCompleteRatio = (_completeRepackProduction / _ttlPlanRepackProduction).ToString("P2");

            var _repackDailyPlan = _ttlPlanRepackProduction / _followupDocument.Workdays;

            _repackTimePropRatio = (_completeRepackProduction / (_repackDailyPlan * _currentWorkday)).ToString("P2");

            var _kftDailyPlan = _ttlPlanKftProduction / _followupDocument.Workdays;
            _kftTimePropRatio = (_completedKftProduction / (_kftDailyPlan * _currentWorkday)).ToString("P2");

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

        /// <summary>
        /// Builds the complete report by processing all required data sections in sequence.
        /// </summary>
        /// <remarks>This method orchestrates the report generation process by invoking a series of data
        /// processing steps in a specific order. It is intended for internal use within the report generation workflow
        /// and should not be called directly by external code.</remarks>
        private void BuildReport()
        {
            //Order is important!!
            WorkcenterDataProcess();
            PlanningDataProcess();
            TurnoverDataProcess();
            ShipoutDataProcess();
            ResultCalculation();
        }

        /// <summary>
        /// Attempts to save the current document and its associated status report to the database.
        /// </summary>
        /// <remarks>If the save operation fails, the returned message provides details about the error.
        /// The method updates the document's status reports before attempting to save. This method does not throw
        /// exceptions; instead, it returns error information in the result tuple.</remarks>
        /// <returns>A tuple containing a Boolean value that indicates whether the operation completed successfully, and a
        /// message describing the result or any error encountered.</returns>
        private (bool isCompleted, string message) SaveDocumentToDatabase()
        {
            (bool isCompleted, string message) ret = (false, string.Empty);

            StatusReport statusReport = new StatusReport()
            {
                IssueDate = DateOnly.FromDateTime(DateTime.Now),
                WokdaysNum = _wordayNumber,
                ReportName = ReportName,
                MachineData = StatusReports.ToList(),
                PlansData = _planData,
                Turnover = _turnover.ToList(),
                ShipoutPlan = _shipoutPlan.ToList(),
                ActualWorkday = _currentWorkday,
                DailyPlans = _kftDailyPlannedQty,
                RepackDailyPlans = _repackDailyPlannedQty,
                ProdCompleteRatio = _kftProdCompleteRatio,
                RepackProdCompleteRatio = _repackProdCompleteRatio,
                ProdTimePropRatio = _kftTimePropRatio,
                RepackProdTimePropRatio = _repackTimePropRatio,

            };
            if (_followupDocument.StatusReports == null)
            {
                _followupDocument.StatusReports = new List<StatusReport>();
                _followupDocument.StatusReports.Add(statusReport);
            }
            else
            {
                _followupDocument.StatusReports.Add(statusReport);
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
