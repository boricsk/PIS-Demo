using CalendarManagement;
using Dapper;
using Microsoft.Data.SqlClient;
using MongoDB.Bson;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Enums;
using ProdInfoSys.Models;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.FollowupDocuments;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace ProdInfoSys.ViewModels
{
    public class AddNewDocumentViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private readonly IUserDialogService _dialogs;
        #endregion

        #region PropChangedInterface

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communications
        //Progress indicator visibility
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        //Termelési tervek neveinek listája
        private ObservableCollection<string>? _nameOfPlans;
        public ObservableCollection<string>? NameOfPlans
        {
            get => _nameOfPlans;
            set { _nameOfPlans = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ErpMachineCenter>? _erpMachineCenters;
        public ObservableCollection<ErpMachineCenter>? ErpMachineCenters
        {
            get => _erpMachineCenters;
            set { _erpMachineCenters = value; OnPropertyChanged(); }
        }

        private NewDocument? _newFollowupDocument;
        public NewDocument? NewFollowupDocument
        {
            get => _newFollowupDocument;
            set { _newFollowupDocument = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DateOnly> _additionalWorkdays;
        public ObservableCollection<DateOnly> AdditionalWorkdays
        {
            get => _additionalWorkdays;
            set { _additionalWorkdays = value; OnPropertyChanged(); }
        }

        private DateOnly _selectedAdditionalDate;
        public DateOnly SelectedAdditionalDate
        {
            get => _selectedAdditionalDate;
            set { _selectedAdditionalDate = value; OnPropertyChanged(); }
        }

        private DateTime _selectedPickerDate;
        public DateTime SelectedPickerDate
        {
            get => _selectedPickerDate;
            set { _selectedPickerDate = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand AddNewDocument => new ProjectCommandRelay(_ => AddingNewDocument());
        private void AddingNewDocument()
        {
            try
            {
                if (string.IsNullOrEmpty(NewFollowupDocument.DocumentName))
                {
                    //MessageBox.Show($"Nincs név adva a dokumentumnak!", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                    _dialogs.ShowErrorInfo($"Nincs név adva a dokumentumnak!", "AddNewDocumentViewModel");
                    return;
                }
                CalendarMgmnt calendar = new CalendarMgmnt(additionalWorkdays: AdditionalWorkdays.ToList(), movedWorkDays: SetupManagement.LoadTrWorkdays());
                (int workDay, int holiday) workayNumbers = (0, 0);
                if (NewFollowupDocument is not null && _erpMachineCenters is not null)
                {
                    if (NewFollowupDocument.StartDate != NewFollowupDocument.FinishDate && NewFollowupDocument.FinishDate > NewFollowupDocument.StartDate)
                    {
                        workayNumbers = calendar.CountDays(
                            DateOnly.FromDateTime(NewFollowupDocument.StartDate),
                            DateOnly.FromDateTime(NewFollowupDocument.FinishDate));
                        NewFollowupDocument.Workdays = workayNumbers.workDay;

                        NewFollowupDocument.WorkdayList = calendar.GetWorkdays(
                            DateOnly.FromDateTime(NewFollowupDocument.StartDate),
                            DateOnly.FromDateTime(NewFollowupDocument.FinishDate));
                    }
                    else
                    {
                        //MessageBox.Show($"A dátumtartomány hibásan van megadva!", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                        _dialogs.ShowErrorInfo($"A dátumtartomány hibásan van megadva!", "AddNewDocumentViewModel");
                        return;
                    }

                    var affectedMachineCenters = GetCheckedItems();

                    if (affectedMachineCenters.Count != 0)
                    {
                        NewFollowupDocument.MachineCenters = affectedMachineCenters;
                    }
                    else
                    {
                        //MessageBox.Show($"Nincsennek gépcsoportok kiválasztva!", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                        _dialogs.ShowErrorInfo($"Nincsennek gépcsoportok kiválasztva!", "AddNewDocumentViewModel");
                        return;
                    }

                    MongoDbModel mdb = new MongoDbModel(NewFollowupDocument);
                    mdb.MakeDataModell(_erpMachineCenters.ToList());
                }
                else
                {
                    //MessageBox.Show($"Adatbetöltési hiba az ErpMachineCenters vagy a NewFollowupDocument esetén!", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                    _dialogs.ShowErrorInfo($"Adatbetöltési hiba az ErpMachineCenters vagy a NewFollowupDocument esetén!", "AddNewDocumentViewModel");
                }
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"", "Followup betöltés");
            }
        }

        public ICommand AddAdditionalDate => new ProjectCommandRelay(_ => AddingAdditionalDate());
        private void AddingAdditionalDate()
        {

            if (_selectedPickerDate.DayOfWeek != DayOfWeek.Sunday && _selectedPickerDate.DayOfWeek != DayOfWeek.Saturday)
            {
                //MessageBox.Show($"Csak hétvége választható ki!", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                _dialogs.ShowErrorInfo($"Csak hétvége választható ki!", "AddNewDocumentViewModel");
                return;
            }

            if (AdditionalWorkdays.Contains(DateOnly.FromDateTime(_selectedPickerDate)))
            {
                //MessageBox.Show($"Ez a nap már a listában van!", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                _dialogs.ShowErrorInfo($"Ez a nap már a listában van!", "AddNewDocumentViewModel");
                return;
            }

            AdditionalWorkdays.Add(DateOnly.FromDateTime(_selectedPickerDate));

        }

        public ICommand RemoveAdditionalDate => new ProjectCommandRelay(_ => RemovingAdditionalDate());
        private void RemovingAdditionalDate()
        {
            if (_selectedAdditionalDate != null)
            {
                AdditionalWorkdays.Remove(_selectedAdditionalDate);
            }
        }
        #endregion

        #region Constructor
        public AddNewDocumentViewModel(IUserDialogService dialogs)
        {
            _dialogs = dialogs;
            try
            {
                _ = LoadDataWithDapper();
                NewFollowupDocument = new();
                AdditionalWorkdays = new();
                SetDefaults();
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"{ex.Message}", "AddNewDocumentViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                _dialogs.ShowErrorInfo($"{ex.Message}", "AddNewDocumentViewModel");
            }
            finally
            {

            }            
        }
        #endregion

        #region Private methods
        private void SetDefaults()
        {
            _selectedPickerDate = DateTime.Now;
            NewFollowupDocument.StartDate = DateTime.Today;
            NewFollowupDocument.FinishDate = DateTime.Today;
            NewFollowupDocument.Workdays = 21;
            NewFollowupDocument.ManualShiftLength = 7.33m;
            NewFollowupDocument.MachineShiftLength = 8m;
            NewFollowupDocument.MachineShiftNumber = 1;
            NewFollowupDocument.ManualShiftNumber = 1;
            NewFollowupDocument.AbsenseRatio = 13;
        }
        /// <summary>
        /// Asynchronously loads data using Dapper and updates the relevant collections and properties.
        /// </summary>
        /// <remarks>This method retrieves ERP machine center data and production plans using Dapper.  It
        /// updates the <see cref="_erpMachineCenters"/> collection with the retrieved machine centers  and populates
        /// the <see cref="_nameOfPlans"/> collection with distinct plan names.  The method also raises property change
        /// notifications for <see cref="ErpMachineCenters"/>  and <see cref="NameOfPlans"/> to ensure data binding
        /// updates.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadDataWithDapper()
        {
            _isLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    DapperFunctions df = new DapperFunctions();
                    _erpMachineCenters = df.GetErpMachineCenters();
                    var plans = df.GetProductionPlan();
                    _nameOfPlans = new ObservableCollection<string>(plans.Select(p => p.Plan_Name).Distinct().ToList());
                    OnPropertyChanged(nameof(ErpMachineCenters));
                    OnPropertyChanged(nameof(NameOfPlans));
                });
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"Hiba történt : {ex.Message}", "Termelési terv betöltés");
            }
            finally
            {
                IsLoading = false;
            }
        }
        /// <summary>
        /// Retrieves a collection of machine centers that are marked as selected.
        /// </summary>
        /// <remarks>This method filters the <see cref="ErpMachineCenters"/> collection to include only
        /// those items  where the <c>IsSelected</c> property is set to <see langword="true"/>. If the <see
        /// cref="ErpMachineCenters"/>  collection is <see langword="null"/>, the method returns <see
        /// langword="null"/>.</remarks>
        /// <returns>An <see cref="ObservableCollection{T}"/> containing the selected machine centers, or  <see langword="null"/>
        /// if <see cref="ErpMachineCenters"/> is <see langword="null"/>.</returns>
        private ObservableCollection<ErpMachineCenter> GetCheckedItems()
        {
            if (ErpMachineCenters is not null)
            {
                return new ObservableCollection<ErpMachineCenter>(ErpMachineCenters.Where(i => i.IsSelected).ToList());
            }
            return null;
        }
        #endregion
    }
}
