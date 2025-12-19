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
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace ProdInfoSys.ViewModels.Nested
{
    /// <summary>
    /// Represents the view model for the QRQC status report, providing properties, commands, and data structures for
    /// displaying and exporting production, reject, and efficiency metrics in a WPF application.
    /// </summary>
    /// <remarks>This view model is designed for use in WPF applications that visualize and manage QRQC (Quick
    /// Response Quality Control) status reports. It exposes collections and properties for binding to charts, tables,
    /// and other UI elements, and implements the INotifyPropertyChanged interface to support dynamic updates. The class
    /// includes commands for exporting report data to Excel and methods for initializing and preparing report data from
    /// underlying data sources. Thread safety is not guaranteed; all members are intended to be accessed from the UI
    /// thread.</remarks>
    public class StatusReportQrqcViewModel : INotifyPropertyChanged
    {
        private TreeNodeModel? _parent;
        private List<string> _xaxisWorkdays = new List<string>();
        private FollowupMetadata? _result = new FollowupMetadata();
        private string? _selectedReportName;
        private List<double>? _kftRejectPercent = new();
        private List<int>? _kftRejects = new();
        private List<int>? _supplierRejects = new();
        private List<decimal>? _kftOutputs = new();
        private List<decimal>? _kftActualRejectRatios = new();
        private ObservableCollection<StatusReportQrqc>? _allStatusReport;
        private decimal _kftOut = 0;

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by the implementation of the INotifyPropertyChanged
        /// interface to notify subscribers that a property value has changed. Handlers attached to this event receive
        /// the name of the property that changed in the PropertyChangedEventArgs parameter.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in the setter of a property to notify subscribers that the
        /// property's value has changed. This is commonly used to implement the INotifyPropertyChanged interface in
        /// data-binding scenarios.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and is automatically provided when called from
        /// a property setter.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF communication
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

        private string[] _labelsForStopTime;
        public string[] LabelsForStopTime
        {
            get => _labelsForStopTime;
            set { _labelsForStopTime = value; OnPropertyChanged(); }
        }

        private StatusReportQrqc? _statusReport;
        public StatusReportQrqc? StatusReport
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

        private ObservableCollection<StopTime>? _stopTimes;
        public ObservableCollection<StopTime>? StopTimes
        {
            get => _stopTimes;
            set { _stopTimes = value; OnPropertyChanged(); }
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

        private SeriesCollection? _kftCumulativeRejectSumChart;
        public SeriesCollection? KftCumulativeRejectSumChart
        {
            get => _kftCumulativeRejectSumChart;
            set { _kftCumulativeRejectSumChart = value; OnPropertyChanged(); }
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

        private SeriesCollection? _stopTimeChart;
        public SeriesCollection? StopTimeChart
        {
            get => _stopTimeChart;
            set { _stopTimeChart = value; OnPropertyChanged(); }
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
        private string? _ttlPlan;
        public string? TtlPlan
        {
            get => _ttlPlan;
            set { _ttlPlan = value; OnPropertyChanged(); }
        }

        private string? _ttlReject;
        public string? TtlReject
        {
            get => _ttlReject;
            set { _ttlReject = value; OnPropertyChanged(); }
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
        public ICommand? ExportPlanChanges => new ProjectCommandRelay(_ => ExportingPlanChanges());
        /// <summary>
        /// Exports the current plan change details to an Excel file.
        /// </summary>
        /// <remarks>If an error occurs during the export process, a message box is displayed with the
        /// error details.</remarks>
        private void ExportingPlanChanges()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                //e.ExportHeadcountFollowup(_headcountFollowupDocs);
                e.ExcelExport(_planChangeDetails, $"Plan changes", $"Plan changes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Excel export", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public ICommand? ExportKftStatus => new ProjectCommandRelay(_ => ExportingKftStatus());
        /// <summary>
        /// Exports the current KFT status data to an Excel file.
        /// </summary>
        /// <remarks>If an error occurs during the export process, a message box is displayed with the
        /// error details. The export includes the daily planned quantity data for KFT.</remarks>
        private void ExportingKftStatus()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                e.ExcelExport(_kftDailyPlannedQty, $"KFT Status", $"KFT Status");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Excel export", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
        public StatusReportQrqcViewModel()
        {
            RepackDailyPlannedQty = new();
            KftDailyPlannedQty = new();
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
        /// Gets a function that formats Y-axis values as percentage strings with two decimal places.
        /// </summary>
        public Func<double, string> YFormatter => value => value.ToString("P2");

        /// <summary>
        /// Gets a function that formats a numeric Y-axis value as a string with two decimal places.
        /// </summary>
        public Func<double, string> YFormatterNum => value => value.ToString("N2");
        #endregion

        #region Private methods
        /// <summary>
        /// Retrieves and processes the follow-up document data based on the selected report name and parent entity.
        /// </summary>
        /// <remarks>This method initializes and populates various properties with data extracted from the
        /// follow-up document. It retrieves the document from the database, processes it using the selected report
        /// name, and calculates key metrics and details for nested report views. The method assumes that both the
        /// parent entity and the selected report name are not null.</remarks>
        private void GetFollowupDocument()
        {
            try
            {
                if (_parent != null && _selectedReportName != null)
                {
                    ConnectionManagement conMgmnt = new ConnectionManagement();
                    var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);
                    var doc = databaseCollection.Find(x => x.DocumentName == _parent.Name).FirstOrDefault();
                    _xaxisWorkdays = doc.Headcount.Select(w => w.Workday.Day.ToString()).ToList();
                    FollowupDocManagement dm = new FollowupDocManagement(doc, _selectedReportName, EnumCallerEntity.QRQC);
                    _result = dm.GetDataForNestedReportViews();

                    _allStatusReport = _result.AllStatusReportForQrqc;
                    StatusReport = _result.SelectedStatusReportQrqc;

                    StatusReportMahines = _result.AffectedMachineStatusReportsForQrqcMeeting;

                    PlanChangeDetails = _result.PlanChangeDetails;
                    WorkcenterData = _result.WorkcenterFollowupData;
                    SamplePrice = _result.PlannedSamplePrice;
                    PlanPrice = _result.ProdPlanPrice;
                    PlanDcPrice = _result.ProdPlanDcPrice;
                    MaterialCost = _result.ProdPlanMaterialPrice;
                    TtlPlan = _result.ProdPlanTotalPrice;

                    ReportName = $"Név : {StatusReport?.ReportName ?? string.Empty}";
                    ReportIssueDate = $"Dátum : {StatusReport?.IssueDate}";
                    CurrentWorkday = $"A report a(z) {StatusReport?.ActualWorkday} munkanapon készült.";
                    KftProdCompleteRatio = $"{StatusReport?.KftProdCompleteRatio}";
                    KftTimePropRatio = $"{StatusReport?.KftProdTimePropRatio}";
                    RepackProdCompleteRatio = $"{StatusReport?.RepackProdCompleteRatio}";
                    RepackTimePropRatio = $"{StatusReport?.RepackProdTimePropRatio}";

                    RepackDailyPlannedQty = _result.RepackDailyPlannedQty;
                    KftDailyPlannedQty = _result.KftDailyPlannedQty;
                    StopTimes = _result.StopTimes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Followup betöltés", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Prepares and organizes data for generating various charts and reports related to machine and workcenter
        /// performance.
        /// </summary>
        /// <remarks>This method processes data from multiple sources, including machine status reports,
        /// workcenter data, and daily plans, to populate a variety of chart collections. These charts include bar
        /// charts, line charts, pie charts, and stacked column charts that visualize metrics such as output, reject
        /// rates, efficiency, and stop times.  The method ensures that data is aggregated, calculated, and formatted
        /// appropriately for each chart type. It also categorizes workcenters based on their types (e.g., manual,
        /// machine, inspection) and calculates cumulative metrics such as reject ratios and efficiency targets.  This
        /// method assumes that the required input data (e.g., machine status reports, workcenter data) is not null and
        /// is properly initialized.</remarks>
        public void PrepareReportData()
        {
            if (_result.MachineStatusReports != null && _allStatusReport != null && WorkcenterData != null)
            {
                foreach (var machine in _result.MachineStatusReports)
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

                var allMachineStatusReport = _result.AllStatusReportForQrqc.Select(s => s.MachineData).ToList();
                var allKftDailyPlans = _result.AllStatusReportForQrqc.Select(s => s.DailyPlans).ToList();

                foreach (var machine in allMachineStatusReport)
                {
                    //Az előző reportokban lévő selejtek listája
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

                _workcenterNames = new ObservableCollection<string>();
                _workcenterNamesManual = new ObservableCollection<string>();
                _workcenterNamesMachine = new ObservableCollection<string>();
                _workcenterNamesInspection = new ObservableCollection<string>();
                List<int> _workcenterKftRejects = new List<int>();
                List<int> _workcenterSupplierRejects = new List<int>();
                int wordayCount = StatusReport?.WokdaysNum ?? 0;

                int _kftRejectSum = 0;
                int _supplierRejectSum = 0;

                foreach (var workcenter in WorkcenterData)
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

                _workcenterNamesManual.Add("AVG");

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

                _kftOut = KftDailyPlannedQty.Select(s => s.ActualQty * 1000).Sum();
                _ttlReject = $"{(_kftRejectSum / _kftOut).ToString("P2")}";
                OnPropertyChanged(nameof(TtlReject));
                KftCumulativeRejectSumChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Cumulative KFT reject",
                        Values = new ChartValues<decimal>{_kftRejectSum / _kftOut },
                        DataLabels = true,
                    }
                };


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

                var _kftRejectTarget = Enumerable.Repeat((decimal)0.01, allMachineStatusReport.Count()).ToList();

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

                //TODO: Miért van itt?? Setupból megy.
                if (_stopTimes != null)
                {
                    StopTimeChart = new SeriesCollection();
                    List<string> workcenters = new List<string> { "CYT", "AD AUTOMATA PUNCHING", "APIM", "SS AUTOMATA BENDING" };
                    List<string> stopCodes = new List<string> { "GEPHIBA" };
                    LabelsForStopTime = workcenters.ToArray();
                    //var stopCodes = _stopTimes.Select(g => g.StopCode).Distinct();
                    foreach (var sc in stopCodes)
                    {
                        var values = new ChartValues<decimal>(
                            workcenters.Select(wc =>
                            _stopTimes
                                .Where(g => g.Workcenter == wc && g.StopCode == sc)
                                .Select(g => g.TtlStopTime)
                                .DefaultIfEmpty(0)
                                .First()
                        ));

                        StopTimeChart.Add(new ColumnSeries
                        {
                            Title = sc,  // gépcsoport neve
                            Values = values,
                            DataLabels = true,

                        });

                    }
                }
            }
        }
        #endregion
    }
}
