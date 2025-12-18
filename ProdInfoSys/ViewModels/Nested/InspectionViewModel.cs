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
    /// Represents the view model for managing inspection follow-up documents, charts, and related operations in a
    /// workcenter inspection workflow.
    /// </summary>
    /// <remarks>The InspectionViewModel provides properties and commands for interacting with inspection
    /// data, including importing from ERP, adding or deleting workdays, saving documents, and exporting to Excel. It
    /// supports data binding for WPF applications and implements INotifyPropertyChanged to notify the UI of property
    /// changes. Chart data and formatting functions are exposed for visualization of inspection metrics. This view
    /// model is intended to be used as part of an MVVM architecture in applications that require tracking and analysis
    /// of inspection follow-up data.</remarks>
    public class InspectionViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
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
        /// <remarks>This event is typically raised by calling the OnPropertyChanged method in a property
        /// setter. It is used to notify subscribers, such as data-binding clients, that a property value has
        /// changed.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in a property's setter to notify subscribers that the property's
        /// value has changed. This is commonly used to implement the INotifyPropertyChanged interface in data-binding
        /// scenarios.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and will be automatically set to the caller
        /// member name if not specified.</param>
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

        private InspectionFollowupDocument? _selectedDocument;
        public InspectionFollowupDocument? SelectedDocument
        {
            get => _selectedDocument;
            set { _selectedDocument = value; OnPropertyChanged(); }
        }

        private string? _workcenterName;
        public string? WorkcenterName
        {
            get => _workcenterName;
            set { _workcenterName = value; OnPropertyChanged(); }
        }

        private string? _comulatedOut;
        public string? ComulatedOut
        {
            get => _comulatedOut;
            set { _comulatedOut = value; OnPropertyChanged(); }
        }

        private string? _workdays;
        public string? Workdays
        {
            get => _workdays;
            set { _workdays = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _outputChart;
        public SeriesCollection? OutputChart
        {
            get => _outputChart;
            set { _outputChart = value; OnPropertyChanged(); }
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

        private ObservableCollection<InspectionFollowupDocument>? _inspectionFollowupDocs;
        public ObservableCollection<InspectionFollowupDocument>? InspectionFollowupDocuments
        {
            get => _inspectionFollowupDocs;
            set { _inspectionFollowupDocs = value; OnPropertyChanged(); }
        }
        private string? _difference;
        public string? Difference
        {
            get => _difference;
            set { _difference = value; OnPropertyChanged(); }
        }
        private double _avgUtil;
        public double AvgUtil
        {
            get => _avgUtil;
            set { _avgUtil = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _workTimeSeriesCollection;
        public SeriesCollection? WorkTimeSeriesCollection
        {
            get => _workTimeSeriesCollection;
            set { _workTimeSeriesCollection = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _comOutChart;
        public SeriesCollection? ComOutChart
        {
            get => _comOutChart;
            set { _comOutChart = value; OnPropertyChanged(); }
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
        /// Imports production and rejection data from the ERP system for the selected document and updates the
        /// corresponding inspection follow-up records.
        /// </summary>
        /// <remarks>This method updates shift output, rejection, supplier rejection, and operating hour
        /// values for the inspection follow-up document that matches the selected workday. After importing data, it
        /// notifies property changes and persists the updated document. No action is taken if no document is
        /// selected.</remarks>
        /// <returns>A task that represents the asynchronous import operation.</returns>
        private async Task ImportingFromErp()
        {
            if (_selectedDocument != null)
            {
                DataExchangeManagement dem = new DataExchangeManagement();
                var capacityTask = dem.GetCapacityLedgerEntries(_selectedWorkcenter, _selectedDocument.Workday.ToString("yyyy-MM-dd"));
                var capacity = await capacityTask;

                var actualDocument = _inspectionFollowupDocs?.FirstOrDefault(s => s.Workday == _selectedDocument.Workday);

                if (capacity != null && actualDocument != null)
                {
                    actualDocument.Shift1Output = (int)capacity.Where(s => s.ShiftCode == "1").Sum(s => s.OutputQty * 1000);
                    actualDocument.Shift2Output = (int)capacity.Where(s => s.ShiftCode == "2").Sum(s => s.OutputQty * 1000);
                    actualDocument.Shift3Output = (int)capacity.Where(s => s.ShiftCode == "3").Sum(s => s.OutputQty * 1000);
                    actualDocument.Shift1Reject = (int)capacity.Where(s => s.ShiftCode == "1" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.ScrapQty * 1000);
                    actualDocument.Shift2Reject = (int)capacity.Where(s => s.ShiftCode == "2" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.ScrapQty * 1000);
                    actualDocument.Shift3Reject = (int)capacity.Where(s => s.ShiftCode == "3" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.ScrapQty * 1000);
                    actualDocument.SupplierReject = (int)capacity.Where(s => s.ScrapCode == "E").Sum(s => s.ScrapQty * 1000);
                    actualDocument.OperatingHour = (double)capacity.Sum(s => s.Quantity);
                }
                OnPropertyChanged(nameof(InspectionFollowupDocuments));
                UpdateMainDocument();
                SavingDocument(false);
            }
        }

        public ICommand AddExtraWorkday => new ProjectCommandRelay(_ => AddingExtraWorkday());
        /// <summary>
        /// Updates the collection of inspection follow-up documents by adding an extra workday and reordering the items
        /// by workday date.
        /// </summary>
        /// <remarks>This method refreshes the InspectionFollowupDocuments collection, attaches property
        /// change event handlers to each item, and updates related state. It should be called when an additional
        /// workday needs to be incorporated into the inspection follow-up workflow.</remarks>
        private void AddingExtraWorkday()
        {
            if (_inspectionFollowupDocs != null && InspectionFollowupDocuments != null)
            {
                UserControlFunctions uf = new UserControlFunctions(_dialogs);

                InspectionFollowupDocuments = new ObservableCollection<InspectionFollowupDocument>(uf.AddExtraWorkdayMachine(_inspectionFollowupDocs, _extraWorkday).ToList());
                InspectionFollowupDocuments = new ObservableCollection<InspectionFollowupDocument>(InspectionFollowupDocuments.OrderBy(Workdays => Workdays.Workday).ToList());
                foreach (var i in InspectionFollowupDocuments) { i.PropertyChanged += InspectionItem_PropertyChanged; }

                UpdateMainDocument();
                OnPropertyChanged(nameof(InspectionFollowupDocuments));
                SavingDocument(false);
            }
        }

        public ICommand? DeleteSelected => new ProjectCommandRelay(_ => DeletingSelected());
        /// <summary>
        /// Deletes the currently selected document from the collection after user confirmation.
        /// </summary>
        /// <remarks>This method prompts the user for confirmation before removing the selected document.
        /// If the user confirms, the document is removed and related state is updated. No action is taken if no
        /// document is selected.</remarks>
        private void DeletingSelected()
        {
            if (_selectedDocument != null && InspectionFollowupDocuments != null && SelectedDocument != null)
            {
                if (_dialogs.ShowConfirmation($"Tényleg törölni szeretnéd a kijelölt ({_selectedDocument.Workday}) munkanapot? ", "Törlés"))
                {
                    InspectionFollowupDocuments.Remove(SelectedDocument);
                    UpdateMainDocument();
                    SavingDocument(false);
                }
            }
        }

        public ICommand? SaveDocument => new ProjectCommandRelay(_ => SavingDocument());
        /// <summary>
        /// Saves the current follow-up document to the database and updates related state. Optionally displays a
        /// confirmation or error message to the user based on the outcome.
        /// </summary>
        /// <remarks>This method updates related properties and triggers UI notifications after saving the
        /// document. If no follow-up document is available, an error dialog is shown when isConfirm is set to
        /// true.</remarks>
        /// <param name="isConfirm">true to display a confirmation or error dialog to the user after the save operation completes; otherwise,
        /// false to suppress user dialogs.</param>
        private void SavingDocument(bool isConfirm = true)
        {
            if (_followupDocument != null)
            {
                OnPropertyChanged(nameof(InspectionFollowupDocuments));
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
                    if (isConfirm) { _dialogs.ShowErrorInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikertelen a következő hiba miatt : {ret.message}", "InspectionViewModel"); }
                }
            }
            else
            {
                _dialogs.ShowErrorInfo($"A dokumentum mentése sikertelen! _followupDocuments => null", "InspectionViewModel");
            }
        }

        public ICommand? ExportExcel => new ProjectCommandRelay(_ => ExportingExcel());
        /// <summary>
        /// Exports the current inspection follow-up documents to an Excel file using the selected work center as the
        /// file name.
        /// </summary>
        /// <remarks>If an error occurs during the export process, an error dialog is displayed to inform
        /// the user. This method does not return a value and is intended for internal use within the class.</remarks>
        private void ExportingExcel()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                e.ExcelExport(_inspectionFollowupDocs, $"{_selectedWorkcenter} followup", $"{_selectedWorkcenter} followup");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Excel export");
            }
        }

        #endregion

        #region Constructor
        public InspectionViewModel(IUserDialogService dialogs,
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
        /// Initializes the current instance with the specified parent node and workcenter identifier.
        /// </summary>
        /// <param name="parent">The parent node to associate with this instance. Cannot be null.</param>
        /// <param name="workcenter">The identifier of the workcenter to select. Cannot be null or empty.</param>
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
        /// <remarks>This method should be called whenever the follow-up document or its shift information
        /// changes to ensure that the UI reflects the correct visibility and font size settings.</remarks>
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
        /// Updates the main document by synchronizing inspection follow-up documents for the selected workcenter.
        /// </summary>
        /// <remarks>This method updates the inspection follow-up documents associated with the selected
        /// workcenter in the main follow-up document. If either the main follow-up document or the collection of
        /// inspection follow-up documents is null, no action is performed.</remarks>
        private void UpdateMainDocument()
        {
            if (_followupDocument != null && InspectionFollowupDocuments != null)
            {
                var target = _followupDocument.InspectionFollowups.FirstOrDefault(x => x.Workcenter == _selectedWorkcenter);
                if (target != null)
                {
                    target.InspectionFollowupDocuments = InspectionFollowupDocuments.ToList();
                }
            }
        }
        /// <summary>
        /// Updates the cumulative output value based on the provided inspection follow-up documents.
        /// </summary>
        /// <remarks>This method calculates the cumulative output using the inspection follow-up documents
        /// and updates the <see cref="ComulatedOut"/> property. It triggers the <see cref="OnPropertyChanged"/> event
        /// to notify listeners of the property change.</remarks>
        private void GetComulatedOut()
        {
            if (_inspectionFollowupDocs != null)
            {
                _comulatedOut = $"Kumulált kimenet : {_userControlFunctions.GetComulatedOut(_inspectionFollowupDocs)}";
                //_comulatedOut = $"Komulált kimenet : {_inspectionFollowupDocs.Select(x => x.OutputSum).Sum()}";
                OnPropertyChanged(nameof(ComulatedOut));
            }
        }
        /// <summary>
        /// Calculates the difference between the daily output sum and the cumulative plan value based on the inspection
        /// follow-up documents, and updates the <see cref="Difference"/> property.
        /// </summary>
        /// <remarks>This method processes the collection of inspection follow-up documents to compute the
        /// difference. If the daily output sum is zero, the cumulative plan value is retrieved from the first document.
        /// Otherwise, the cumulative plan value is retrieved from the last document where the total output is greater
        /// than zero. After calculating the difference, the <see cref="Difference"/> property is updated and a property
        /// change notification is raised.</remarks>
        private void GetDifference()
        {
            if (_inspectionFollowupDocs != null)
            {
                var dailyOutSum = _inspectionFollowupDocs.Select(x => x.OutputSum).Sum();
                if (dailyOutSum == 0)
                {
                    var actualComulatedSum = _inspectionFollowupDocs.Select(x => x.ComulatedPlan).FirstOrDefault();
                    _difference = $"Diff.: {dailyOutSum - actualComulatedSum}";
                }
                else
                {
                    var actualComulatedSum = _inspectionFollowupDocs.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                    _difference = $"Diff.: {dailyOutSum - actualComulatedSum}";
                }
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Retrieves the follow-up document associated with the current parent object and updates related collections.
        /// </summary>
        /// <remarks>This method fetches the follow-up document from the database based on the parent's
        /// name and updates the  <see cref="_followupDocument"/> field. It also refreshes the <see
        /// cref="_inspectionFollowupDocs"/> collection  with inspection follow-up documents filtered by the selected
        /// work center. Event handlers are managed to ensure  proper notification of property changes.</remarks>
        private void GetFollowupDocument()
        {
            if (_parent != null)
            {
                var databaseCollection = _connectionManagement.GetCollection<MasterFollowupDocument>(_connectionManagement.DbName);

                var doc = databaseCollection.Find(x => x.DocumentName == _parent.Name).FirstOrDefault();
                _followupDocument = doc; //A módosításhoz ell kell tárolni a FollowupDocument eredetijét a Save-ben használjuk            
                if (_inspectionFollowupDocs != null)
                {
                    foreach (var item in _inspectionFollowupDocs)
                    {
                        item.PropertyChanged -= InspectionItem_PropertyChanged;
                    }
                }

                _inspectionFollowupDocs = new ObservableCollection<InspectionFollowupDocument>(doc.InspectionFollowups.Where(x => x.Workcenter == _selectedWorkcenter).Select(s => s.InspectionFollowupDocuments).FirstOrDefault().ToList());

                // a dokumentumban lévő összes elemre feliratkoztatjuk a HeadcountItem_PropertyChanged eventet
                foreach (var item in _inspectionFollowupDocs)
                {
                    item.PropertyChanged += InspectionItem_PropertyChanged;
                }

                OnPropertyChanged(nameof(InspectionFollowupDocuments));
            }
        }
        /// <summary>
        /// Handles the <see cref="PropertyChanged"/> event for an inspection item.
        /// </summary>
        /// <remarks>This method responds to changes in specific properties of the <see
        /// cref="InspectionFollowupDocument"/>. When certain properties are updated, it triggers recalculations and
        /// updates related to cumulative data, output differences, and other relevant state changes.</remarks>
        /// <param name="sender">The source of the property change event. Typically, this is the object whose property was changed.</param>
        /// <param name="e">The event data containing the name of the property that changed.</param>
        private void InspectionItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InspectionFollowupDocument.DailyPlan))
            {
                UpdateComulatedData();
                GetComulatedOut();
                GetDifference();
                //SetChart();
            }
            if (
                e.PropertyName == nameof(InspectionFollowupDocument.Shift1Output) ||
                e.PropertyName == nameof(InspectionFollowupDocument.Shift2Output) ||
                e.PropertyName == nameof(InspectionFollowupDocument.Shift3Output) ||
                e.PropertyName == nameof(InspectionFollowupDocument.Shift1Reject) ||
                e.PropertyName == nameof(InspectionFollowupDocument.Shift2Reject) ||
                e.PropertyName == nameof(InspectionFollowupDocument.Shift3Reject) ||
                e.PropertyName == nameof(InspectionFollowupDocument.SupplierReject) ||
                e.PropertyName == nameof(InspectionFollowupDocument.CommentForRejects)
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
        /// Configures and updates various chart data series based on inspection follow-up documents.
        /// </summary>
        /// <remarks>This method processes the data from the inspection follow-up documents and populates
        /// multiple chart series collections,  including column, line, and pie charts. The charts represent metrics
        /// such as output, daily plans, reject counts, reject percentages,  and work time utilization. Additionally, it
        /// calculates the average utilization percentage if applicable.</remarks>
        private void SetChart()
        {
            Labels = _inspectionFollowupDocs.Select(c => c.Workday.Day.ToString()).ToList();
            OnPropertyChanged(nameof(Labels));
            int wordayCount = _inspectionFollowupDocs.Select(c => c.Workday).ToList().Count();

            OutputChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Output",
                    Values = new ChartValues<int> (_inspectionFollowupDocs.Select(x => x.OutputSum).ToList()),
                    DataLabels = true,
                },
                new LineSeries
                {
                    Title = "Dail plan",
                    Values = new ChartValues<int>(_inspectionFollowupDocs.Select(x => x.DailyPlan)),
                },
            };

            ComOutChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Output",
                    Values = new ChartValues<int> (_inspectionFollowupDocs.Select(x => x.ComulatedOutput).ToList()),
                    DataLabels = true,
                },
                new LineSeries
                {
                    Title = "Comulated plan",
                    Values = new ChartValues<int>(_inspectionFollowupDocs.Select(x => x.ComulatedPlan)),
                },
            };

            RejectChart = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Reject",
                    Values = new ChartValues<int>(_inspectionFollowupDocs.Select(x => x.RejectSum).ToList()),
                    DataLabels = true,
                    //PointGeometrySize = 10
                },
            };

            RejectPercentChart = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Reject",
                    Values = new ChartValues<double>(_inspectionFollowupDocs.Select(x => x.CalcRejectRatio).ToList()),
                    //PointGeometrySize = 10
                    DataLabels = true,
                },
            };

            RejectPieChart = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Reject",
                    Values = new ChartValues<int>{_inspectionFollowupDocs.Select(x => x.RejectSum).Sum() },
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Supplier reject",
                    Values = new ChartValues<int>{_inspectionFollowupDocs.Select(x => x.SupplierReject).Sum() },
                    DataLabels = true,
                }
            };
            WorkTimeSeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Actual WH",
                    Values = new ChartValues<double>( _inspectionFollowupDocs.Select(x => x.OperatingHour).ToList() ),
                    DataLabels = true,
                },

                new LineSeries
                {
                    Title = "Avail. WH",
                    Values = new ChartValues<double>(_inspectionFollowupDocs.Select(x => x.AvailOperatingHour).ToList() ),
                    DataLabels = false,
                }
            };

            if (_inspectionFollowupDocs.Where(x => x.PersistedUtilization > 0).Select(x => x.PersistedUtilization * 100).ToList().Count() > 0)
            {
                AvgUtil = _inspectionFollowupDocs.Where(x => x.PersistedUtilization > 0).Select(x => x.PersistedUtilization * 100).Average();
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
            if (InspectionFollowupDocuments != null)
            {
                int currentComulatedQty = 0;
                int currentComulatedOutputQty = 0;
                foreach (var day in InspectionFollowupDocuments)
                {
                    day.ComulatedPlan = currentComulatedQty + day.DailyPlan;
                    day.ComulatedOutput = currentComulatedOutputQty + day.TTLOutput;
                    currentComulatedQty = day.ComulatedPlan;
                    currentComulatedOutputQty = day.ComulatedOutput;
                }

                OnPropertyChanged(nameof(InspectionFollowupDocuments));
            }
        }
        #endregion
    }
}
