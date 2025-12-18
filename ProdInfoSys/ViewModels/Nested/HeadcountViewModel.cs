using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Models;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.NonRelationalModels;
using ProdInfoSys.Windows;
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
    public class HeadcountViewModel : INotifyPropertyChanged
    {
        private TreeNodeModel? _parent;
        private MasterFollowupDocument? _followupDocument;
        public Action? ForceCommit {get; set;}

        #region Dependency injection
        private IUserDialogService _dialogs;
        private IUserControlFunctions _userControlFunctions;
        private IConnectionManagement _connectionManagement;
        #endregion
      
        #region Charts
        public List<string>? Labels { get; set; }
        #endregion

        #region PropChangedInterface

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication

        private double _fontSize;
        public double FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        private string? _mustSave;
        public string? MustSave
        {
            get => _mustSave;
            set { _mustSave = value; OnPropertyChanged(); }
        }

        private DateTime _extraWorkday;
        public DateTime ExtraWorkday
        {
            get => _extraWorkday;
            set { _extraWorkday = value; OnPropertyChanged(); }
        }

        private ObservableCollection<HeadCountFollowupDocument>? _headcountFollowupDocs;
        public ObservableCollection<HeadCountFollowupDocument>? HeadCountFollowupDocuments
        {
            get => _headcountFollowupDocs;
            set { _headcountFollowupDocs = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _seriesCollection;
        public SeriesCollection? SeriesCollection
        {
            get => _seriesCollection;
            set { _seriesCollection = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _absenseSeriesCollection;
        public SeriesCollection? AbsenseSeriesCollection
        {
            get => _absenseSeriesCollection;
            set { _absenseSeriesCollection = value; OnPropertyChanged(); }
        }

        private SeriesCollection? _workTimeSeriesCollection;
        public SeriesCollection? WorkTimeSeriesCollection
        {
            get => _workTimeSeriesCollection;
            set { _workTimeSeriesCollection = value; OnPropertyChanged(); }
        }

        private HeadCountFollowupDocument? _selectedDocument;
        public HeadCountFollowupDocument? SelectedDocument
        {
            get => _selectedDocument;
            set { _selectedDocument = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand AddExtraWorkday => new ProjectCommandRelay(_ => AddingExtraWorkday());
        private void AddingExtraWorkday()
        {
            if (_headcountFollowupDocs != null && HeadCountFollowupDocuments != null)
            {
                UserControlFunctions uf = new UserControlFunctions(_dialogs);
                HeadCountFollowupDocuments = new ObservableCollection<HeadCountFollowupDocument>(uf.AddExtraWorkday(_headcountFollowupDocs, _extraWorkday).ToList());

                HeadCountFollowupDocuments = new ObservableCollection<HeadCountFollowupDocument>(HeadCountFollowupDocuments.OrderBy(s => s.Workday).ToList());
                foreach (var i in HeadCountFollowupDocuments)
                {
                    i.PropertyChanged += HeadcountItem_PropertyChanged;
                }
                OnPropertyChanged(nameof(HeadCountFollowupDocuments));
                UpdateMainDocument();
                SavingDocument(false);
            }
        }


        public ICommand? DeleteSelected => new ProjectCommandRelay(_ => DeletingSelected());
        private void DeletingSelected()
        {
            if (_selectedDocument != null && _headcountFollowupDocs != null)
            {
                if (_dialogs.ShowConfirmation($"Tényleg törölni szeretnéd a kijelölt ({_selectedDocument.Workday}) munkanapot? ", "Törlés"))
                {
                    _headcountFollowupDocs.Remove(_selectedDocument);
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
                OnPropertyChanged(nameof(HeadCountFollowupDocuments));
                ForceCommit?.Invoke();
                var ret = _userControlFunctions.SaveDocumentToDatabase(_connectionManagement, _followupDocument);
                if (ret.isCompleted)
                {
                    GetFollowupDocument();
                    SetChart();
                    _mustSave = string.Empty;
                    OnPropertyChanged(nameof(MustSave));
                    if (isConfirm) { _dialogs.ShowInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikeres!", "HeadcountViewModel"); }

                }
                else
                {
                    if (isConfirm) { _dialogs.ShowErrorInfo($"A(z) {_followupDocument.DocumentName} nevű dokumentum mentése sikertelen a következő hiba miatt : {ret.message}", "HeadcountViewModel"); }
                }
            }
        }

        public ICommand? ExportExcel => new ProjectCommandRelay(_ => ExportingExcel());
        private void ExportingExcel()
        {
            try
            {
                ExcelIO e = new ExcelIO();
                e.ExcelExport(_headcountFollowupDocs, "Headcount followup", "Headcount followup");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Excel export");
            }
        }
        #endregion

        #region Constructor
        public HeadcountViewModel(IUserDialogService dialogs,
            IUserControlFunctions userControlFunctions,
            IConnectionManagement connectionManagement
            )
        {
            _dialogs = dialogs;
            _userControlFunctions = userControlFunctions;
            _connectionManagement = connectionManagement;            
            _extraWorkday = DateTime.Now;
            _fontSize = (double)RegistryManagement.ReadIntRegistryKey("FontSize");
            //_mustSave = "Nem mentett dokumentum";
            //OnPropertyChanged(nameof(MustSave));
            OnPropertyChanged(nameof(FontSize));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// A szülőt át kell hozni a nested formra az adatbázis kezelés miatt.
        /// Mivel a WPF-nek paraméter nélküli ctor kell ezért van ez az init
        /// </summary>
        /// <param name="parent">Az aktuális TreeViewModell</param>
        public void Init(TreeNodeModel parent)
        {
            _parent = parent;
            GetFollowupDocument();
            SetChart();
        }
        #endregion

        #region Private methods
        private void UpdateMainDocument()
        {
            if (_followupDocument != null && HeadCountFollowupDocuments != null)
            {
                _followupDocument.Headcount = HeadCountFollowupDocuments.ToList();
            }
        }

        /// <summary>
        /// Retrieves the follow-up document associated with the parent entity and initializes related collections.
        /// </summary>
        /// <remarks>This method fetches the follow-up document from the database based on the parent's
        /// name and assigns it to the internal field. It also initializes the collection of headcount follow-up
        /// documents, subscribing to their <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
        /// events. The collection is then sorted by the <c>Workday</c> property, and a property change notification is
        /// raised for the <c>HeadCountFollowupDocuments</c> property.</remarks>
        private void GetFollowupDocument()
        {
            var databaseCollection = _connectionManagement.GetCollection<MasterFollowupDocument>(_connectionManagement.DbName);

            var doc = databaseCollection.Find(x => x.DocumentName == _parent.Name).FirstOrDefault();
            _followupDocument = doc; //A módosításhoz ell kell tárolni a FollowupDocument eredetijét a Save-ben használjuk

            if (_headcountFollowupDocs != null)
            {
                foreach (var item in _headcountFollowupDocs)
                {
                    item.PropertyChanged -= HeadcountItem_PropertyChanged;
                }
            }

            _headcountFollowupDocs = new ObservableCollection<HeadCountFollowupDocument>(doc.Headcount);

            // a dokumentumban lévő összes elemre feliratkoztatjuk a HeadcountItem_PropertyChanged eventet
            foreach (var item in _headcountFollowupDocs)
            {
                item.PropertyChanged += HeadcountItem_PropertyChanged;
            }
            _headcountFollowupDocs = new ObservableCollection<HeadCountFollowupDocument>(_headcountFollowupDocs.OrderBy(s => s.Workday).ToList());
            OnPropertyChanged(nameof(HeadCountFollowupDocuments));

        }

        /// <summary>
        /// Erre van feliratkozva a dokumentum összes property-je. Itt kell definiálni melyik property változást akarjuk figyelni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeadcountItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (
                e.PropertyName == nameof(HeadCountFollowupDocument.ActualHC) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.FFCIndirect) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.NettoHCPlan) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.HCPlan) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.Subcontactor) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.FFCIndirect) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.QAIndirect) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.Holiday) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.Others) ||
                e.PropertyName == nameof(HeadCountFollowupDocument.Sick)

                )
            {
                
                _mustSave = "Nem mentett dokumentum";
                OnPropertyChanged(nameof(MustSave));
                //SetChart();
            }
        }
        /// <summary>
        /// Configures and populates the chart data for displaying headcount and work time statistics.
        /// </summary>
        /// <remarks>This method initializes the chart data series and labels based on the provided
        /// headcount follow-up documents  and the follow-up document. It prepares multiple series collections for
        /// different metrics, including planned  and actual headcount, absence statistics, and work time comparisons.
        /// The method ensures that the data is  aggregated and formatted appropriately for visualization.</remarks>
        private void SetChart()
        {
            if (_headcountFollowupDocs != null && _followupDocument != null)
            {
                Labels = _headcountFollowupDocs.Select(c => c.Workday.Day.ToString()).ToList();
                OnPropertyChanged(nameof(Labels));
                int wordayCount = _headcountFollowupDocs.Select(c => c.Workday).ToList().Count();
                double avgNettoHcPlan = _headcountFollowupDocs.Select(x => x.NettoHCPlan).Average();

                List<double> actualHCAvgList = new List<double>();
                //List<double> nettoAvgList = Enumerable.Repeat(avgNettoHcPlan, wordayCount).ToList();

                if (_headcountFollowupDocs.Where(x => x.ActualHC > 0).Count() != 0)
                {
                    double avgActualHcPlan = _headcountFollowupDocs.Where(x => x.ActualHC > 0).Select(x => x.ActualHC).Average();
                    actualHCAvgList = Enumerable.Repeat(avgActualHcPlan, wordayCount).ToList();
                }
                else
                {
                    double avgActualHcPlan = 0;
                    actualHCAvgList = Enumerable.Repeat(avgActualHcPlan, wordayCount).ToList();
                }

                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Netto HC plan",
                        Values = new ChartValues<int> (_headcountFollowupDocs.Select(x => x.NettoHCPlan).ToList())
                    },
                    new ColumnSeries
                    {
                        Title = "Daily Actual HC",
                        Values = new ChartValues<int>(_headcountFollowupDocs.Select(x => x.ActualHC).ToList())
                    },
                    new LineSeries
                    {
                        Title = "FFC direct+indirect",
                        Values = new ChartValues<int>(_headcountFollowupDocs.Select(x => x.FFCDirectPlusIndirect).ToList())
                    },
                };

                List<double> absenseLimitList = _headcountFollowupDocs.Select(x => x.HCPlan * _followupDocument.AbsenseRatio).ToList();
                List<int> absenseTotal = _headcountFollowupDocs.Select(x => x.AbsebseTotal).ToList();

                AbsenseSeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Távollét terv",
                        Values = new ChartValues<double>(absenseLimitList),
                        //PointGeometrySize = 10
                    },

                    new ColumnSeries
                    {
                        Title = "Távollét tény",
                        Values = new ChartValues<int>(absenseTotal),
                        //PointGeometrySize = 10
                    }
                };

                List<double> plannedWorkTime = _headcountFollowupDocs.Select(x => x.CalcPlannedSH).ToList();
                List<double> actualWorkTime = _headcountFollowupDocs.Select(x => x.CalcActualSH).ToList();

                WorkTimeSeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Terv",
                        Values = new ChartValues<double>(plannedWorkTime),
                        //PointGeometrySize = 10
                    },

                    new ColumnSeries
                    {
                        Title = "Tény",
                        Values = new ChartValues<double>(actualWorkTime),
                        //PointGeometrySize = 10
                    },
                };
            }
        }
    }
    #endregion
}

