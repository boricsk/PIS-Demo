using ProdInfoSys.Enums;

namespace ProdInfoSys.Models.ErpDataModels
{
    /// <summary>
    /// Represents a machine center in an ERP system, including its identification, operational parameters, costs, and
    /// planning attributes.
    /// </summary>
    /// <remarks>Use this class to model the properties and configuration of a machine center for
    /// manufacturing planning, costing, and scheduling scenarios. The properties provide information about the
    /// machine's capacity, efficiency, costs, and status, which can be used in production planning and analysis
    /// workflows.</remarks>
    public class ErpMachineCenter
    {
        public string Workcenter { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public decimal DirectUnitCost { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Capacity { get; set; }
        public decimal Efficiency { get; set; }
        public bool Blocked { get; set; }
        public decimal SetupTime { get; set; }
        public decimal WaitTime { get; set; }
        public decimal MoveTime { get; set; }
        public decimal FixedScrapQty { get; set; }
        public decimal Scrap { get; set; }
        public decimal ParalellCaps { get; set; }
        public decimal ShiftNum { get; set; }
        public EnumMachineType MachineType { get; set; }
        public decimal ShiftLength { get; set; }
        public bool IsPlanFixHc { get; set; }
        public decimal FixHc { get; set; }
        public decimal CostSheetUnitCost { get; set; }
        public decimal ScrapRatio { get; set; }
        public int DailyPlan { get; set; } = 0;
        public bool IsSelected { get; set; } = false;

    }
}
