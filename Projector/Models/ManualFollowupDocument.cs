using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    /// <summary>
    /// Represents a manual follow-up document containing production, output, and reject data for a specific workday,
    /// including shift-level and cumulative metrics.
    /// </summary>
    /// <remarks>This class is typically used in manufacturing or production tracking scenarios to record and
    /// monitor daily and cumulative output, reject quantities, and related comments. It implements the
    /// INotifyPropertyChanged interface to support data binding and automatic UI updates when property values change.
    /// Calculated properties provide aggregate values such as total output, reject sums, and reject ratios based on the
    /// underlying shift and supplier data.</remarks>
    public class ManualFollowupDocument : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // Ha ezen oszlopok értékei változnak
            if (propertyName == nameof(Shift1Output) ||
                propertyName == nameof(Shift2Output) ||
                propertyName == nameof(Shift3Output) ||
                propertyName == nameof(Shift1SubconOutput) ||
                propertyName == nameof(Shift2SubconOutput) ||
                propertyName == nameof(Shift3SubconOutput) ||
                propertyName == nameof(SupplierReject) ||
                propertyName == nameof(Shift1Reject) ||
                propertyName == nameof(Shift2Reject) ||
                propertyName == nameof(Shift3Reject) ||
                propertyName == nameof(Shift1SubconReject) ||
                propertyName == nameof(Shift2SubconReject) ||
                propertyName == nameof(Shift3SubconReject) ||
                propertyName == nameof(CommentForRejects)
                )
            {
                // akkor frissítenie kéne az összegeket
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KftOtuputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubconOtuputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RejectSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputDifference)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalcRejectRatio)));
                TTLOutput = OutputSum;
            }
        }

        public int KftOtuputSum => Shift1Output + Shift2Output + Shift3Output;
        public int SubconOtuputSum => Shift1SubconOutput + Shift2SubconOutput + Shift3SubconOutput;
        public int OutputSum => KftOtuputSum + SubconOtuputSum;
        public int OutputDifference => DailyPlan - OutputSum;
        public int KftRejectSum => Shift1Reject + Shift2Reject + Shift3Reject;
        public int SubconRejectSum => Shift1SubconReject + Shift2SubconReject + Shift3SubconReject;
        public int RejectSum => KftRejectSum + SubconRejectSum;
        public double CalcRejectRatio => (OutputSum + RejectSum) == 0 ? 0 : (double)RejectSum / (OutputSum + RejectSum);


        private DateOnly _workday;
        public DateOnly Workday { get => _workday; set { _workday = value; OnPropertyChanged(); } }

        private int _numberOfWorkdays;
        public int NumberOfWorkdays { get => _numberOfWorkdays; set { _numberOfWorkdays = value; OnPropertyChanged(); } }

        private int _dailyPlan;
        public int DailyPlan { get => _dailyPlan; set { _dailyPlan = value; OnPropertyChanged(); } }

        private int _comulatedPlan;
        public int ComulatedPlan { get => _comulatedPlan; set { _comulatedPlan = value; OnPropertyChanged(); } }

        private int _comulatedOutput;
        public int ComulatedOutput { get => _comulatedOutput; set { _comulatedOutput = value; OnPropertyChanged(); } }

        private int _ttlOutput;
        public int TTLOutput { get => _ttlOutput; set { _ttlOutput = value; OnPropertyChanged(); } }

        private int _shift1Output;
        public int Shift1Output { get => _shift1Output; set { _shift1Output = value; OnPropertyChanged(); } }

        private int _shift2Output;
        public int Shift2Output { get => _shift2Output; set { _shift2Output = value; OnPropertyChanged(); } }

        private int _shift3Output;
        public int Shift3Output { get => _shift3Output; set { _shift3Output = value; OnPropertyChanged(); } }

        private int _shift1SubconOutput;
        public int Shift1SubconOutput { get => _shift1SubconOutput; set { _shift1SubconOutput = value; OnPropertyChanged(); } }

        private int _shift2SubconOutput;
        public int Shift2SubconOutput { get => _shift2SubconOutput; set { _shift2SubconOutput = value; OnPropertyChanged(); } }

        private int _shift3SubconOutput;
        public int Shift3SubconOutput { get => _shift3SubconOutput; set { _shift3SubconOutput = value; OnPropertyChanged(); } }

        private int _ttlRejectQty;
        public int TTLRejectQty { get => _ttlRejectQty; set { _ttlRejectQty = value; OnPropertyChanged(); } }

        private decimal _rejectRatio;
        public decimal RejectRatio { get => _rejectRatio; set { _rejectRatio = value; OnPropertyChanged(); } }

        private int _shift1Reject;
        public int Shift1Reject { get => _shift1Reject; set { _shift1Reject = value; OnPropertyChanged(); } }

        private int _shift2Reject;
        public int Shift2Reject { get => _shift2Reject; set { _shift2Reject = value; OnPropertyChanged(); } }

        private int _shift3Reject;
        public int Shift3Reject { get => _shift3Reject; set { _shift3Reject = value; OnPropertyChanged(); } }

        private int _shift1SubconReject;
        public int Shift1SubconReject { get => _shift1SubconReject; set { _shift1SubconReject = value; OnPropertyChanged(); } }

        private int _shift2SubconReject;
        public int Shift2SubconReject { get => _shift2SubconReject; set { _shift2SubconReject = value; OnPropertyChanged(); } }

        private int _shift3SubconReject;
        public int Shift3SubconReject { get => _shift3SubconReject; set { _shift3SubconReject = value; OnPropertyChanged(); } }

        private int _supplierReject;
        public int SupplierReject { get => _supplierReject; set { _supplierReject = value; OnPropertyChanged(); } }

        private int _bs;
        public int BS { get => _bs; set { _bs = value; OnPropertyChanged(); } }

        private int _manufacturingReject;
        public int ManufacturingReject { get => _manufacturingReject; set { _manufacturingReject = value; OnPropertyChanged(); } }

        private string _commentForRejects = string.Empty;
        public string CommentForRejects { get => _commentForRejects; set { _commentForRejects = value; OnPropertyChanged(); } }

        private decimal _productivityKft;
        public decimal ProductivityKft { get => _productivityKft; set { _productivityKft = value; OnPropertyChanged(); } }

        private string _prodCommentKft = string.Empty;
        public string ProdCommentKft { get => _prodCommentKft; set { _prodCommentKft = value; OnPropertyChanged(); } }

        private decimal _productivitySubcon;
        public decimal ProductivitySubcon { get => _productivitySubcon; set { _productivitySubcon = value; OnPropertyChanged(); } }

        private string _prodCommentSubcon = string.Empty;
        public string ProdCommentSubcon { get => _prodCommentSubcon; set { _prodCommentSubcon = value; OnPropertyChanged(); } }

    }
}
