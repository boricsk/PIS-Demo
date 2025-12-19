using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a master follow-up document containing scheduling, staffing, and status information for a production
    /// plan or project.
    /// </summary>
    /// <remarks>This class aggregates key details such as document and plan names, scheduling dates, workday
    /// calculations, shift information, headcount, machine and manual follow-ups, inspections, and status reports. It
    /// is typically used to track and manage the progress and resources associated with a manufacturing or operational
    /// plan. All properties are intended for data storage and retrieval; business logic should be implemented
    /// externally.</remarks>
    public class MasterFollowupDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        [BsonElement("Document name")]
        public string DocumentName { get; set; }

        [BsonElement("Plan name")]
        public string PlanName { get; set; }
        [BsonElement("Start date")]
        public string StartDate { get; set; }

        [BsonElement("Finish date")]
        public string FinishDate { get; set; }

        [BsonElement("Workdays")]
        public int Workdays { get; set; }

        [BsonElement("Shift Length (Manual)")]
        public decimal ShiftLenManual { get; set; }

        [BsonElement("Shift Length (Machine)")]
        public decimal ShiftLenMachine { get; set; }

        [BsonElement("Shift Number (Manual)")]
        public int ShiftNumberManual { get; set; }

        [BsonElement("Shift Number (Machine)")]
        public int ShiftNumMachine { get; set; }

        [BsonElement("Absense ratio")]
        public double AbsenseRatio { get; set; }

        [BsonElement("Additional workdays")]
        public List<DateOnly> AdditionalWorkdays { get; set; }

        [BsonElement("Headcount")]
        public List<HeadCountFollowupDocument> Headcount { get; set; } = new List<HeadCountFollowupDocument>();

        [BsonElement("Machine followup")]
        public List<Machines> MachineFollowups { get; set; }

        [BsonElement("Inspection followup")]
        public List<Inspection> InspectionFollowups { get; set; }

        [BsonElement("Manual followup")]
        public List<Manual> ManualFollowups { get; set; }

        [BsonElement("Status reports")]
        public List<StatusReport> StatusReports { get; set; }

        [BsonElement("Status reports QRQC")]
        public List<StatusReportQrqc> StatusReportsQRQC { get; set; }
    }
}
