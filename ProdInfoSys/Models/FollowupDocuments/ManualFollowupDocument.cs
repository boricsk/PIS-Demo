using ProdInfoSys.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProdInfoSys.Models.FollowupDocuments
{
    /// <summary>
    /// Represents a manual follow-up document containing production output, reject, and productivity data for multiple
    /// shifts and workdays. Supports property change notification for data binding scenarios.
    /// </summary>
    /// <remarks>This class is typically used to track and aggregate production and quality metrics across
    /// different shifts, including planned and actual outputs, reject quantities, and productivity figures. It
    /// implements <see cref="INotifyPropertyChanged"/> to support UI data binding and notifies listeners when property
    /// values change. Calculated properties provide convenient access to aggregated totals and ratios based on the
    /// underlying shift-level data.</remarks>
    public class ManualFollowupDocument : INotifyPropertyChanged, IHasFieldFollowupDoc, IHasFieldManualFollowupDoc
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by the implementation of the INotifyPropertyChanged
        /// interface to notify subscribers that a property value has changed. Handlers receive the name of the property
        /// that changed in the event data.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property to notify listeners of a property value change.
        /// </summary>
        /// <remarks>If the changed property affects calculated summary properties, this method also
        /// raises PropertyChanged events for those dependent properties to ensure that data bindings are updated
        /// appropriately.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and is automatically provided when called from
        /// a property setter.</param>
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtuputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubconOtuputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RejectSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputDifference)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalcRejectRatio)));
                TTLOutput = OutputSum;
            }
        }

        public int OtuputSum => Shift1Output + Shift2Output + Shift3Output;
        public int SubconOtuputSum => Shift1SubconOutput + Shift2SubconOutput + Shift3SubconOutput;
        public int OutputSum => OtuputSum + SubconOtuputSum;
        public int OutputDifference => OutputSum - DailyPlan;
        public int RejectSum => Shift1Reject + Shift2Reject + Shift3Reject;
        public int SubconRejectSum => Shift1SubconReject + Shift2SubconReject + Shift3SubconReject;
        public int RejectSumSubconOwn => RejectSum + SubconRejectSum;
        public double CalcRejectRatio => (OutputSum + RejectSumSubconOwn) == 0 ? 0 : (double)RejectSumSubconOwn / (OutputSum + RejectSumSubconOwn);


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

        private decimal _productivity;
        public decimal Productivity { get => _productivity; set { _productivity = value; OnPropertyChanged(); } }

        private string _prodComment = string.Empty;
        public string ProdComment { get => _prodComment; set { _prodComment = value; OnPropertyChanged(); } }

        private decimal _productivitySubcon;
        public decimal ProductivitySubcon { get => _productivitySubcon; set { _productivitySubcon = value; OnPropertyChanged(); } }

        private string _prodCommentSubcon = string.Empty;
        public string ProdCommentSubcon { get => _prodCommentSubcon; set { _prodCommentSubcon = value; OnPropertyChanged(); } }

    }
}
