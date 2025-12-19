using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
    /// <summary>
    /// Represents a data model for tracking and reporting headcount and related workforce metrics for a specific
    /// workday, supporting property and collection change notifications.
    /// </summary>
    /// <remarks>This class implements the INotifyPropertyChanged interface to support data binding scenarios,
    /// such as those found in UI frameworks. It exposes a range of properties for planned and actual headcount,
    /// absences, shift information, and calculated workforce statistics. Property changes for key metrics automatically
    /// trigger notifications for dependent calculated properties, ensuring that consumers receive up-to-date values.
    /// The class is suitable for use in applications that require dynamic updates to headcount and shift-related data,
    /// such as workforce planning or HR analytics tools.</remarks>
    public class HeadCountFollowupDocument : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            // Ha ezen oszlopok értékei változnak
            if (propertyName == nameof(ActualHC) || 
                propertyName == nameof(Indirect) ||
                propertyName == nameof(NettoHCPlan) ||              
                propertyName == nameof(Subcontactor) ||              
                propertyName == nameof(HCPlan) ||              
                propertyName == nameof(QAIndirect) ||              
                propertyName == nameof(Holiday) ||              
                propertyName == nameof(Others) ||              
                propertyName == nameof(Sick)              
                ) 
                {
                    // akkor frissítenie kéne az összegeket
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectPlusIndirect)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Diff)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalcActualSH)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalcPlannedSH)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbsebseTotal)));
                }
        }
        public int DirectPlusIndirect => Indirect + ActualHC;
        public int Diff => ActualHC - NettoHCPlan;
        public int AbsebseTotal => Sick + Holiday;
        public double CalcActualSH => (double)ActualHC * (double)ShiftNum * (double)ShiftLen;
        public double CalcPlannedSH => (double)NettoHCPlan * (double)ShiftNum * (double)ShiftLen;        

        private DateOnly _workday;
        public DateOnly Workday
        {
            get => _workday;
            set { _workday = value; OnPropertyChanged(); }
        }

        private int _numberOfWorkdays;
        public int NumberOfWorkdays
        {
            get => _numberOfWorkdays;
            set { _numberOfWorkdays = value; OnPropertyChanged(); }
        }

        private int _nettoHCPlan;
        public int NettoHCPlan
        {
            get => _nettoHCPlan;
            set { _nettoHCPlan = value; OnPropertyChanged(); }
        }

        private int _comulatedHCPlanNet;
        public int ComulatedHCPlanNet
        {
            get => _comulatedHCPlanNet;
            set { _comulatedHCPlanNet = value; OnPropertyChanged(); }
        }

        private int _hcPlan;
        public int HCPlan
        {
            get => _hcPlan;
            set { _hcPlan = value; OnPropertyChanged(); }
        }

        private int _subcontactor;
        public int Subcontactor
        {
            get => _subcontactor;
            set { _subcontactor = value; OnPropertyChanged(); }
        }

        private int _others;
        public int Others
        {
            get => _others;
            set { _others = value; OnPropertyChanged(); }
        }

        private int _actualHC;
        public int ActualHC
        {
            get => _actualHC;
            set
            {
                if (_actualHC != value)
                {
                    _actualHC = value; OnPropertyChanged(); 
                }
            }
        }

        private int _Indirect;
        public int Indirect
        {
            get => _Indirect;
            set { _Indirect = value; OnPropertyChanged(); }
        }

        private int _qaIndirect;
        public int QAIndirect
        {
            get => _qaIndirect;
            set { _qaIndirect = value; OnPropertyChanged(); }
        }

        private int _ffcDirectIndirect;
        public int FFCDirectIndirect
        {
            get => _ffcDirectIndirect;
            set { _ffcDirectIndirect = value; OnPropertyChanged(); }
        }

        private int _actualHCComulated;
        public int ActualHCComulated
        {
            get => _actualHCComulated;
            set { _actualHCComulated = value; OnPropertyChanged(); }
        }

        private int _actualHCDaily;
        public int ActualHCDaily
        {
            get => _actualHCDaily;
            set { _actualHCDaily = value; OnPropertyChanged(); }
        }

        private int _dailyHCPlanAvg;
        public int DailyHCPlanAvg
        {
            get => _dailyHCPlanAvg;
            set
            {
                if (_dailyHCPlanAvg != value)
                {
                    _dailyHCPlanAvg = value; OnPropertyChanged();
                }
            }
        }

        private int _holiday;
        public int Holiday
        {
            get => _holiday;
            set { _holiday = value; OnPropertyChanged(); }
        }

        private int _sick;
        public int Sick
        {
            get => _sick;
            set { _sick = value; OnPropertyChanged(); }
        }

        private int _difference;
        public int Difference
        {
            get => _difference;
            set { _difference = value; OnPropertyChanged(); }
        }

        private decimal _availWorkingHour;
        public decimal AvailWorkingHour
        {
            get => _availWorkingHour;
            set { _availWorkingHour = value; OnPropertyChanged(); }
        }

        private double _plannedSH;
        public double PlannedSH
        {
            get => _plannedSH;
            set { _plannedSH = value; OnPropertyChanged(); }
        }

        private double _actualSH;
        public double ActualSH
        {
            get => _actualSH;
            set { _actualSH = value; OnPropertyChanged(); }
        }

        private int _shiftNum;
        public int ShiftNum
        {
            get => _shiftNum;
            set { _shiftNum = value; OnPropertyChanged(); }
        }

        private decimal _shiftLen;
        public decimal ShiftLen
        {
            get => _shiftLen;
            set { _shiftLen = value; OnPropertyChanged(); }
        }

    }
}
