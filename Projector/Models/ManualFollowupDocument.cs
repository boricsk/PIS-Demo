using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class ManualFollowupDocument : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // Ha ezen oszlopok értékei változnak
            if (propertyName == nameof(Shift1KftOutput) ||
                propertyName == nameof(Shift2KftOutput) ||
                propertyName == nameof(Shift3KftOutput) ||
                propertyName == nameof(Shift1SubconOutput) ||
                propertyName == nameof(Shift2SubconOutput) ||
                propertyName == nameof(Shift3SubconOutput) ||
                propertyName == nameof(SupplierReject) ||
                propertyName == nameof(Shift1KftReject) ||
                propertyName == nameof(Shift2KftReject) ||
                propertyName == nameof(Shift3KftReject) ||
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

        public int KftOtuputSum => Shift1KftOutput + Shift2KftOutput + Shift3KftOutput;
        public int SubconOtuputSum => Shift1SubconOutput + Shift2SubconOutput + Shift3SubconOutput;
        public int OutputSum => KftOtuputSum + SubconOtuputSum;
        public int OutputDifference => DailyPlan - OutputSum;
        public int KftRejectSum => Shift1KftReject + Shift2KftReject + Shift3KftReject;
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

        private int _shift1KftOutput;
        public int Shift1KftOutput { get => _shift1KftOutput; set { _shift1KftOutput = value; OnPropertyChanged(); } }

        private int _shift2KftOutput;
        public int Shift2KftOutput { get => _shift2KftOutput; set { _shift2KftOutput = value; OnPropertyChanged(); } }

        private int _shift3KftOutput;
        public int Shift3KftOutput { get => _shift3KftOutput; set { _shift3KftOutput = value; OnPropertyChanged(); } }

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

        private int _shift1KftReject;
        public int Shift1KftReject { get => _shift1KftReject; set { _shift1KftReject = value; OnPropertyChanged(); } }

        private int _shift2KftReject;
        public int Shift2KftReject { get => _shift2KftReject; set { _shift2KftReject = value; OnPropertyChanged(); } }

        private int _shift3KftReject;
        public int Shift3KftReject { get => _shift3KftReject; set { _shift3KftReject = value; OnPropertyChanged(); } }

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
