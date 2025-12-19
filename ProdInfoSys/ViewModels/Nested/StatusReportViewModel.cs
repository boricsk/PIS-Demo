using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Driver;
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
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ProdInfoSys.ViewModels.Nested
{
    /// <summary>
    /// Represents the view model for generating and managing status reports, including production, efficiency, and
    /// shipment data for reporting and analysis in a WPF application.
    /// </summary>
    /// <remarks>The StatusReportViewModel provides properties and commands to support data binding in WPF
    /// views, enabling the display and export of various production metrics, plan changes, and efficiency charts. It
    /// implements INotifyPropertyChanged to support dynamic UI updates and exposes methods for initializing report
    /// data, filtering collections, and exporting reports. This view model is intended to be used as the data context
    /// for status report views, facilitating user interactions such as searching, exporting, and sending summary
    /// emails. Thread safety is not guaranteed; all interactions should occur on the UI thread.</remarks>
    public class StatusReportViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        private IUserControlFunctions _userControlFunctions;
        private IConnectionManagement _connectionManagement;
        #endregion

        private List<string> _xaxisWorkdays = new List<string>();
        private TreeNodeModel? _parent;
        private string? _selectedReportName;
        private string? _statusReportName;
        private List<double>? _kftRejectPercent = new();
        private List<int>? _kftRejects = new();
        private List<int>? _supplierRejects = new();
        private List<decimal>? _kftOutputs = new();
        private List<decimal>? _kftActualRejectRatios = new();
        private ObservableCollection<StatusReport>? _allStatusReport;
        private ObservableCollection<ShipoutPlan>? _allShipouts = new ObservableCollection<ShipoutPlan>();
        private FollowupMetadata _result = new FollowupMetadata();

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by the implementation of the INotifyPropertyChanged
        /// interface to notify subscribers that a property value has changed. Handlers attached to this event receive
        /// the name of the property that changed in the PropertyChangedEventArgs parameter. This event is commonly used
        /// in data binding scenarios to update UI elements when underlying data changes.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in the setter of a property to notify subscribers that the property
        /// value has changed. This is commonly used to implement the INotifyPropertyChanged interface in data-binding
        /// scenarios.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and is automatically provided when called from
        /// a property setter.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication
        private ObservableCollection<DailyPlan>? _kftDailyPlannedQty;
        public ObservableCollection<DailyPlan>? KftDailyPlannedQty
        {
            get => _kftDailyPlannedQty;
            set { _kftDailyPlannedQty = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DailyPlan>? _repackDailyPlannedQty;
        public ObservableCollection<DailyPlan>? RepackDailyPlannedQty
        {
            get => _repackDailyPlannedQty;
            set { _repackDailyPlannedQty = value; OnPropertyChanged(); }
        }

        private string? _currentWorkday;
        public string? CurrentWorkday
        {
            get => _currentWorkday;
            set { _currentWorkday = value; OnPropertyChanged(); }
        }


        private string? _kftProdCompleteRatio;
        public string? KftProdCompleteRatio
        {
            get => _kftProdCompleteRatio;
            set { _kftProdCompleteRatio = value; OnPropertyChanged(); }
        }

        private string? _ttlReject;
        public string? TtlReject
        {
            get => _ttlReject;
            set { _ttlReject = value; OnPropertyChanged(); }
        }

        private string? _repackProdCompleteRatio;
        public string? RepackProdCompleteRatio
        {
            get => _repackProdCompleteRatio;
            set { _repackProdCompleteRatio = value; OnPropertyChanged(); }
        }

        private string? _repackTimePropRatio;
        public string? RepackTimePropRatio
        {
            get => _repackTimePropRatio;
            set { _repackTimePropRatio = value; OnPropertyChanged(); }
        }

        private string? _kftTimePropRatio;
        public string? KftTimePropRatio
        {
            get => _kftTimePropRatio;
            set { _kftTimePropRatio = value; OnPropertyChanged(); }
        }
        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); if (_searchText != null) { PerformSearch(_searchText); } }
        }

        private StatusReport? _statusReport;
        public StatusReport? StatusReport
        {
            get => _statusReport;
            set { _statusReport = value; OnPropertyChanged(); }
        }
        private ObservableCollection<StatusReportMachineList>? _statusReportMahines;
        public ObservableCollection<StatusReportMachineList>? StatusReportMahines
        {
            get => _statusReportMahines;
            set { _statusReportMahines = value; OnPropertyChanged(); }
        }

        private ObservableCollection<StatusReportMachineList>? _statusReportMahinesForEfficiencyChart;
        public ObservableCollection<StatusReportMachineList>? StatusReportMahinesForEfficiencyChart
        {
            get => _statusReportMahinesForEfficiencyChart;
            set { _statusReportMahinesForEfficiencyChart = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PlanningMasterData>? _planChangeDetails;
        public ObservableCollection<PlanningMasterData>? PlanChangeDetails
        {
            get => _planChangeDetails;
            set { _planChangeDetails = value; OnPropertyChanged(); }
        }

        private ObservableCollection<StatusReportMachineList>? _workcenterData;
        public ObservableCollection<StatusReportMachineList>? WorkcenterData
        {
            get => _workcenterData;
            set { _workcenterData = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ShipoutPlan>? _shipOutPlanDetails;
        public ObservableCollection<ShipoutPlan>? ShipOutPlanDetails
        {
            get => _shipOutPlanDetails;
            set { _shipOutPlanDetails = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string>? _workcenterNames;
        public ObservableCollection<string>? WorkcenterNames
        {
            get => _workcenterNames;
            set { _workcenterNames = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string>? _workcenterNamesManual;
        public ObservableCollection<string>? WorkcenterNamesManual
        {
            get => _workcenterNamesManual;
            set { _workcenterNamesManual = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string>? _workcenterNamesMachine;
        public ObservableCollection<string>? WorkcenterNamesMachine
        {
            get => _workcenterNamesMachine;
            set { _workcenterNamesMachine = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string>? _workcenterNamesInspection;
        public ObservableCollection<string>? WorkcenterNamesInspection
        {
            get => _workcenterNamesInspection;
            set { _workcenterNamesInspection = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _workcenterRejectChart;
        public SeriesCollection? WorkcenterRejectChart
        {
            get => _workcenterRejectChart;
            set { _workcenterRejectChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _planChangesChart;
        public SeriesCollection? PlanChangesChart
        {
            get => _planChangesChart;
            set { _planChangesChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _kftCumulativeRejectSumChart;
        public SeriesCollection? KftCumulativeRejectSumChart
        {
            get => _kftCumulativeRejectSumChart;
            set { _kftCumulativeRejectSumChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _salesChangesChart;
        public SeriesCollection? SalesChangesChart
        {
            get => _salesChangesChart;
            set { _salesChangesChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _rejectSumPieChart;
        public SeriesCollection? RejectSumPieChart
        {
            get => _rejectSumPieChart;
            set { _rejectSumPieChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _rejectChangesChart;
        public SeriesCollection? RejectChangesChart
        {
            get => _rejectChangesChart;
            set { _rejectChangesChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _efficiencyChart;
        public SeriesCollection? EfficiencyChart
        {
            get => _efficiencyChart;
            set { _efficiencyChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _manualRejectSumChart;
        public SeriesCollection? ManualRejectSumChart
        {
            get => _manualRejectSumChart;
            set { _manualRejectSumChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _machineRejectSumChart;
        public SeriesCollection? MachineRejectSumChart
        {
            get => _machineRejectSumChart;
            set { _machineRejectSumChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _inspectionRejectSumChart;
        public SeriesCollection? InspectionRejectSumChart
        {
            get => _inspectionRejectSumChart;
            set { _inspectionRejectSumChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _allRejectSumChart;
        public SeriesCollection? AllRejectSumChart
        {
            get => _allRejectSumChart;
            set { _allRejectSumChart = value; OnPropertyChanged(); }
        }

        private string? _samplePrice;
        public string? SamplePrice
        {
            get => _samplePrice;
            set { _samplePrice = value; OnPropertyChanged(); }
        }
        private string? _planPrice;
        public string? PlanPrice
        {
            get => _planPrice;
            set { _planPrice = value; OnPropertyChanged(); }
        }

        private string? _planDcPrice;
        public string? PlanDcPrice
        {
            get => _planDcPrice;
            set { _planDcPrice = value; OnPropertyChanged(); }
        }

        private string? _materialCost;
        public string? MaterialCost
        {
            get => _materialCost;
            set { _materialCost = value; OnPropertyChanged(); }
        }

        private string? _repackMaterialCost;
        public string? RepackMaterialCost
        {
            get => _repackMaterialCost;
            set { _repackMaterialCost = value; OnPropertyChanged(); }
        }

        private string? _kftMaterialCost;
        public string? KftMaterialCost
        {
            get => _kftMaterialCost;
            set { _kftMaterialCost = value; OnPropertyChanged(); }
        }

        private string? _ttlPlan;
        public string? TtlPlan
        {
            get => _ttlPlan;
            set { _ttlPlan = value; OnPropertyChanged(); }
        }

        private string? _salesPlan;
        public string? SalesPlan
        {
            get => _salesPlan;
            set { _salesPlan = value; OnPropertyChanged(); }
        }

        private string? _dcMovement;
        public string? DcMovement
        {
            get => _dcMovement;
            set { _dcMovement = value; OnPropertyChanged(); }
        }

        private string? _salesPlanDc;
        public string? SalesPlanDc
        {
            get => _salesPlanDc;
            set { _salesPlanDc = value; OnPropertyChanged(); }
        }

        private string? _reportName;
        public string? ReportName
        {
            get => _reportName;
            set { _reportName = value; OnPropertyChanged(); }
        }

        private string? _reportIssueDate;
        public string? ReportIssueDate
        {
            get => _reportIssueDate;
            set { _reportIssueDate = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand SendSummarizeMail => new ProjectCommandRelay(_ => SendingSummarizeMail());
        /// <summary>
        /// Sends a summary email containing meeting and plan information to designated recipients after user
        /// confirmation.
        /// </summary>
        /// <remarks>The method prompts the user for confirmation before sending the summary email. The
        /// email is sent to a list of leader email addresses and includes details such as sample price, plan price,
        /// material costs, and sales plans. If the email cannot be sent, an error message is displayed to the user.
        /// This method should be called in contexts where sending a summary report via email is appropriate.</remarks>
        private void SendingSummarizeMail()
        {
            if (MessageBox.Show($"Biztosan küldeni szeretné az összefoglalót?", "Email küldés", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {

                    //ConnectionManagement conMgmnt = new ConnectionManagement();
                    var databaseCollection = _connectionManagement.GetCollection<MeetingMinutes>(_connectionManagement.MeetingMemo);
                    var allDocuments = databaseCollection.Find(FilterDefinition<MeetingMinutes>.Empty).ToList();
                    BuildEmail email = new BuildEmail(
                        SamplePrice, PlanPrice, PlanDcPrice, MaterialCost, RepackMaterialCost, KftMaterialCost, TtlPlan, SalesPlan, SalesPlanDc, DcMovement
                        );

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(RegistryManagement.ReadStringRegistryKey("SMTPUser"));
                    foreach (var item in SetupManagement.GetLeaderEmailList())
                    {
                        mail.To.Add(item);
                    }

                    mail.Subject = $"Terv összegzés {_statusReportName}";
                    mail.IsBodyHtml = true;
                    mail.Body = email.BuidLeaderEmailMessage();

                    SmtpClient smtp = new SmtpClient(
                        RegistryManagement.ReadStringRegistryKey("SMTPServer"),
                        int.Parse(RegistryManagement.ReadStringRegistryKey("SMTPPort")));
                    smtp.Credentials = new NetworkCredential(RegistryManagement.ReadStringRegistryKey("SMTPUser"), SetupManagement.GetSmtpPassword());
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                catch (Exception ex)
                {
                    _dialogs.ShowErrorInfo($"Küldés sikertelen a következő hiba miatt : {ex.Message}", "StatusReportViewModel");
                }
            }
        }

        public ICommand? ExportShipoutPlan => new ProjectCommandRelay(_ => ExportingShipoutPlan());
        /// <summary>
        /// Exports the current shipout plan details to an Excel file.
        /// </summary>
        /// <remarks>If an error occurs during the export process, an error dialog is displayed to inform
        /// the user.</remarks>
        private void ExportingShipoutPlan()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                //e.ExportHeadcountFollowup(_headcountFollowupDocs);
                e.ExcelExport(_shipOutPlanDetails, $"Shipout plan", $"Shipout plan");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Excel export");
            }
        }

        public ICommand? ExportPlanChanges => new ProjectCommandRelay(_ => ExportingPlanChanges());
        /// <summary>
        /// Exports the current plan change details to an Excel file.
        /// </summary>
        /// <remarks>If an error occurs during the export process, an error dialog is displayed to inform
        /// the user. The exported file contains the plan change details with a default name of "Plan
        /// changes".</remarks>
        private void ExportingPlanChanges()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                e.ExcelExport(_planChangeDetails, $"Plan changes", $"Plan changes");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Excel export");
            }

        }

        public ICommand? ExportKftStatus => new ProjectCommandRelay(_ => ExportingKftStatus());
        /// <summary>
        /// Exports the current KFT daily planned quantity data to an Excel file named "KFT Status."
        /// </summary>
        /// <remarks>If an error occurs during the export process, an error dialog is displayed to inform
        /// the user. This method does not return a value and is intended to be used as part of the export
        /// workflow.</remarks>
        private void ExportingKftStatus()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                e.ExcelExport(_kftDailyPlannedQty, $"KFT Status", $"KFT Status");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Excel export");
            }

        }

        public ICommand? ExportRepackStatus => new ProjectCommandRelay(_ => ExportingRepackStatus());
        /// <summary>
        /// Exports the current repack daily planned quantity data to an Excel file named "Repack Status."
        /// </summary>
        private void ExportingRepackStatus()
        {

            ExcelIO e = new ExcelIO();
            e.ExcelExport(_repackDailyPlannedQty, $"Repack Status", $"Repack Status");

        }

        #endregion

        #region Constructor
        public StatusReportViewModel(IUserDialogService dialogs,
            IUserControlFunctions userControlFunctions,
            IConnectionManagement connectionManagement)
        {
            _dialogs = dialogs;
            _userControlFunctions = userControlFunctions;
            _connectionManagement = connectionManagement;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes the current instance with the specified parent node and selected report name.
        /// </summary>
        /// <param name="parent">The parent <see cref="TreeNodeModel"/> to associate with this instance. Cannot be null.</param>
        /// <param name="selectedReportName">The name of the report to select for this instance. Cannot be null or empty.</param>
        public void Init(TreeNodeModel parent, string selectedReportName)
        {
            _parent = parent;
            _selectedReportName = selectedReportName;
            GetFollowupDocument();
            PrepareReportData();
        }

        /// <summary>
        /// Gets a function that formats a numeric Y-axis value as a percentage string with two decimal places.
        /// </summary>
        /// <remarks>The returned formatter uses the "P2" format specifier, which multiplies the value by
        /// 100 and appends a percent sign. For example, a value of 0.1234 is formatted as "12.34%".</remarks>
        public Func<double, string> YFormatter => value => value.ToString("P2");

        /// <summary>
        /// Gets a function that formats a numeric value as a string with two decimal places, using the current
        /// culture's number format.
        /// </summary>
        public Func<double, string> YFormatterNum => value => value.ToString("N2");
        #endregion

        #region Private methods
        /// <summary>
        /// Filters the collection of ship-out plans based on the specified search term.
        /// </summary>
        /// <remarks>The search term is matched against multiple properties of each ship-out plan,
        /// including identifiers, customer name, and various dates. The comparison is case-sensitive and checks for
        /// partial matches within the properties.</remarks>
        /// <param name="search">The search term used to filter the ship-out plans. If <see langword="null"/> or empty, the method resets the
        /// collection to its original state.</param>
        private void PerformSearch(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (_allShipouts != null)
                {
                    ShipOutPlanDetails = new ObservableCollection<ShipoutPlan>(_allShipouts);
                    OnPropertyChanged(nameof(ShipOutPlanDetails));
                }
            }
            else
            {
                if (_allShipouts != null)
                {
                    var filtered = _allShipouts.Where(
                    s =>
                    (s.Szam?.Contains(search) ?? false) ||
                    (s.Bizonylatszam?.Contains(search) ?? false) ||
                    (s.CustomerName?.Contains(search) ?? false) ||
                    (s.SapPoNum?.Contains(search) ?? false) ||
                    (s.SapSoNum?.Contains(search) ?? false) ||
                    (s.KiszallitasiDatum.ToString("yyyy-MM-dd").Contains(search)) ||
                    (s.EredetiKertDatum.ToString("yyyy-MM-dd").Contains(search)) ||
                    (s.OrderDate.ToString("yyyy-MM-dd").Contains(search)) ||
                    (s.ETD.Contains(search))
                    ).ToList();

                    ShipOutPlanDetails = new ObservableCollection<ShipoutPlan>(filtered);
                    OnPropertyChanged(nameof(ShipOutPlanDetails));
                }
            }
        }
        /// <summary>
        /// Prepares and organizes data for generating various production and efficiency reports.
        /// </summary>
        /// <remarks>This method processes data related to machine statuses, production plans, and reject
        /// rates to populate multiple charts and collections used for reporting. It calculates metrics such as
        /// cumulative reject ratios, efficiency, and output changes, and organizes data by workcenter type. The
        /// prepared data is used to generate visualizations such as column charts, line charts, and pie charts for
        /// production follow-up and analysis.</remarks>
        public void PrepareReportData()
        {
            /*
             A gépcsoport összefoglalókból csak azok a gépcsoportok kellenek, ami a setuppban be van állítva.
             */

            foreach (var machine in _result.AffectedMachineStatusReportsForProdMeeting)
            {

                machine.ChartData = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (machine.Output),
                    },
                    new LineSeries
                    {
                        Title = "Comulated plan",
                        Values = new ChartValues<int>(machine.ComulatedPlans),
                    },
                };
                machine.XAxisLabel = _xaxisWorkdays;
            }


            //Miden gépcsoportra kigyüjti az followup adatokat.
            var allMachineStatusReports = _result.AllStatusReportForProd.Select(s => s.MachineData).ToList();
            var allKftDailyPlans = _result.AllStatusReportForProd.Select(s => s.DailyPlans).ToList();

            foreach (var machine in allMachineStatusReports)
            {
                _kftRejects?.Add(machine.Select(s => s.Reject).Sum());
                _supplierRejects?.Add(machine.Select(s => s.SupplierReject).Sum());

                if (machine.Where(s => s.AvgRejectRatio > 0).Select(s => s.AvgRejectRatio).ToList().Count() != 0)
                {
                    _kftRejectPercent?.Add(machine.Where(s => s.AvgRejectRatio > 0).Select(s => s.AvgRejectRatio).ToList().Average());
                }
            }


            if (allKftDailyPlans != null)
            {
                foreach (var plan in allKftDailyPlans)
                {
                    if (plan != null)
                    {
                        _kftOutputs?.Add(plan.Select(s => s.ActualQty * 1000).Sum());
                    }
                }
            }

            //_kftActualRejectRatios = _kftRejects.Zip(_kftOutputs, (a, b) => (a, b))
            //    .Where(x => x.b != 0)
            //    .Select(x => x.a / x.b)
            //    .ToList();

            if (_kftRejects != null && _kftActualRejectRatios != null && _kftOutputs != null)
            {
                for (int i = 0; i < _kftRejects.Count; i++)
                {
                    if (_kftRejects[i] == 0 || _kftOutputs[i] == 0) { _kftActualRejectRatios.Add(0); }
                    else { _kftActualRejectRatios.Add(_kftRejects[i] / _kftOutputs[i]); }
                }
            }

            //A grafikonok a sales változáshoz és a terv változáshoz.
            PlanChangesChart = new SeriesCollection
            {
                    new LineSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<decimal> (_result.PlanOutputValues),
                    },

                    new LineSeries
                    {
                        Title = "RM Cost (TTL)",
                        Values = new ChartValues<decimal> (_result.RmCostValues),
                    },
                    new LineSeries
                    {
                        Title = "RM Cost (KFT)",
                        Values = new ChartValues<decimal> (_result.KftRmCostValues),
                    },
                    new LineSeries
                    {
                        Title = "RM Cost (Repack)",
                        Values = new ChartValues<decimal> (_result.RepackRmCostValues),
                    },
            };

            SalesChangesChart = new SeriesCollection
            {
                    new LineSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<decimal> (_result.SalesOutputValues),
                    },
            };

            //A gépcsoport neveinek listába rendezése típus szerint a selejtkimutatásokhoz
            _workcenterNames = new ObservableCollection<string>();
            _workcenterNamesManual = new ObservableCollection<string>();
            _workcenterNamesMachine = new ObservableCollection<string>();
            _workcenterNamesInspection = new ObservableCollection<string>();
            List<int> _workcenterKftRejects = new List<int>();
            List<int> _workcenterSupplierRejects = new List<int>();


            int wordayCount = _result.SelectedStatusReportProd.WokdaysNum;

            int _kftRejectSum = 0;
            int _supplierRejectSum = 0;

            foreach (var workcenter in _result.WorkcenterFollowupData)
            {

                if (workcenter.WorkcenterType == EnumMachineType.InscpectionProcess.ToString())
                {
                    _workcenterNamesInspection.Add(workcenter.Workcenter);
                }

                if (workcenter.WorkcenterType == EnumMachineType.ManualProcess.ToString())
                {
                    _workcenterNamesManual.Add(workcenter.Workcenter);
                }

                if (workcenter.WorkcenterType == EnumMachineType.MachineProcess.ToString())
                {
                    _workcenterNamesMachine.Add(workcenter.Workcenter);
                }

                if (workcenter.Reject != 0 || workcenter.SupplierReject != 0)
                {
                    _workcenterNames.Add(workcenter.Workcenter);
                    _workcenterKftRejects.Add(workcenter.Reject);
                    _workcenterSupplierRejects.Add(workcenter.SupplierReject);
                }
            }

            var target = Enumerable.Repeat((decimal)1, _workcenterNamesManual.Count()).ToList();

            _kftRejectSum = _workcenterKftRejects.Sum();
            _supplierRejectSum = _workcenterSupplierRejects.Sum();

            WorkcenterRejectChart = new SeriesCollection
            {
                    new StackedColumnSeries
                    {
                        Title = "KFT Selejt",
                        Values = new ChartValues<int> (_workcenterKftRejects),
                    },
                    new StackedColumnSeries
                    {
                        Title = "Beszállítói selejt",
                        Values = new ChartValues<int> (_workcenterSupplierRejects),
                    }
            };

            RejectSumPieChart = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "KFT Selejt",
                    Values = new ChartValues<int>{_kftRejectSum },
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Beszállítói selejt",
                    Values = new ChartValues<int>{_supplierRejectSum },
                    DataLabels = true,
                }
            };

            var kftOut = KftDailyPlannedQty.Select(s => s.ActualQty * 1000).Sum();
            if (kftOut == 0)
            {
                _ttlReject = "0";
            }
            else
            {
                _ttlReject = $"{(_kftRejectSum / kftOut).ToString("P2")}";
            }
            OnPropertyChanged(nameof(TtlReject));

            if (kftOut > 0)
            {
                KftCumulativeRejectSumChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Cumulative KFT reject",
                        Values = new ChartValues<decimal>{_kftRejectSum / kftOut },
                        DataLabels = true,
                    }
                };
            }

            RejectChangesChart = new SeriesCollection
            {
                    new LineSeries
                    {
                        Title = "KFT Selejt",
                        Values = new ChartValues<int> (_kftRejects),
                    },
                    new LineSeries
                    {
                        Title = "Beszállítói selejt",
                        Values = new ChartValues<int> (_supplierRejects),
                    },
            };

            var _manualRejectTarget = Enumerable.Repeat((decimal)0.01, _workcenterNamesManual.Count()).ToList();

            ManualRejectSumChart = new SeriesCollection
            {
                    new ColumnSeries
                    {
                        Title = "Kézi folyamat selejt %",
                        Values = new ChartValues<double> (_result.ManualRejectRatio),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<decimal>(_manualRejectTarget),
                    },
            };

            var _machineRejectTarget = Enumerable.Repeat((decimal)0.01, _workcenterNamesMachine.Count()).ToList();

            MachineRejectSumChart = new SeriesCollection
            {
                    new ColumnSeries
                    {
                        Title = "Gépi folyamat selejt %",
                        Values = new ChartValues<double> (_result.MachineRejectRatio),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<decimal>(_machineRejectTarget),
                    },
            };

            var _inspectionRejectTarget = Enumerable.Repeat((decimal)0.01, _workcenterNamesMachine.Count()).ToList();

            InspectionRejectSumChart = new SeriesCollection
            {
                    new ColumnSeries
                    {
                        Title = "Ellenőrzés folyamat selejt %",
                        Values = new ChartValues<double> (_result.InspectionRejectRatio),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<decimal>(_inspectionRejectTarget),
                    },
            };

            var _kftRejectTarget = Enumerable.Repeat((decimal)0.01, allMachineStatusReports.Count()).ToList();

            AllRejectSumChart = new SeriesCollection
            {
                    new ColumnSeries
                    {
                        Title = "Teljes folyamat selejt %",
                        Values = new ChartValues<double> (_kftRejectPercent),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<decimal>(_kftRejectTarget),
                    },

                    new LineSeries
                    {
                        Title = "Cumulated",
                        Values = new ChartValues<decimal>(_kftActualRejectRatios),
                        //Stroke = new SolidColorBrush(Color.FromRgb(227,142,190)),
                        //Fill = new SolidColorBrush(Color.FromRgb(227,142,190))
                    }
            };

            EfficiencyChart = new SeriesCollection
            {
                    new ColumnSeries
                    {
                        Title = "KFT Hatékonyság",
                        Values = new ChartValues<decimal> (_result.KftEfficiency),
                        DataLabels = true,
                    },
                    new ColumnSeries
                    {
                        Title = "Subcon hatékonyság",
                        Values = new ChartValues<decimal> (_result.SubconEfficiency),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<decimal>(target),
                    },
            };
        }

        /// <summary>
        /// Retrieves and processes the follow-up document associated with the current parent entity, populating various
        /// properties with data for reporting and analysis.
        /// </summary>
        /// <remarks>This method initializes and processes data related to production follow-up reports,
        /// including workday details, machine status reports, plan change details, shipment plans, and financial
        /// metrics. The processed data is used to populate properties that can be accessed for further reporting or
        /// display.</remarks>
        private void GetFollowupDocument()
        {
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);

            var doc = databaseCollection.Find(x => x.DocumentName == _parent.Name).FirstOrDefault();

            _xaxisWorkdays = doc.Headcount.Select(w => w.Workday.Day.ToString()).ToList();


            FollowupDocManagement dm = new FollowupDocManagement(doc, _selectedReportName, EnumCallerEntity.PROD);
            _result = dm.GetDataForNestedReportViews();

            StatusReportMahines = _result.AffectedMachineStatusReportsForProdMeeting;

            PlanChangeDetails = _result.PlanChangeDetails;
            ShipOutPlanDetails = _result.ShipoutPlanDetails;
            _allShipouts = new ObservableCollection<ShipoutPlan>(ShipOutPlanDetails);
            WorkcenterData = _result.WorkcenterFollowupData;
            SamplePrice = _result.PlannedSamplePrice;
            PlanPrice = _result.ProdPlanPrice;
            PlanDcPrice = _result.ProdPlanDcPrice;
            MaterialCost = _result.ProdPlanMaterialPrice;
            KftMaterialCost = _result.ProdPlanKftMaterialPrice;
            RepackMaterialCost = _result.ProdPlanRepackMaterialPrice;
            TtlPlan = _result.ProdPlanTotalPrice;
            SalesPlan = _result.ShipoutPlanPrice;
            SalesPlanDc = _result.ShipoutPlanDcPrice;
            DcMovement = _result.DcMovement;
            _statusReportName = _result.SelectedStatusReportProd?.ReportName;

            ReportName = $"Név : {_statusReportName}";
            ReportIssueDate = $"Dátum : {_result.SelectedStatusReportProd?.IssueDate}";
            CurrentWorkday = $"A report a(z) {_result.SelectedStatusReportProd?.ActualWorkday} munkanapon készült.";
            KftProdCompleteRatio = $"{_result.SelectedStatusReportProd?.ProdCompleteRatio}";
            KftTimePropRatio = $"{_result.SelectedStatusReportProd?.ProdTimePropRatio}";
            RepackProdCompleteRatio = $"{_result.SelectedStatusReportProd?.RepackProdCompleteRatio}";
            RepackTimePropRatio = $"{_result.SelectedStatusReportProd?.RepackProdTimePropRatio}";

            RepackDailyPlannedQty = _result.RepackDailyPlannedQty;
            KftDailyPlannedQty = _result.KftDailyPlannedQty;
        }
        #endregion
    }
}
