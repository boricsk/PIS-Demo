using LiveCharts;
using LiveCharts.Wpf;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents the data model for a projector chart, including axis titles, labels, formatting, and chart data.
    /// </summary>
    /// <remarks>This model is typically used to supply data and configuration for rendering projector-style
    /// charts in data visualization scenarios. It encapsulates axis metadata, label formatting, and the underlying
    /// chart data series.</remarks>
    public class ProjectorDataModel
    {
        public string Id { get; set; }
        public string XAxisTitle { get; set; } 
        public List<string> XAxisLabel { get; set; } 
        public string YAxisTitle { get; set; }   
        public Func<double, string> YAxisFormatter { get; set; }
        public SeriesCollection ChartData { get; set; }
        public string XAxisRotation {get; set; } = "0";
    }
}
