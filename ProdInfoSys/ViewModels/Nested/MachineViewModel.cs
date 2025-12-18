using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Models;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.NonRelationalModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProdInfoSys.ViewModels.Nested
{
    /// <summary>
    /// Represents the view model for managing machine follow-up data, charts, and related operations in a production or
    /// manufacturing context.
    /// </summary>
    /// <remarks>The MachineViewModel coordinates data binding, user commands, and chart visualization for
    /// machine follow-up documents within a WPF application. It provides properties for displaying and editing machine
    /// data, exposes commands for importing, saving, and exporting documents, and manages chart data for output,
    /// efficiency, and reject rates. The class implements INotifyPropertyChanged to support property change
    /// notifications for UI updates. Thread safety is not guaranteed; interactions should occur on the UI thread as
    /// required by WPF data binding.</remarks>
    public class MachineViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private readonly IUserDialogService _dialogs;
        private IUserControlFunctions _userControlFunctions;
        private IConnectionManagement _connectionManagement;
        #endregion

        private TreeNodeModel? _parent;
        private string _selectedWorkcenter = string.Empty;
        private MasterFollowupDocument? _followupDocument;
        private List<string> _notInvolvedScrapCodes = new() { "E", "TST" };
        public Action? ForceCommit { get; set; }

        #region Charts
        public List<string>? Labels { get; set; }
        #endregion

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by calling the OnPropertyChanged method in property
        /// setters to notify listeners that a property value has changed. It is commonly used to support data binding
        /// scenarios in applications such as WPF, UWP, or Xamarin.Forms.</remarks>
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

        #region WPF Communication
        private string? _mustSave;
        public string? MustSave
        {
            get => _mustSave;
            set { _mustSave = value; OnPropertyChanged(); }
        }

        private bool _is2ShiftVisible;
        public bool Is2ShiftVisible
        {
            get => _is2ShiftVisible;
            set { _is2ShiftVisible = value; OnPropertyChanged(); }
        }

        private bool _is3ShiftVisible;
        public bool Is3ShiftVisible
        {
            get => _is3ShiftVisible;
            set { _is3ShiftVisible = value; OnPropertyChanged(); }
        }

        private double _fontSize;
        public double FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        private DateTime _extraWorkday;
        public DateTime ExtraWorkday
        {
            get => _extraWorkday;
            set { _extraWorkday = value; OnPropertyChanged(); }
        }

        private MachineFollowupDocument? _selectedDocument;
        public MachineFollowupDocument? SelectedDocument
        {
            get => _selectedDocument;
            set { _selectedDocument = value; OnPropertyChanged(); }
        }

        private ObservableCollection<MachineFollowupDocument>? _machineFollowupDocs;
        public ObservableCollection<MachineFollowupDocument>? MachineFollowupDocs
        {
            get => _machineFollowupDocs;
            set { _machineFollowupDocs = value; OnPropertyChanged(); }
        }

        private string? _workcenterName;
        public string? WorkcenterName
        {
            get => _workcenterName;
            set { _workcenterName = value; OnPropertyChanged(); }
        }

        private string? _workdays;
        public string? Workdays
        {
            get => _workdays;
            set { _workdays = value; OnPropertyChanged(); }
        }

        private string? _comulatedOut;
        public string? ComulatedOut
        {
            get => _comulatedOut;
            set { _comulatedOut = value; OnPropertyChanged(); }
        }
        private SeriesCollection? _outputChart;
        public SeriesCollection? OutputChart
        {
            get => _outputChart;
            set { _outputChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _workTimeSeriesCollection;
        public SeriesCollection? WorkTimeSeriesCollection
        {
            get => _workTimeSeriesCollection;
            set { _workTimeSeriesCollection = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _rejectChart;
        public SeriesCollection? RejectChart
        {
            get => _rejectChart;
            set { _rejectChart = value; OnPropertyChanged(); }
        }
        private SeriesCollection? _rejectPieChart;
        public SeriesCollection? RejectPieChart
        {
            get => _rejectPieChart;
            set { _rejectPieChart = value; OnPropertyChanged(); }
        }

        private double _avgUtil;
        public double AvgUtil
        {
            get => _avgUtil;
            set { _avgUtil = value; OnPropertyChanged(); }
        }


        private string? _difference;
        public string? Difference
        {
            get => _difference;
            set { _difference = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _comOutChart;
        public SeriesCollection? ComOutChart
        {
            get => _comOutChart;
            set { _comOutChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _efficiencyChart;
        public SeriesCollection? EfficiencyChart
        {
            get => _efficiencyChart;
            set { _efficiencyChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _rejectPercentChart;
        public SeriesCollection? RejectPercentChart
        {
            get => _rejectPercentChart;
            set { _rejectPercentChart = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand ImportFromErp => new ProjectCommandRelay(async _ => await ImportingFromErp());
        /// <summary>
        /// Imports production and capacity data from the ERP system and updates the selected machine follow-up document
        /// with the retrieved values.
        /// </summary>
        /// <remarks>This method updates shift outputs, rejects, and other production metrics for the
        /// selected document based on data retrieved from the ERP system. The method does not perform any action if no
        /// document is selected.</remarks>
        /// <returns>A task that represents the asynchronous import operation.</returns>
        private async Task ImportingFromErp()
        {
            if (_selectedDocument != null)
            {
                DataExchangeManagement dem = new DataExchangeManagement();
                var capacityTask = dem.GetExtCapacityLedgerEntries(_selectedWorkcenter, _selectedDocument.Workday.ToString("yyyy-MM-dd"));
                var capacity = await capacityTask;

                var actualDocument = _machineFollowupDocs?.FirstOrDefault(s => s.Workday == _selectedDocument.Workday);

                if (capacity != null && actualDocument != null)
                {
                    actualDocument.Shift1Output = (int)capacity.Where(s => s.ShiftCode == "1").Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift2Output = (int)capacity.Where(s => s.ShiftCode == "2").Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift3Output = (int)capacity.Where(s => s.ShiftCode == "3").Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift1Reject = (int)capacity.Where(s => s.ShiftCode == "1" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.Shift2Reject = (int)capacity.Where(s => s.ShiftCode == "2" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift3Reject = (int)capacity.Where(s => s.ShiftCode == "3" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.OutputQuantity * 1000);
                    actualDocument.SupplierReject = (int)capacity.Where(s => s.ScrapCode == "E").Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.TST = (int)capacity.Where(s => s.ScrapCode == "TST").Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.NK = (int)capacity.Where(s => s.ScrapCode == "NK").Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.SetupReject = (int)capacity.Where(s => s.ScrapCode == "BS").Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.OperatingHour = (double)capacity.Sum(s => s.RunTime);

                    if (capacity.Where(s => s.Perf != 0).Count() != 0)
                    {
                        actualDocument.Efficiency = capacity.Where(s => s.Perf != 0).Average(s => s.Perf);
                    }
                }
                OnPropertyChanged(nameof(MachineFollowupDocs));
                UpdateMainDocument();
                SavingDocument(false);
            }
        }
        public ICommand AddExtraWorkday => new ProjectCommandRelay(_ => AddingExtraWorkday());
        /// <summary>
        /// Updates the collection of machine follow-up documents to include an additional workday and refreshes related
        /// state.
        /// </summary>
        /// <remarks>This method recalculates the machine follow-up documents by adding an extra workday,
        /// reorders the collection, and updates property change notifications. It should be called when an extra
        /// workday needs to be reflected in the machine follow-up data.</remarks>
        private void AddingExtraWorkday()
        {
            if (_machineFollowupDocs != null && MachineFollowupDocs != null)
            {
                UserControlFunctions uf = new UserControlFunctions(_dialogs);

                MachineFollowupDocs = new ObservableCollection<MachineFollowupDocument>(uf.AddExtraWorkdayMachine(_machineFollowupDocs, _extraWorkday).ToList());
                MachineFollowupDocs = new ObservableCollection<MachineFollowupDocument>(MachineFollowupDocs.OrderBy(Workdays => Workdays.Workday).ToList());
                foreach (var i in MachineFollowupDocs) { i.PropertyChanged += MachineItem_PropertyChanged; }

                UpdateMainDocument();
                OnPropertyChanged(nameof(MachineFollowupDocs));
                SavingDocument(false);
            }
        }

        public ICommand? SaveDocument => new ProjectCommandRelay(_ => SavingDocument());
        /// <summary>
        /// Saves the current follow-up document to the database and updates related state. Optionally displays a
        /// confirmation dialog indicating the result of the save operation.
        /// </summary>
        /// <remarks>This method updates related properties and triggers UI notifications after saving the
        /// document. If no follow-up document is present, the method performs no action.</remarks>
        /// <param name="isConfirm">true to display a confirmation dialog after attempting to save the document; otherwise, false. The default
        /// is true.</param>
        private void SavingDocument(bool isConfirm = true)
        {
            if (_followupDocument != null)
            {
                OnPropertyChanged(nameof(MachineFollowupDocs));
                ForceCommit?.Invoke();
                UpdateComulatedData();
                UpdateMainDocument();
                var ret = _userControlFunctions.SaveDocumentToDatabase(_connectionManagement, _followupDocument);
                if (ret.isCompleted)
                {
                    SetChart();
                    GetComulatedOut();
                    GetDifference();
                    _mustSave = string.Empty;
                    OnPropertyChanged(nameof(MustSave));
                    if (isConfirm) { _dialogs.ShowInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikeres!", "InspectionViewModel"); }

                }
                else
                {
                    if (isConfirm) { _dialogs.ShowInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikertelen a következő hiba miatt : {ret.message}", "InspectionViewModel"); }
                }
            }
        }

        public ICommand? DeleteSelected => new ProjectCommandRelay(_ => DeletingSelected());
        /// <summary>
        /// Deletes the currently selected document after user confirmation.
        /// </summary>
        /// <remarks>This method prompts the user for confirmation before removing the selected document
        /// from the collection. If the user confirms, the document is deleted and related updates are performed. No
        /// action is taken if no document is selected.</remarks>
        private void DeletingSelected()
        {
            if (_selectedDocument != null && MachineFollowupDocs != null && SelectedDocument != null)
            {
                if (_dialogs.ShowConfirmation($"Tényleg törölni szeretnéd a kijelölt ({_selectedDocument.Workday}) munkanapot? ", "Törlés"))
                {
                    MachineFollowupDocs.Remove(SelectedDocument);
                    UpdateMainDocument();
                    SavingDocument(false);
                }
            }
        }

        public ICommand? ExportExcel => new ProjectCommandRelay(_ => ExportingExcel());
        /// <summary>
        /// Exports the current machine follow-up documents to an Excel file using the selected workcenter as the file
        /// name and worksheet name.
        /// </summary>
        /// <remarks>If an error occurs during the export process, an error dialog is displayed with
        /// details about the failure.</remarks>
        private void ExportingExcel()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                e.ExcelExport(_machineFollowupDocs, $"{_selectedWorkcenter} followup", $"{_selectedWorkcenter} followup");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Excel export");
            }
        }

        #endregion

        #region Constructor
        public MachineViewModel(IUserDialogService dialogs,
            IUserControlFunctions userControlFunctions,
            IConnectionManagement connectionManagement
            )
        {
            _dialogs = dialogs;
            _userControlFunctions = userControlFunctions;
            _connectionManagement = connectionManagement;
            _extraWorkday = DateTime.Now;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes the current node with the specified parent node and workcenter identifier.
        /// </summary>
        /// <param name="parent">The parent node to associate with the current node. Cannot be null.</param>
        /// <param name="workcenter">The identifier of the workcenter to assign to the current node. Cannot be null or empty.</param>
        public void Init(TreeNodeModel parent, string workcenter)
        {
            _parent = parent;
            _selectedWorkcenter = workcenter;
            _workcenterName = workcenter;
            GetFollowupDocument();
            _workdays = $"Munkanapok : {_followupDocument?.Workdays ?? 0}";
            GetComulatedOut();
            GetDifference();
            SetChart();
            SetVisibility();
        }

        /// <summary>
        /// Gets a function that formats a numeric value for display on a gauge.
        /// </summary>
        public Func<double, string> GaugeFormatter => val => val.ToString("N2");

        /// <summary>
        /// Gets a function that formats Y-axis values as percentage strings with two decimal places.
        /// </summary>
        public Func<double, string> YFormatter => value => value.ToString("P2");

        #endregion

        #region Private methods
        /// <summary>
        /// Updates the visibility state of shift-related UI elements and font size based on the current follow-up
        /// document.
        /// </summary>
        /// <remarks>This method should be called whenever the follow-up document changes to ensure that
        /// the UI reflects the correct shift visibility and font size settings. It raises property change notifications
        /// for the affected properties.</remarks>
        private void SetVisibility()
        {
            if (_followupDocument != null)
            {
                switch (_followupDocument.ShiftNumMachine)
                {
                    case 2: _is2ShiftVisible = true; _is3ShiftVisible = false; break;
                    case 3: _is2ShiftVisible = true; _is3ShiftVisible = true; break;
                }

                _fontSize = (double)RegistryManagement.ReadIntRegistryKey("FontSize");
                OnPropertyChanged(nameof(FontSize));
                OnPropertyChanged(nameof(Is2ShiftVisible));
                OnPropertyChanged(nameof(Is3ShiftVisible));
            }
        }
        /// <summary>
        /// Updates the main document by synchronizing machine follow-up documents for the selected workcenter.
        /// </summary>
        /// <remarks>This method checks if the follow-up document and machine follow-up documents are
        /// available. If a matching machine follow-up entry for the selected workcenter is found, its associated
        /// machine follow-up documents are updated.</remarks>
        private void UpdateMainDocument()
        {
            if (_followupDocument != null && MachineFollowupDocs != null)
            {
                var target = _followupDocument.MachineFollowups.FirstOrDefault(x => x.Workcenter == _selectedWorkcenter);
                if (target != null)
                {
                    target.MachineFollowupDocuments = MachineFollowupDocs.ToList();
                }
            }
        }
        /// <summary>
        /// Updates the cumulative output value based on the current machine follow-up documents.
        /// </summary>
        /// <remarks>This method calculates the cumulative output using the provided machine follow-up
        /// documents and updates the associated property. If the machine follow-up documents are null, no update is
        /// performed.</remarks>
        private void GetComulatedOut()
        {
            if (_machineFollowupDocs != null)
            {
                //_comulatedOut = $"Komulált kimenet : {_machineFollowupDocs.Select(x => x.OutputSum).Sum()}";
                _comulatedOut = $"Kumulált kimenet : {_userControlFunctions.GetComulatedOut(_machineFollowupDocs)}";
                OnPropertyChanged(nameof(ComulatedOut));
            }
        }
        /// <summary>
        /// Calculates the difference between the daily output sum and the cumulative plan for machine follow-up
        /// documents.
        /// </summary>
        /// <remarks>This method updates the <see cref="Difference"/> property with the calculated
        /// difference. If the daily output sum is zero, the cumulative plan is retrieved from the first document.
        /// Otherwise, the cumulative plan is retrieved from the last document where the total output is greater than
        /// zero. After the calculation, the <see cref="OnPropertyChanged"/> method is invoked to notify property
        /// changes.</remarks>
        private void GetDifference()
        {
            if (_machineFollowupDocs != null)
            {
                var dailyOutSum = _machineFollowupDocs.Select(x => x.OutputSum).Sum();
                if (dailyOutSum == 0)
                {
                    var actualComulatedSum = _machineFollowupDocs.Select(x => x.ComulatedPlan).FirstOrDefault();
                    _difference = $"Diff.: {dailyOutSum - actualComulatedSum}";
                }
                else
                {
                    //var list = _machineFollowupDocs.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                    //var actualComulatedSum = _machineFollowupDocs.Where(x => x.TTLOutput > 0).Select(x => x.DailyPlan).Sum();

                    var actualComulatedSum = _machineFollowupDocs.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                    _difference = $"Diff.: {dailyOutSum - actualComulatedSum}";
                }
                OnPropertyChanged(nameof(Difference));
            }
        }
        /// <summary>
        /// Retrieves the follow-up document associated with the current parent object and selected workcenter.
        /// </summary>
        /// <remarks>This method fetches the follow-up document from the database based on the parent's
        /// name and initializes  the collection of machine follow-up documents filtered by the selected workcenter. It
        /// also subscribes  to property change events for each item in the machine follow-up documents
        /// collection.</remarks>
        private void GetFollowupDocument()
        {
            var databaseCollection = _connectionManagement.GetCollection<MasterFollowupDocument>(_connectionManagement.DbName);
            if (_parent != null)
            {
                var doc = databaseCollection.Find(x => x.DocumentName == _parent.Name).FirstOrDefault();
                _followupDocument = doc; //A módosításhoz ell kell tárolni a FollowupDocument eredetijét a Save-ben használjuk            
                _machineFollowupDocs = new ObservableCollection<MachineFollowupDocument>(doc.MachineFollowups.Where(x => x.Workcenter == _selectedWorkcenter).Select(s => s.MachineFollowupDocuments).FirstOrDefault().ToList());

                // a dokumentumban lévő összes elemre feliratkoztatjuk a HeadcountItem_PropertyChanged eventet
                foreach (var item in _machineFollowupDocs)
                {
                    item.PropertyChanged += MachineItem_PropertyChanged;
                }

                OnPropertyChanged(nameof(MachineFollowupDocs));
            }

        }
        /// <summary>
        /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for a machine item.
        /// </summary>
        /// <remarks>This method responds to changes in specific properties of the <see
        /// cref="MachineFollowupDocument"/> class. When certain properties are updated, it triggers calculations and
        /// updates related to cumulative data, output differences, and other relevant metrics.</remarks>
        /// <param name="sender">The source of the property change event. Typically, this is the machine item that raised the event.</param>
        /// <param name="e">An instance of <see cref="PropertyChangedEventArgs"/> containing the name of the property that changed.</param>
        private void MachineItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(MachineFollowupDocument.DailyPlan))
            {
                UpdateComulatedData();
                GetComulatedOut();
                GetDifference();
                //SetChart();
            }
            if (
                e.PropertyName == nameof(MachineFollowupDocument.Shift1Output) ||
                e.PropertyName == nameof(MachineFollowupDocument.Shift2Output) ||
                e.PropertyName == nameof(MachineFollowupDocument.Shift3Output) ||
                e.PropertyName == nameof(MachineFollowupDocument.Shift1Reject) ||
                e.PropertyName == nameof(MachineFollowupDocument.Shift2Reject) ||
                e.PropertyName == nameof(MachineFollowupDocument.Shift3Reject) ||
                e.PropertyName == nameof(MachineFollowupDocument.SupplierReject) ||
                e.PropertyName == nameof(MachineFollowupDocument.TO) ||
                e.PropertyName == nameof(MachineFollowupDocument.MAA) ||
                e.PropertyName == nameof(MachineFollowupDocument.TST) ||
                e.PropertyName == nameof(MachineFollowupDocument.NK) ||
                e.PropertyName == nameof(MachineFollowupDocument.CommentForRejects)
                )
            {
                _mustSave = "Nem mentett dokumentum";
                OnPropertyChanged(nameof(MustSave));
                GetComulatedOut();
                GetDifference();
                //SetChart();
            }
        }
        /// <summary>
        /// Configures and initializes various chart collections based on machine follow-up data.
        /// </summary>
        /// <remarks>This method populates multiple chart series collections, including output charts,
        /// reject charts,  efficiency charts, and others, using data from the machine follow-up documents. The charts
        /// are  designed to visualize metrics such as daily output, cumulative output, reject ratios, operating hours, 
        /// and efficiency. If the machine follow-up data is null, no charts are configured.</remarks>
        private void SetChart()
        {
            if (_machineFollowupDocs != null)
            {
                Labels = _machineFollowupDocs.Select(c => c.Workday.Day.ToString()).ToList();
                OnPropertyChanged(nameof(Labels));
                int wordayCount = _machineFollowupDocs.Select(c => c.Workday).ToList().Count();
                var target = Enumerable.Repeat(1, wordayCount).ToList();
                OutputChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (_machineFollowupDocs.Select(x => x.OutputSum).ToList()),
                        DataLabels = true,
                        LabelsPosition = BarLabelPosition.Top
                    },
                    new LineSeries
                    {
                        Title = "Dail plan",
                        Values = new ChartValues<int>(_machineFollowupDocs.Select(x => x.DailyPlan))
                    },
                };

                ComOutChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (_machineFollowupDocs.Select(x => x.ComulatedOutput).ToList()),
                        DataLabels = true,
                    },
                    new LineSeries
                    {
                        Title = "Comulated plan",
                        Values = new ChartValues<int>(_machineFollowupDocs.Select(x => x.ComulatedPlan)),
                    },
                };

                RejectChart = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Reject",
                        Values = new ChartValues<int>(_machineFollowupDocs.Select(x => x.RejectSum).ToList()),
                        DataLabels = true,
                        //PointGeometrySize = 10
                    },

                };

                RejectPercentChart = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Reject",
                        Values = new ChartValues<double>(_machineFollowupDocs.Select(x => x.CalcRejectRatio).ToList()),
                        //PointGeometrySize = 10
                        DataLabels = true,
                    },
                };

                RejectPieChart = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Reject",
                        Values = new ChartValues<int>{_machineFollowupDocs.Select(x => x.RejectSum).Sum() },
                        DataLabels = true
                    },

                    new PieSeries
                    {
                        Title = "Supplier reject",
                        Values = new ChartValues<int>{_machineFollowupDocs.Select(x => x.SupplierReject).Sum() },
                        DataLabels = true,
                    }
                };

                WorkTimeSeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Actual WH",
                        Values = new ChartValues<double>( _machineFollowupDocs.Select(x => x.OperatingHour).ToList() ),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Avail. WH",
                        Values = new ChartValues<double>(_machineFollowupDocs.Select(x => x.AvailOperatingHour).ToList() ),
                        DataLabels = false,
                    }
                };

                EfficiencyChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Tény",
                        Values = new ChartValues<decimal> (_machineFollowupDocs.Select(x => x.Efficiency).ToList()),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<int>(target),
                    },
                };

                if (_machineFollowupDocs.Where(x => x.PersistedUtilization > 0).Select(x => x.PersistedUtilization * 100).ToList().Count() > 0)
                {
                    AvgUtil = _machineFollowupDocs.Where(x => x.PersistedUtilization > 0).Select(x => x.PersistedUtilization * 100).Average();
                }
            }
        }
        /// <summary>
        /// Updates the cumulative plan and output quantities for each day in the machine follow-up documents.
        /// </summary>
        /// <remarks>This method iterates through the collection of machine follow-up documents and
        /// calculates the cumulative plan and output quantities based on the daily plan and total output values for
        /// each day. After updating the values, it raises a property change notification for the <see
        /// cref="MachineFollowupDocs"/> property.</remarks>
        private void UpdateComulatedData()
        {
            int currentComulatedQty = 0;
            int currentComulatedOutputQty = 0;
            foreach (var day in MachineFollowupDocs)
            {
                day.ComulatedPlan = currentComulatedQty + day.DailyPlan;
                day.ComulatedOutput = currentComulatedOutputQty + day.TTLOutput;
                currentComulatedQty = day.ComulatedPlan;
                currentComulatedOutputQty = day.ComulatedOutput;
            }

            OnPropertyChanged(nameof(MachineFollowupDocs));
        }
        #endregion
    }
}
