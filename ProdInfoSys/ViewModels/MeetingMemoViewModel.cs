using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Enums;
using ProdInfoSys.Models;
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
//using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ProdInfoSys.ViewModels
{
    public class MeetingMemoViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        #endregion

        ConnectionManagement conMgmnt = new ConnectionManagement();
        public List<string> StatusList { get; } = new() { "Nyitott", "Lezárt", "Nincs" };
        private List<MeetingMinutes> _allMemos = new List<MeetingMinutes>();


        #region PropChangedInterface
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication

        private MeetingMinutes _newMemo;
        public MeetingMinutes NewMemo
        {
            get => _newMemo;
            set { _newMemo = value; OnPropertyChanged(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); if (_searchText != null) { PerformSearch(_searchText); } }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                    FilterRecordsByDate();
                }
            }
        }
        private MeetingMinutes? _selectedItem;
        public MeetingMinutes? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<MeetingMinutes> _meetingMinutes;
        public ObservableCollection<MeetingMinutes> MeetingMinutes
        {
            get => _meetingMinutes;
            set => _meetingMinutes = value;
        }

        private string _taskDescription;
        public string TaskDescription
        {
            get => _taskDescription;
            set { _taskDescription = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand? SaveDocument => new ProjectCommandRelay(_ => SavingDocument());
        private void SavingDocument()
        {
            var ret = SaveAllDocumentToDatabase(_meetingMinutes);
            if (ret.isCompleted)
            {
                //MessageBox.Show($"A dokumentumok mentése sikeres!", "HeadcountViewModel", MessageBoxButton.OK, MessageBoxImage.Information);
                _dialogs.ShowInfo($"A dokumentumok mentése sikeres!", "MeetingMemoViewModel");
            }
            else
            {
                //MessageBox.Show($"A dokumentumok mentése sikertelen a következő hiba miatt : {ret.message}", "HeadcountViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
                _dialogs.ShowErrorInfo($"A dokumentumok mentése sikertelen a következő hiba miatt : {ret.message}", "MeetingMemoViewModel");
            }
        }

        public ICommand AddNewMemo => new ProjectCommandRelay(_ => AddingNewMemo());
        private void AddingNewMemo()
        {
            
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<MeetingMinutes>(conMgmnt.MeetingMemo);
            MongoDbOperations<MeetingMinutes> dbo = new MongoDbOperations<MeetingMinutes>(databaseCollection);
            MeetingMinutes _new = new()
            {
                date = DateTime.Today.AddHours(12),
                pic = _newMemo.pic,
                task = _newMemo.task,
                comment = _newMemo.comment,
                deadline = _newMemo.deadline,
                status = _newMemo.status
            };

            dbo.AddNewDocument(_new);
            _meetingMinutes.Add(_new);
            _allMemos.Add(_new);
           
            _newMemo.task = string.Empty;
            _newMemo.pic = string.Empty;
            _newMemo.comment = string.Empty;
            _newMemo.status = EnumTaskStatus.Nyitott.ToString();

            OnPropertyChanged(nameof(NewMemo));
            OnPropertyChanged(nameof(MeetingMinutes));
        }

        public ICommand AddNoEvent => new ProjectCommandRelay(_ => AddingNoEvent());
        private void AddingNoEvent()
        {
            _newMemo.date = DateTime.Today.AddHours(12);            
            _newMemo.deadline = DateTime.Today.AddHours(1);            
            _newMemo.pic = "-";
            _newMemo.task = "Nem történt semmi említésre méltó esemény.";
            _newMemo.comment = "-";
            _newMemo.status = EnumTaskStatus.Lezárt.ToString();
            OnPropertyChanged(nameof(NewMemo));
        }

        public ICommand DeleteSelectedMemo => new ProjectCommandRelay(_ => DeletingSelectedMemo());
        private void DeletingSelectedMemo()
        {
            if (_selectedItem != null)
            {
                //(MessageBox.Show($"Tényleg törölni szeretnéd a kijelölt dokumentumot? ", "Törlés", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                if (_dialogs.ShowConfirmation("Tényleg törölni szeretnéd a kijelölt dokumentumot? ", "Törlés"))
                {
                    DeleteDocumentFromDatabase();
                }
            }
        }
        #endregion

        #region Constructor
        public MeetingMemoViewModel(IUserDialogService dialogs)
        {
            _dialogs = dialogs;
            _newMemo = new();
            _newMemo.deadline = DateTime.Now;
            _newMemo.status = StatusList[0];
            GetMeetingMemoCollection();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Deletes the selected document from the database and updates the local collections.
        /// </summary>
        /// <remarks>This method attempts to delete the document identified by the selected item's ID from
        /// the database. If the operation is successful, the document is also removed from the local collections, and
        /// the <see cref="OnPropertyChanged"/> method is invoked to notify of changes. If an exception occurs during
        /// the operation, the method returns a failure status along with the exception message.</remarks>
        /// <returns>A tuple containing the following: <list type="bullet"> <item> <term><c>isCompleted</c></term>
        /// <description><see langword="true"/> if the document was successfully deleted; otherwise, <see
        /// langword="false"/>.</description> </item> <item> <term><c>message</c></term> <description>A message
        /// describing the result of the operation. If the operation fails, this contains the exception message;
        /// otherwise, it is empty.</description> </item> </list></returns>
        private (bool isCompleted, string message) DeleteDocumentFromDatabase()
        {
            (bool isCompleted, string message) ret = (false, string.Empty);
            try
            {
                conMgmnt.ConnectToDatabase();

                var collection = conMgmnt.GetCollection<MeetingMinutes>(conMgmnt.MeetingMemo);


                var filter = Builders<MeetingMinutes>.Filter.Eq(x => x.id, _selectedItem.id);
                collection.DeleteOne(filter);
                _meetingMinutes.Remove(_selectedItem);
                _allMemos.Remove(_selectedItem);
                ret.isCompleted = true;
                OnPropertyChanged();
                return ret;
            }
            catch (Exception ex)
            {
                ret.isCompleted = false;
                ret.message = ex.Message;
                return ret;
            }
        }
        /// <summary>
        /// Saves a collection of <see cref="MeetingMinutes"/> documents to the database.
        /// </summary>
        /// <remarks>This method connects to the database, iterates through the provided collection, and
        /// performs an upsert operation for each document. If an exception occurs during the operation, the method
        /// returns <see langword="false"/> for <c>isCompleted</c> and includes the exception message in
        /// <c>message</c>.</remarks>
        /// <param name="items">The collection of <see cref="MeetingMinutes"/> items to be saved. Each item is either updated or inserted
        /// into the database.</param>
        /// <returns>A tuple containing two values: <list type="bullet"> <item><term><c>isCompleted</c></term><description><see
        /// langword="true"/> if all documents were successfully saved; otherwise, <see
        /// langword="false"/>.</description></item> <item><term><c>message</c></term><description>A message describing
        /// the result of the operation. If an error occurs, this contains the exception message.</description></item>
        /// </list></returns>
        private (bool isCompleted, string message) SaveAllDocumentToDatabase(IEnumerable<MeetingMinutes> items)
        {
            (bool isCompleted, string message) ret = (false, string.Empty);
            try
            {
                conMgmnt.ConnectToDatabase();

                var collection = conMgmnt.GetCollection<MeetingMinutes>(conMgmnt.MeetingMemo);
                //_followupDocument.Headcount = HeadCountFollowupDocuments.ToList();
                foreach (var item in items)
                {
                    var filter = Builders<MeetingMinutes>.Filter.Eq(x => x.id, item.id);
                    var result = collection.ReplaceOne(filter, item, new ReplaceOptions { IsUpsert = true }); // ha nincs, beszúrja
                }

                //collection.ReplaceOne(filter, _selectedItem);
                ret.isCompleted = true;
                return ret;
            }
            catch (Exception ex)
            {
                ret.isCompleted = false;
                ret.message = ex.Message;
                return ret;
            }
        }

        private void GetMeetingMemoCollection()
        {
            var databaseCollection = conMgmnt.GetCollection<MeetingMinutes>(conMgmnt.MeetingMemo);
            var allDocuments = databaseCollection.Find(FilterDefinition<MeetingMinutes>.Empty).ToList();

            _meetingMinutes = new ObservableCollection<MeetingMinutes>(allDocuments);
            _allMemos = _meetingMinutes.ToList();
        }

        private void FilterRecordsByDate()
        {
            _taskDescription = _meetingMinutes.Where(c => c.date == _selectedDate).Select(c => c.task).First().ToString();
            OnPropertyChanged(nameof(TaskDescription));
        }

        private void PerformSearch(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (_allMemos != null)
                {
                    MeetingMinutes = new ObservableCollection<MeetingMinutes>(_allMemos);
                    OnPropertyChanged(nameof(MeetingMinutes));
                }
            }
            else
            {

                //MessageBox.Show(search);
                if (_allMemos != null)
                {
                    var filtered = _allMemos.Where(
                    s =>
                    (s.comment?.Contains(search) ?? false) ||
                    (s.pic?.Contains(search) ?? false) ||
                    (s.task?.Contains(search) ?? false) ||
                    (s.status?.Contains(search) ?? false) ||
                    (s.date.ToString("yyyy-MM-dd").Contains(search)) ||
                    (s.deadline.ToString("yyyy-MM-dd").Contains(search))
                    ).ToList();


                    MeetingMinutes = new ObservableCollection<MeetingMinutes>(filtered);
                    OnPropertyChanged(nameof(MeetingMinutes));
                }
            }
        }
        #endregion
    }
}

