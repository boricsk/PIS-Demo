using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Models;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ProdInfoSys.ViewModels.Nested
{
    public class ManualViewModel : INotifyPropertyChanged
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
        public Action? ForceCommit {get; set;}

        #region PropChangedInterface

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Charts
        public List<string>? Labels { get; set; }
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

        //Subcon visibility
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(); }
        }

        private DateTime _extraWorkday;
        public DateTime ExtraWorkday
        {
            get => _extraWorkday;
            set { _extraWorkday = value; OnPropertyChanged(); }
        }

        private ManualFollowupDocument? _selectedDocument;
        public ManualFollowupDocument? SelectedDocument
        {
            get => _selectedDocument;
            set { _selectedDocument = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ManualFollowupDocument>? _manualFollowupDocs;
        public ObservableCollection<ManualFollowupDocument>? ManualFollowupDocs
        {
            get => _manualFollowupDocs;
            set { _manualFollowupDocs = value; OnPropertyChanged(); }
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

        private string? _difference;
        public string? Difference
        {
            get => _difference;
            set { _difference = value; OnPropertyChanged(); }
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

        private SeriesCollection? _rejectChart;
        public SeriesCollection? RejectChart
        {
            get => _rejectChart;
            set { _rejectChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _rejectPercentChart;
        public SeriesCollection? RejectPercentChart
        {
            get => _rejectPercentChart;
            set { _rejectPercentChart = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _rejectPieChart;
        public SeriesCollection? RejectPieChart
        {
            get => _rejectPieChart;
            set { _rejectPieChart = value; OnPropertyChanged(); }
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
        #endregion

        #region ICommand
        public ICommand ImportFromErp => new ProjectCommandRelay(async _ => await ImportingFromErp());
        private async Task ImportingFromErp()
        {
            if (_selectedDocument != null)
            {
                DataExchangeManagement dem = new DataExchangeManagement();
                var capacityTask = dem.GetExtCapacityLedgerEntries(_selectedWorkcenter, _selectedDocument.Workday.ToString("yyyy-MM-dd"));
                var capacity = await capacityTask;

                var actualDocument = _manualFollowupDocs?.FirstOrDefault(s => s.Workday == _selectedDocument.Workday);

                if (capacity != null && actualDocument != null)
                {
                    actualDocument.Shift1KftOutput = (int)capacity.Where(s => s.ShiftCode == "1").Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift2KftOutput = (int)capacity.Where(s => s.ShiftCode == "2").Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift3KftOutput = (int)capacity.Where(s => s.ShiftCode == "3").Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift1KftReject = (int)capacity.Where(s => s.ShiftCode == "1" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.Shift2KftReject = (int)capacity.Where(s => s.ShiftCode == "2" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.OutputQuantity * 1000);
                    actualDocument.Shift3KftReject = (int)capacity.Where(s => s.ShiftCode == "3" && !_notInvolvedScrapCodes.Contains(s.ScrapCode)).Sum(s => s.OutputQuantity * 1000);
                    actualDocument.SupplierReject = (int)capacity.Where(s => s.ScrapCode == "E").Sum(s => s.ScrapQuantity * 1000);
                    //actualDocument.TST = (int)capacity.Where(s => s.ScrapCode == "TST").Sum(s => s.ScrapQuantity * 1000);
                    //actualDocument.NK = (int)capacity.Where(s => s.ScrapCode == "NK").Sum(s => s.ScrapQuantity * 1000);
                    actualDocument.BS = (int)capacity.Where(s => s.ScrapCode == "BS").Sum(s => s.ScrapQuantity * 1000);
                    //actualDocument.OperatingHour = (double)capacity.Sum(s => s.RunTime);

                    if (capacity.Where(s => s.Perf != 0).Count() != 0)
                    {
                        actualDocument.ProductivityKft = capacity.Where(s => s.Perf != 0).Average(s => s.Perf);
                    }
                }
                OnPropertyChanged(nameof(ManualFollowupDocs));
                UpdateMainDocument();
                SavingDocument(false);
            }
        }
        public ICommand AddExtraWorkday => new ProjectCommandRelay(_ => AddingExtraWorkday());
        private void AddingExtraWorkday()
        {
            if (_manualFollowupDocs != null && ManualFollowupDocs != null)
            {
                UserControlFunctions uf = new UserControlFunctions(_dialogs);
                ManualFollowupDocs = new ObservableCollection<ManualFollowupDocument>(uf.AddExtraWorkdayManual(_manualFollowupDocs, _extraWorkday).ToList());
                ManualFollowupDocs = new ObservableCollection<ManualFollowupDocument>(ManualFollowupDocs.OrderBy(Workdays => Workdays.Workday).ToList());
                foreach (var i in ManualFollowupDocs) { i.PropertyChanged += ManualItem_PropertyChanged; }

                UpdateMainDocument();
                OnPropertyChanged(nameof(ManualFollowupDocs));
                SavingDocument(false);
            }
        }

        public ICommand? DeleteSelected => new ProjectCommandRelay(_ => DeletingSelected());
        private void DeletingSelected()
        {
            if (_selectedDocument != null && ManualFollowupDocs != null && SelectedDocument != null)
            {
                if (_dialogs.ShowConfirmation($"Tényleg törölni szeretnéd a kijelölt ({_selectedDocument.Workday}) munkanapot? ", "Törlés"))
                {
                    ManualFollowupDocs.Remove(SelectedDocument);
                    UpdateMainDocument();
                    SavingDocument(false);
                }
            }
        }

        public ICommand? SaveDocument => new ProjectCommandRelay(_ => SavingDocument());
        private void SavingDocument(bool isConfirm = true)
        {
            if (_followupDocument != null)
            {
                OnPropertyChanged(nameof(ManualFollowupDocs));
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
                    if (isConfirm) { _dialogs.ShowInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikeres!", "ManualViewModel"); }
                }
                else
                {
                    if (isConfirm) { _dialogs.ShowErrorInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikertelen a következő hiba miatt : {ret.message}", "ManualViewModel"); }
                }
            }
        }

        public ICommand? ExportExcel => new ProjectCommandRelay(_ => ExportingExcel());
        private void ExportingExcel()
        {
            ExcelIO e = new ExcelIO();            
            e.ExcelExport(_manualFollowupDocs, $"{_selectedWorkcenter} followup", $"{_selectedWorkcenter} followup");
        }

        #endregion

        #region Constructor
        public ManualViewModel(IUserDialogService dialogs,
            IUserControlFunctions userControlFunctions,
            IConnectionManagement connectionManagement
            )
        {
            _dialogs = dialogs;            
            _extraWorkday = DateTime.Now;
            _userControlFunctions = userControlFunctions;
            _connectionManagement = connectionManagement;            
        }
        #endregion

        #region Public methods

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

        public Func<double, string> YFormatter => value => value.ToString("P2");
        public Func<double, string> ScrapYFormatter => value => value.ToString("N2");

        #endregion

        #region Private methods
        private void SetVisibility()
        {
            if (_followupDocument != null)
            {
                switch (_followupDocument.ShiftNumberManual)
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
        /// Updates the main follow-up document with the current manual follow-up documents for the selected workcenter.
        /// </summary>
        /// <remarks>This method modifies the follow-up document by updating the list of manual follow-up
        /// documents associated with the selected workcenter. If no matching workcenter is found, no changes are
        /// made.</remarks>
        private void UpdateMainDocument()
        {
            if (_followupDocument != null && ManualFollowupDocs != null)
            {
                var target = _followupDocument.ManualFollowups.FirstOrDefault(x => x.Workcenter == _selectedWorkcenter);
                if (target != null)
                {
                    target.MaualFollowupDocuments = ManualFollowupDocs.ToList();
                }
            }
        }
        /// <summary>
        /// Updates the <see cref="ComulatedOut"/> property with a formatted string based on the current manual
        /// follow-up documents.
        /// </summary>
        /// <remarks>This method checks if the <see cref="ManualFollowupDocs"/> property is not null and,
        /// if so, generates a cumulative output string using the provided manual follow-up documents. It then raises
        /// the <see cref="OnPropertyChanged"/> event for the <see cref="ComulatedOut"/> property.</remarks>
        private void GetComulatedOut()
        {
            if (ManualFollowupDocs != null)
            {
                _comulatedOut = $"Kumulált kimenet : {_userControlFunctions.GetComulatedOut(_manualFollowupDocs)}";
                OnPropertyChanged(nameof(ComulatedOut));
            }
        }
        /// <summary>
        /// Calculates the difference between the daily output sum and the cumulative plan value  based on the provided
        /// manual follow-up documents, and updates the <see cref="Difference"/> property.
        /// </summary>
        /// <remarks>This method computes the difference using the `_manualFollowupDocs` collection. If
        /// the daily output sum  is zero, the first cumulative plan value is used; otherwise, the last cumulative plan
        /// value corresponding  to non-zero total output is used. After calculation, the <see cref="Difference"/>
        /// property is updated  and the <see cref="OnPropertyChanged"/> method is invoked to notify
        /// listeners.</remarks>
        private void GetDifference()
        {
            if (_manualFollowupDocs != null)
            {
                var dailyOutSum = _manualFollowupDocs.Select(x => x.OutputSum).Sum();
                if (dailyOutSum == 0)
                {
                    var actualComulatedSum = _manualFollowupDocs.Select(x => x.ComulatedPlan).FirstOrDefault();
                    _difference = $"Diff.: {dailyOutSum - actualComulatedSum}";
                }
                else
                {
                    var actualComulatedSum = _manualFollowupDocs.Where(x => x.TTLOutput > 0).Select(x => x.ComulatedPlan).LastOrDefault();
                    _difference = $"Diff.: {dailyOutSum - actualComulatedSum}";
                }
                OnPropertyChanged(nameof(Difference));
            }
        }
        /// <summary>
        /// Retrieves the follow-up document associated with the current parent object and initializes related data.
        /// </summary>
        /// <remarks>This method fetches the follow-up document from the database based on the parent's
        /// name and populates the collection of manual follow-up documents filtered by the selected work center. It
        /// also subscribes to property change events for each item in the collection to track changes.</remarks>
        private void GetFollowupDocument()
        {
            if (_parent != null)
            {
                var databaseCollection = _connectionManagement.GetCollection<MasterFollowupDocument>(_connectionManagement.DbName);

                var doc = databaseCollection.Find(x => x.DocumentName == _parent.Name).FirstOrDefault();
                _followupDocument = doc; //A módosításhoz ell kell tárolni a FollowupDocument eredetijét a Save-ben használjuk            
                _manualFollowupDocs = new ObservableCollection<ManualFollowupDocument>(doc.ManualFollowups.Where(x => x.Workcenter == _selectedWorkcenter).Select(s => s.MaualFollowupDocuments).FirstOrDefault().ToList());

                // a dokumentumban lévő összes elemre feliratkoztatjuk a HeadcountItem_PropertyChanged eventet
                foreach (var item in _manualFollowupDocs)
                {
                    item.PropertyChanged += ManualItem_PropertyChanged;
                }

                OnPropertyChanged(nameof(ManualFollowupDocs));
            }

        }

        /// <summary>
        /// Handles the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for a manual item.
        /// </summary>
        /// <remarks>This method responds to changes in specific properties of the <see
        /// cref="ManualFollowupDocument"/> object. When certain properties are updated, it triggers calculations to
        /// update cumulative data, output values,  and differences. Additionally, it sets a flag to indicate that the
        /// state has changed for specific property updates.</remarks>
        /// <param name="sender">The source of the property change event. Typically, this is the <see cref="ManualFollowupDocument"/>
        /// instance.</param>
        /// <param name="e">An instance of <see cref="PropertyChangedEventArgs"/> that contains the name of the property that changed.</param>
        private void ManualItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ManualFollowupDocument.DailyPlan))
            {
                UpdateComulatedData();
                GetComulatedOut();
                GetDifference();
            }
            if (
                e.PropertyName == nameof(ManualFollowupDocument.Shift1KftOutput) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift2KftOutput) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift3KftOutput) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift1SubconOutput) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift2SubconOutput) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift3SubconOutput) ||
                e.PropertyName == nameof(ManualFollowupDocument.SupplierReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift1KftReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift2KftReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift3KftReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift1SubconReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift2SubconReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.Shift3SubconReject) ||
                e.PropertyName == nameof(ManualFollowupDocument.CommentForRejects)
                )
            {                
                _mustSave = "Nem mentett dokumentum";
                OnPropertyChanged(nameof(MustSave));
                GetComulatedOut();
                GetDifference();
            }
        }
        /// <summary>
        /// Configures and updates various chart data series based on the current state of manual follow-up documents.
        /// </summary>
        /// <remarks>This method populates multiple chart series collections, including output charts,
        /// reject charts, efficiency charts,  and pie charts, using data derived from the <c>_manualFollowupDocs</c>
        /// collection. Each chart series is tailored  to represent specific metrics such as daily output, cumulative
        /// output, reject ratios, and productivity.  Charts are updated only if <c>_manualFollowupDocs</c> is not null.
        /// The method processes data from the collection  to generate labels and values for the charts, ensuring that
        /// the visualized data reflects the current state of the  follow-up documents.</remarks>
        private void SetChart()
        {
            if (_manualFollowupDocs != null)
            {
                Labels = _manualFollowupDocs.Select(c => c.Workday.Day.ToString()).ToList();
                OnPropertyChanged(nameof(Labels));
                int wordayCount = _manualFollowupDocs.Select(c => c.Workday).ToList().Count();
                var target = Enumerable.Repeat(1, wordayCount).ToList();

                OutputChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (_manualFollowupDocs.Select(x => x.OutputSum).ToList()),
                        DataLabels = true,
                    },
                    new LineSeries
                    {
                        Title = "Dail plan",
                        Values = new ChartValues<int>(_manualFollowupDocs.Select(x => x.DailyPlan)),
                    },
                };

                ComOutChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Output",
                        Values = new ChartValues<int> (_manualFollowupDocs.Select(x => x.ComulatedOutput).ToList()),
                        DataLabels=true,
                    },
                    new LineSeries
                    {
                        Title = "Comulated plan",
                        Values = new ChartValues<int>(_manualFollowupDocs.Select(x => x.ComulatedPlan)),
                    },
                };

                RejectChart = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Reject",
                        Values = new ChartValues<int>(_manualFollowupDocs.Select(x => x.RejectSum).ToList()),
                        //PointGeometrySize = 10
                        DataLabels = true,
                    },

                };

                RejectPercentChart = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Reject",
                        Values = new ChartValues<double>(_manualFollowupDocs.Select(x => x.CalcRejectRatio).ToList()),
                        //PointGeometrySize = 10
                        DataLabels = true,
                    },
                };

                RejectPieChart = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Reject",
                        Values = new ChartValues<int>{_manualFollowupDocs.Select(x => x.RejectSum).Sum() },
                        DataLabels = true
                    },

                    new PieSeries
                    {
                        Title = "Supplier reject",
                        Values = new ChartValues<int>{_manualFollowupDocs.Select(x => x.SupplierReject).Sum() },
                        DataLabels = true,
                    }
                };

                EfficiencyChart = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Tény (KFT)",
                        Values = new ChartValues<decimal> (_manualFollowupDocs.Select(x => x.ProductivityKft).ToList()),
                        DataLabels = true,
                    },

                    new ColumnSeries
                    {
                        Title = "Tény (Subcon)",
                        Values = new ChartValues<decimal> (_manualFollowupDocs.Select(x => x.ProductivitySubcon).ToList()),
                        DataLabels = true,
                    },

                    new LineSeries
                    {
                        Title = "Cél",
                        Values = new ChartValues<int>(target),
                    },
                };
            }
        }
        /// <summary>
        /// Updates the cumulative plan and output quantities for each day in the manual follow-up documents.
        /// </summary>
        /// <remarks>This method iterates through the collection of manual follow-up documents and
        /// calculates the cumulative plan and output quantities based on the daily plan and total output values for
        /// each day. After updating the values, it raises a property change notification for <see
        /// cref="ManualFollowupDocs"/>.</remarks>
        private void UpdateComulatedData()
        {
            if (_manualFollowupDocs != null && ManualFollowupDocs != null)
            {
                int currentComulatedQty = 0;
                int currentComulatedOutputQty = 0;
                foreach (var day in ManualFollowupDocs)
                {
                    day.ComulatedPlan = currentComulatedQty + day.DailyPlan;
                    day.ComulatedOutput = currentComulatedOutputQty + day.TTLOutput;
                    currentComulatedQty = day.ComulatedPlan;
                    currentComulatedOutputQty = day.ComulatedOutput;
                }

                OnPropertyChanged(nameof(ManualFollowupDocs));
            }
        }
        #endregion
    }
}
