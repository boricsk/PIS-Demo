using LiveCharts;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents the data model for a chart projector, including axis titles, labels, formatting, and chart data.
    /// </summary>
    /// <remarks>This model is typically used to supply data and configuration for charting components that
    /// display series-based data with labeled axes. The properties provide both the raw data and the necessary metadata
    /// for rendering charts, such as axis titles and label formatting.</remarks>
    public class ProjectorDataModel
    {
        public string Id { get; set; }
        public string XAxisTitle { get; set; }
        public List<string> XAxisLabel { get; set; }
        public string YAxisTitle { get; set; }
        public Func<double, string> YAxisFormatter { get; set; }
        public SeriesCollection ChartData { get; set; }
    }
}
