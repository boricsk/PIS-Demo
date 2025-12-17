using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class InspectionFollowupDocument : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Ha ezen oszlopok értékei változnak
            if (propertyName == nameof(Shift1Output) ||
                propertyName == nameof(Shift1Reject) ||
                propertyName == nameof(Shift2Output) ||
                propertyName == nameof(Shift2Reject) ||
                propertyName == nameof(Shift3Output) ||
                propertyName == nameof(Shift3Reject) ||
                propertyName == nameof(SupplierReject) ||
                propertyName == nameof(OperatingHour) ||
                propertyName == nameof(CommentForRejects)
                )
            {
                // akkor frissítenie kéne az összegeket
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtuputSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RejectSum)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputDifference)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalcRejectRatio)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Utilization)));

                TTLOutput = OtuputSum;
            }
        }

        public int OtuputSum => Shift1Output + Shift2Output + Shift3Output;
        public int RejectSum => Shift1Reject + Shift2Reject + Shift3Reject;
        public int OutputDifference => DailyPlan - (Shift1Output + Shift2Output + Shift3Output);
        //Mivel int / int ezért nulla lesz a végeredmény, mert az osztás után konvertál double-ra.
        //Az egyik poerandusnak double-nak kell lenni.
        public double CalcRejectRatio => (OtuputSum + RejectSum) == 0 ? 0 : (double)RejectSum / (OtuputSum + RejectSum);
        public double Utilization => (AvailOperatingHour) == 0 ? 0 : OperatingHour / AvailOperatingHour;

        private DateOnly _workday;
        public DateOnly Workday { get => _workday; set { _workday = value; OnPropertyChanged(); } }

        private int _numberOfWorkdays;
        public int NumberOfWorkdays { get => _numberOfWorkdays; set { _numberOfWorkdays = value; OnPropertyChanged(); } }

        private int _dailyPlan;
        public int DailyPlan { get => _dailyPlan; set { _dailyPlan = value; OnPropertyChanged(); } }
        
        private int _comulatedPlan;
        public int ComulatedPlan
        {
            get => _comulatedPlan;
            set { _comulatedPlan = value; OnPropertyChanged(); }
        }

        private int _comulatedOutput;
        public int ComulatedOutput
        {
            get => _comulatedOutput;
            set { _comulatedOutput = value; OnPropertyChanged(); }
        }

        private int _ttlOutput;
        public int TTLOutput
        {
            get => _ttlOutput;
            set { _ttlOutput = value; OnPropertyChanged(); }
        }

        private int _shift1Output;
        public int Shift1Output
        {
            get => _shift1Output;
            set { _shift1Output = value; OnPropertyChanged(); }
        }

        private int _shift2Output;
        public int Shift2Output
        {
            get => _shift2Output;
            set { _shift2Output = value; OnPropertyChanged(); }
        }

        private int _shift3Output;
        public int Shift3Output
        {
            get => _shift3Output;
            set { _shift3Output = value; OnPropertyChanged(); }
        }

        private int _ttlRejectQty;
        public int TTLRejectQty
        {
            get => _ttlRejectQty;
            set { _ttlRejectQty = value; OnPropertyChanged(); }
        }

        private decimal _rejectRatio;
        public decimal RejectRatio
        {
            get => _rejectRatio;
            set { _rejectRatio = value; OnPropertyChanged(); }
        }

        private int _shift1Reject;
        public int Shift1Reject
        {
            get => _shift1Reject;
            set { _shift1Reject = value; OnPropertyChanged(); }
        }

        private int _shift2Reject;
        public int Shift2Reject
        {
            get => _shift2Reject;
            set { _shift2Reject = value; OnPropertyChanged(); }
        }

        private int _shift3Reject;
        public int Shift3Reject
        {
            get => _shift3Reject;
            set { _shift3Reject = value; OnPropertyChanged(); }
        }

        private int _supplierReject;
        public int SupplierReject
        {
            get => _supplierReject;
            set { _supplierReject = value; OnPropertyChanged(); }
        }

        private string _commentForRejects = string.Empty;
        public string CommentForRejects
        {
            get => _commentForRejects;
            set { _commentForRejects = value; OnPropertyChanged(); }
        }

        private double _availOperatingHour;
        public double AvailOperatingHour { get => _availOperatingHour; set { _availOperatingHour = value; OnPropertyChanged(); } }

        private double _operatingHour;
        public double OperatingHour { get => _operatingHour; set { _operatingHour = value; OnPropertyChanged(); UpdatePersistedUtil(); } }

        private double _persistedUtilization;
        public double PersistedUtilization
        {
            get => _persistedUtilization;
            set { _persistedUtilization = value; OnPropertyChanged(); }
        }

        private void UpdatePersistedUtil()
        {
            PersistedUtilization = Utilization;
        }
    }
}
