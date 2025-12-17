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
