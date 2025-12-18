using ProdInfoSys.Interfaces;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProdInfoSys.Models.FollowupDocuments
{
    /// <summary>
    /// Represents a headcount follow-up document that tracks planned and actual workforce data, including attendance,
    /// absences, and shift information for a specific workday. Supports property and collection change notifications
    /// for data binding scenarios.
    /// </summary>
    /// <remarks>This class is typically used in workforce planning or reporting applications to monitor and
    /// compare planned versus actual headcount and working hours. It implements both INotifyPropertyChanged and
    /// collection change notifications, making it suitable for use with data-bound UI frameworks such as WPF or
    /// Xamarin.Forms. Changes to key properties automatically trigger updates to related calculated properties,
    /// ensuring that dependent values remain consistent.</remarks>
    public class HeadCountFollowupDocument : INotifyPropertyChanged, IHasFieldHCFollowupDoc
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by calling the OnPropertyChanged method after a
        /// property value is modified. Subscribers can use this event to respond to changes in property values, such as
        /// updating user interface elements in data-binding scenarios.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when the collection changes, such as when items are added, removed, or the entire list is refreshed.
        /// </summary>
        /// <remarks>Subscribers are notified whenever the collection is modified. The event provides
        /// information about the type of change and the affected items. This event is typically used to update user
        /// interfaces or synchronize data when the underlying collection changes.</remarks>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in a property setter to notify subscribers that the property value
        /// has changed. If the changed property is one of ActualHC, Indirect, NettoHCPlan, Subcontactor, HCPlan,
        /// QAIndirect, Holiday, Others, or Sick, this method also raises PropertyChanged for related calculated
        /// properties to ensure that dependent bindings are updated.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and is automatically supplied by the compiler
        /// when called from a property setter.</param>
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

        private int _DirectIndirect;
        public int DirectIndirect
        {
            get => _DirectIndirect;
            set { _DirectIndirect = value; OnPropertyChanged(); }
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
