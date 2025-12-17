using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Driver;
using Projector.Enums;
using Projector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;


namespace Projector.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _parent = string.Empty;
        private MasterFollowupDocument? _followupDocument;
        private ObservableCollection<ProjectorDataModel>? _charts;
        private int _projectCounter = 0;
        private List<string> _affectedWorkcenters = new List<string>(); 
        //{ "AD AMI", "AD AUTOMATA PUNCHING", "CYT", "APIM", "SS AUTOMATA BENDING", "BUNDLING", "PACKAGING" };

        private DispatcherTimer? _visualizationTimer;
        private DispatcherTimer? _updateTimer;
        private int _currentChartIndex = 0;
        private ObservableCollection<StatusReportMachineList> _statusReportMahines = new ObservableCollection<StatusReportMachineList>();
        private ObservableCollection<StopTime> _statusReportStopTimes = new ObservableCollection<StopTime>();

        #region PropChangedInterface

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication

        private SeriesCollection? _actualChartData;
        public SeriesCollection? ActualChartData
        {
            get => _actualChartData;
            set { _actualChartData = value; OnPropertyChanged(); }
        }

        private List<string>? _affectedManualWorkcenters;
        public List<string>? AffectedManualWorkcenters
        {
            get => _affectedManualWorkcenters;
            set { _affectedManualWorkcenters = value; OnPropertyChanged(); }
        }

        private string? _chartTitle;
        public string? ChartTitle
        {
            get => _chartTitle;
            set { _chartTitle = value; OnPropertyChanged(); }
        }

        private string? _xAxisTitle;
        public string? XAxisTitle
        {
            get => _xAxisTitle;
            set { _xAxisTitle = value; OnPropertyChanged(); }
        }

        private string? _yAxisTitle;
        public string? YAxisTitle
        {
            get => _yAxisTitle;
            set { _yAxisTitle = value; OnPropertyChanged(); }
        }

        private string? _xAxisRotation;
        public string? XAxisRotation
        {
            get => _xAxisRotation;
            set { _xAxisRotation = value; OnPropertyChanged(); }
        }

        private Func<double, string>? _yAxisFormatter;
        public Func<double, string>? YAxisFormatter
        {
            get => _yAxisFormatter;
            set { _yAxisFormatter = value; OnPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            _charts = new();
            _parent = LoadSetupData().ActualFollowup;
            AffectedManualWorkcenters = new();
            _affectedWorkcenters = LoadSetupData().ProjectorWorkcenters.ToList();
            GetFollowupDocument();
            BuildChartData();
            StartVisualizationLoop();
            UpdateTimerInit();
        }
        #endregion

        #region Private methods

        private void UpdateTimerInit()
        {
            _visualizationTimer.Stop();
            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1800)
            };

            _updateTimer.Tick += (s, e) => RunDataUpdate();

            _updateTimer.Start();
            _visualizationTimer.Start();
        }

        private void RunDataUpdate()
        {

            GetFollowupDocument();
            BuildChartData();

        }
        private void StartVisualizationLoop()
        {
            if (_charts == null || _charts.Count == 0)
                return;

            _visualizationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };

            _visualizationTimer.Tick += (s, e) => ShowNextChart();

            ShowNextChart();

            _visualizationTimer.Start();
        }

        private void ShowNextChart()
        {
            if (_charts == null || _charts.Count == 0)
                return;

            if (_charts.Count == 0)
                return;

            var chart = _charts[_currentChartIndex];

            // Frissítés UI-threaden
            ChartTitle = chart.Id;
            ActualChartData = chart.ChartData;
            XAxisTitle = chart.XAxisTitle;
            YAxisTitle = chart.YAxisTitle;
            YAxisFormatter = chart.YAxisFormatter;
            XAxisRotation = chart.XAxisRotation;
            if (chart.XAxisLabel != null) { AffectedManualWorkcenters = chart.XAxisLabel; }

            _currentChartIndex++;
            if (_currentChartIndex >= _charts.Count)
                _currentChartIndex = 0;
        }
        private void GetFollowupDocument()
        {
            try
            {
                ConnectionManagement conMgmnt = new ConnectionManagement();
                var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);
                var doc = databaseCollection.Find(x => x.DocumentName == _parent).FirstOrDefault();
                _followupDocument = doc;
                _statusReportMahines = new ObservableCollection<StatusReportMachineList>(doc.StatusReports.LastOrDefault().MachineData);
                if (doc.StatusReportsQRQC != null)
                {
                    _statusReportStopTimes = new ObservableCollection<StopTime>(doc.StatusReportsQRQC.LastOrDefault().StopTimes);
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"A következő hiba lépett fel a followup dokumentum betöltése közben : {ex.Message}", "GetFollowupDocuments", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        private void BuildChartData()
        {

            if (_followupDocument != null && _charts != null)
            {
                _charts.Clear();
                int workdayCount = _followupDocument.Headcount.Count();
                var efficiencyTarget = Enumerable.Repeat(1, workdayCount);

                List<string> xAxisLabel = _followupDocument.Headcount.Select(w => w.Workday.Day.ToString()).ToList();

                foreach (var document in _followupDocument.MachineFollowups)
                {
                    if (_affectedWorkcenters.Contains(document.Workcenter))
                    {
                        ProjectorDataModel projectorDataModel = new ProjectorDataModel();
                        projectorDataModel.Id = $"{document.Workcenter} Kimenet / Output";
                        projectorDataModel.XAxisTitle = "Munkanap / Workday";
                        projectorDataModel.YAxisTitle = "Kimenet / Output";
                        projectorDataModel.YAxisFormatter = value => value.ToString("N0");
                        projectorDataModel.XAxisLabel = xAxisLabel;
                        projectorDataModel.ChartData = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Kimenet / Output",
                                Values = new ChartValues<int>(document.MachineFollowupDocuments.Where(s => s.TTLOutput > 0).Select(x => x.ComulatedOutput).ToList()),
                                DataLabels = true,

                            },

                            new LineSeries
                            {
                                Title = "Kumulált terv / Accumulated plan",
                                Values = new ChartValues<int>(document.MachineFollowupDocuments.Select(x => x.ComulatedPlan)),

                            },

                        };
                        _charts.Add(projectorDataModel);

                        ProjectorDataModel projectorDataEff = new ProjectorDataModel();
                        projectorDataEff.Id = $"{document.Workcenter} Hatékonyság / Efficiency";
                        projectorDataEff.XAxisTitle = "Munkanap / Workday";
                        projectorDataEff.YAxisTitle = "Hatékonyság / Efficiency";
                        projectorDataEff.YAxisFormatter = value => value.ToString("P2");
                        projectorDataEff.XAxisLabel = xAxisLabel;
                        projectorDataEff.ChartData = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Hatékonyság / Efficiency",
                                Values = new ChartValues<double>(document.MachineFollowupDocuments.Select(s => (double)s.Efficiency).ToList()),
                                DataLabels = true,
                            },

                            new LineSeries
                            {
                                Title = "Cél / Target",
                                Values = new ChartValues<int>(efficiencyTarget),
                            }
                        };

                        _charts.Add(projectorDataEff);

                        var availOpHours = document.MachineFollowupDocuments.Select(s => s.AvailOperatingHour).FirstOrDefault();
                        var ophoursTarget = Enumerable.Repeat(availOpHours, workdayCount);
                        ProjectorDataModel projectorDataOpHours = new ProjectorDataModel();
                        projectorDataOpHours.Id = $"{document.Workcenter} Működési idő / Operation hours";
                        projectorDataOpHours.XAxisTitle = "Munkanap / Workday";
                        projectorDataOpHours.YAxisTitle = "Működési idő / Operation hours";
                        projectorDataOpHours.YAxisFormatter = value => value.ToString("N0");
                        projectorDataOpHours.XAxisLabel = xAxisLabel;
                        projectorDataOpHours.ChartData = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Működési idő / Operation hours",
                                Values = new ChartValues<double>(document.MachineFollowupDocuments.Select(s => (double)s.OperatingHour).ToList()),
                                DataLabels = true,
                            },

                            new LineSeries
                            {
                                Title = "Cél / Target",
                                Values = new ChartValues<double>((IEnumerable<double>)ophoursTarget),
                            }
                        };

                        _charts.Add(projectorDataOpHours);
                    }
                }

                foreach (var document in _followupDocument.InspectionFollowups)
                {
                    if (_affectedWorkcenters.Contains(document.Workcenter))
                    {
                        ProjectorDataModel projectorDataModel = new ProjectorDataModel();
                        projectorDataModel.Id = $"{document.Workcenter} Kimenet / Output";
                        projectorDataModel.XAxisTitle = "Munkanap / Workday";
                        projectorDataModel.YAxisTitle = "Kimenet / Output";
                        projectorDataModel.YAxisFormatter = value => value.ToString("N0");
                        projectorDataModel.XAxisLabel = xAxisLabel;
                        projectorDataModel.ChartData = new SeriesCollection
                                {
                                new ColumnSeries
                                {
                                    Title = "Kimenet / Output",
                                    Values = new ChartValues<int>(document.InspectionFollowupDocuments.Where(s => s.TTLOutput > 0).Select(x => x.ComulatedOutput).ToList()),
                                    DataLabels = true,
                                },

                                new LineSeries
                                {
                                    Title = "Kumulált terv / Accumulated plan",
                                    Values = new ChartValues<int>(document.InspectionFollowupDocuments.Select(x => x.ComulatedPlan)),
                                },
                                };

                        _charts.Add(projectorDataModel);

                        var availOpHours = document.InspectionFollowupDocuments.Select(s => s.AvailOperatingHour).FirstOrDefault();
                        var ophoursTarget = Enumerable.Repeat(availOpHours, workdayCount);
                        ProjectorDataModel projectorDataOpHours = new ProjectorDataModel();
                        projectorDataOpHours.Id = $"{document.Workcenter} Működési idő / Operation hours";
                        projectorDataOpHours.XAxisTitle = "Munkanap / Workday";
                        projectorDataOpHours.YAxisTitle = "Működési idő / Operation hours";
                        projectorDataOpHours.YAxisFormatter = value => value.ToString("N0");
                        projectorDataOpHours.XAxisLabel = xAxisLabel;
                        projectorDataOpHours.ChartData = new SeriesCollection
                                {
                                    new ColumnSeries
                                    {
                                        Title = "Működési idő / Operation hours",
                                        Values = new ChartValues<double>(document.InspectionFollowupDocuments.Select(s => (double)s.OperatingHour).ToList()),
                                        DataLabels = true,
                                    },

                                    new LineSeries
                                    {
                                        Title = "Cél / Target",
                                        Values = new ChartValues<double>((IEnumerable<double>)ophoursTarget),
                                    }
                                };

                        _charts.Add(projectorDataOpHours);
                    }
                }


                foreach (var document in _followupDocument.ManualFollowups)
                {
                    if (_affectedWorkcenters.Contains(document.Workcenter))
                    {
                        ProjectorDataModel projectorManual = new ProjectorDataModel();
                        projectorManual.Id = $"{document.Workcenter} Kimenet / Output";
                        projectorManual.XAxisTitle = "Munkanap / Workday";
                        projectorManual.YAxisTitle = "Kimenet / Output";
                        projectorManual.YAxisFormatter = value => value.ToString("N0");
                        projectorManual.XAxisLabel = xAxisLabel;
                        projectorManual.ChartData = new SeriesCollection
                            {
                                new ColumnSeries
                                {
                                    Title = "Kimenet / Output",
                                    Values = new ChartValues<int>(document.MaualFollowupDocuments.Where(s => s.TTLOutput > 0).Select(x => x.ComulatedOutput).ToList()),
                                    DataLabels = true,
                                },

                                new LineSeries
                                {
                                    Title = "Kumulált terv / Accumulated plan",
                                    Values = new ChartValues<int>(document.MaualFollowupDocuments.Select(x => x.ComulatedPlan)),
                                },
                            };

                        _charts.Add(projectorManual);
                    }
                }

                var _kftEff = _statusReportMahines.Where(m => m.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(m => m.AvgEfficiency).ToList();
                AffectedManualWorkcenters = _statusReportMahines.Where(m => m.WorkcenterType == EnumMachineType.FFCManualProcess.ToString()).Select(m => m.Workcenter).ToList();

                ProjectorDataModel projectorManualEff = new ProjectorDataModel();
                projectorManualEff.Id = $"Kézi folyamat hatékonyság / Manual process efficiency";
                projectorManualEff.XAxisTitle = "Munkanap / Workday";
                projectorManualEff.YAxisTitle = "Hatékonyság / Efficiency";
                projectorManualEff.XAxisLabel = AffectedManualWorkcenters;
                projectorManualEff.YAxisFormatter = value => value.ToString("P2");
                projectorManualEff.ChartData = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Hatékonyság / Efficiency",
                                Values = new ChartValues<decimal>(_kftEff),
                                DataLabels = true,
                            },

                            new LineSeries
                            {
                                Title = "Cél / Target",
                                Values = new ChartValues<int>(efficiencyTarget),
                            },
                        };
                projectorManualEff.XAxisRotation = "30";
                _charts.Add(projectorManualEff);

                ProjectorDataModel projectorHc = new ProjectorDataModel();
                projectorHc.Id = $"Létszám / Headcount";
                projectorHc.XAxisTitle = "Munkanap / Workday";
                projectorHc.YAxisTitle = "Létszám / Headcount";
                projectorHc.YAxisFormatter = value => value.ToString("N2");
                projectorHc.XAxisLabel = xAxisLabel;
                projectorHc.ChartData = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Aktuális létszám / Actual headcount",
                                Values = new ChartValues<int>(_followupDocument.Headcount.Where(s => s.ActualHC > 0).Select(s => s.ActualHC).ToList()),
                                DataLabels = true,
                            },

                            new LineSeries
                            {
                                Title = "Netto terv / Net headcount plan",
                                Values = new ChartValues<int>(_followupDocument.Headcount.Select(s => s.NettoHCPlan).ToList()),
                            },
                        };
                _charts.Add(projectorHc);

                if (_statusReportStopTimes != null)
                {
                    List<string> workcenters = new List<string> { "CYT", "AD AUTOMATA PUNCHING", "APIM", "SS AUTOMATA BENDING" };
                    List<string> stopCodes = new List<string> { "GEPHIBA" };
                    ProjectorDataModel statusReportStopTimes = new ProjectorDataModel();
                    statusReportStopTimes.Id = "Géphiba / Machine down time";
                    //statusReportStopTimes.XAxisTitle = "Munkanap / Workday";
                    statusReportStopTimes.YAxisTitle = "Állásidő / Down time";
                    statusReportStopTimes.XAxisLabel = stopCodes;
                    statusReportStopTimes.YAxisFormatter = value => value.ToString("N2");
                    //AffectedManualWorkcenters = workcenters.ToList();

                    statusReportStopTimes.ChartData = new SeriesCollection
                    {
                        new ColumnSeries
                        {
                            Title = "CYT",
                            Values = new ChartValues<decimal>(_statusReportStopTimes.Where(s => s.Workcenter == "CYT").Where(s => s.StopCode == "GEPHIBA").Select(s => s.TtlStopTime)),
                        },
                        new ColumnSeries
                        {
                            Title = "AD AUTOMATA PUNCHING",
                            Values = new ChartValues<decimal>(_statusReportStopTimes.Where(s => s.Workcenter == "AD AUTOMATA PUNCHING").Where(s => s.StopCode == "GEPHIBA").Select(s => s.TtlStopTime)),
                        },
                        new ColumnSeries
                        {
                            Title = "APIM",
                            Values = new ChartValues<decimal>(_statusReportStopTimes.Where(s => s.Workcenter == "APIM").Where(s => s.StopCode == "GEPHIBA").Select(s => s.TtlStopTime)),
                        },
                        new ColumnSeries
                        {
                            Title = "SS AUTOMATA BENDING",
                            Values = new ChartValues<decimal>(_statusReportStopTimes.Where(s => s.Workcenter == "SS AUTOMATA BENDING").Where(s => s.StopCode == "GEPHIBA").Select(s => s.TtlStopTime)),
                        }
                    };

                    _charts.Add(statusReportStopTimes);
                }
            }
        }


        public static PisSetup LoadSetupData()
        {
            //var emailSetupData = new ObservableCollection<PisSetup>();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            return databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();
        }

        #endregion
    }
}
