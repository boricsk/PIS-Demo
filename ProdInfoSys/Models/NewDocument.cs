using ProdInfoSys.Models.ErpDataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models
{
    public class NewDocument
    {
        /// <summary>
        /// A followup dokumentum felépítéséhez szükséges adatok az új dolumentum létrehozásáról
        /// </summary>
        public ObservableCollection<ErpMachineCenter> MachineCenters { get; set; }
        public string PlanName { get; set; }
        public string DocumentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Workdays { get; set; }
        public decimal ManualShiftLength { get; set; }
        public decimal MachineShiftLength { get; set; }
        public int ManualShiftNumber { get; set; }
        public int MachineShiftNumber { get; set; }
        public double AbsenseRatio { get; set; }
        public int HeadcountPlanNet { get; set; }
        public int HeadcountPlanBr { get; set; }
        public string Description { get; set; }
        public List<DateOnly> WorkdayList {get; set;}

    }
}
