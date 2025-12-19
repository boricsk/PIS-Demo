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

namespace Projector.ViewModels
{
    /// <summary>
    /// Represents the view model for the main application window, providing data binding and logic for chart
    /// visualization, status reporting, and follow-up document management in a WPF application.
    /// </summary>
    /// <remarks>This view model implements the INotifyPropertyChanged interface to support property change
    /// notifications for data binding in WPF. It manages chart data, titles, axis formatting, and periodic updates
    /// using dispatcher timers. The MainWindowViewModel coordinates the retrieval and processing of follow-up documents
    /// and status reports, and exposes properties for use in the main window's UI. Thread safety is not guaranteed; all
    /// members are intended to be accessed from the UI thread.</remarks>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _parent = string.Empty;
        private MasterFollowupDocument? _followupDocument;
        private ObservableCollection<ProjectorDataModel>? _charts;
        private int _projectCounter = 0;
        private List<string> _affectedWorkcenters = new List<string>(); 

        private DispatcherTimer? _visualizationTimer;
        private DispatcherTimer? _updateTimer;
        private int _currentChartIndex = 0;
        private ObservableCollection<StatusReportMachineList> _statusReportMahines = new ObservableCollection<StatusReportMachineList>();
        private ObservableCollection<StopTime> _statusReportStopTimes = new ObservableCollection<StopTime>();

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>Subscribe to this event to receive notifications when a property on the object has
        /// changed. This event is typically raised by calling the OnPropertyChanged method after a property value is
        /// modified. Handlers receive the name of the property that changed in the event arguments.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in the setter of a property to notify subscribers that the
        /// property's value has changed. This is commonly used to support data binding in applications that implement
        /// the INotifyPropertyChanged interface.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and will be automatically provided when called
        /// from a property setter.</param>
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
        /// <summary>
        /// Initializes and starts the update timer and visualization timer for periodic data updates.
        /// </summary>
        /// <remarks>This method configures the update timer to trigger data updates at regular intervals.
        /// It should be called to ensure that both timers are properly initialized and running. Calling this method
        /// multiple times will reset the timers.</remarks>
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

        /// <summary>
        /// Performs a data update by retrieving follow-up documents and building chart data.
        /// </summary>
        private void RunDataUpdate()
        {

            GetFollowupDocument();
            BuildChartData();

        }

        /// <summary>
        /// Starts the timer-based loop that cycles through available charts for visualization.
        /// </summary>
        /// <remarks>This method initializes and starts a timer to periodically display the next chart in
        /// the collection. If there are no charts available, the method does nothing. This method is intended to be
        /// called when chart visualization should begin or resume.</remarks>
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

        /// <summary>
        /// Advances to the next chart in the collection and updates the chart-related properties to display its data.
        /// </summary>
        /// <remarks>If the end of the chart collection is reached, the method cycles back to the first
        /// chart. If there are no charts available, the method performs no action.</remarks>
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

        /// <summary>
        /// Loads the follow-up document and updates related status report collections for the current context.
        /// </summary>
        /// <remarks>This method retrieves a follow-up document based on the parent document name and
        /// updates internal collections with the latest status report and stop time data. If an error occurs during the
        /// loading process, an error message is displayed to the user.</remarks>
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
        /// <summary>
        /// Builds and populates the chart data collections based on the current follow-up document and affected
        /// workcenters.
        /// </summary>
        /// <remarks>This method clears any existing chart data and generates new chart series for output,
        /// efficiency, operation hours, headcount, and machine downtime. The generated charts reflect the current state
        /// of the follow-up document and are filtered by the set of affected workcenters. This method should be called
        /// whenever the underlying data changes to ensure that the chart data remains up to date.</remarks>
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

                var _kftEff = _statusReportMahines.Where(m => m.WorkcenterType == EnumMachineType.ManualProcess.ToString()).Select(m => m.AvgEfficiency).ToList();
                AffectedManualWorkcenters = _statusReportMahines.Where(m => m.WorkcenterType == EnumMachineType.ManualProcess.ToString()).Select(m => m.Workcenter).ToList();

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
                    statusReportStopTimes.YAxisTitle = "Állásidő / Down time";
                    statusReportStopTimes.XAxisLabel = stopCodes;
                    statusReportStopTimes.YAxisFormatter = value => value.ToString("N2");

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

        /// <summary>
        /// Retrieves the first available setup configuration from the PisSetup database collection.
        /// </summary>
        /// <returns>A <see cref="PisSetup"/> object representing the setup configuration if found; otherwise, <c>null</c>.</returns>
        public static PisSetup LoadSetupData()
        {
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<PisSetup>(conMgmnt.PisSetupDbName);
            return databaseCollection.Find(FilterDefinition<PisSetup>.Empty).FirstOrDefault();
        }

        #endregion
    }
}
