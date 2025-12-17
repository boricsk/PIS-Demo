using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Models.NonRelationalModels;
using ProdInfoSys.Windows;
using System;
using System.Collections.Generic;
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
    public class DeleteWindowViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        #endregion

        ConnectionManagement conMgmnt = new ConnectionManagement();

        #region PropChangedInterface

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication
        private List<string> _documents;
        public List<string> Documents
        {
            get => _documents;
            set { _documents = value; OnPropertyChanged(); }
        }
        private string _selectedDocuments;
        public string SelectedDocuments
        {
            get => _selectedDocuments;
            set { _selectedDocuments = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand? DeleteDocument => new ProjectCommandRelay(_ => DeletingDocument());
        private void DeletingDocument()
        {
            if (!_selectedDocuments.IsNullOrEmpty())
            {
                //(MessageBox.Show($"Tényleg törölni szeretnéd a következő dokumentumot? {_selectedDocuments}", "Törlés", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                if (_dialogs.ShowConfirmation($"Tényleg törölni szeretnéd a következő dokumentumot? {_selectedDocuments}", "Törlés"))
                {
                    var filter = Builders<MasterFollowupDocument>.Filter.Eq(x => x.DocumentName, _selectedDocuments);
                    var databaseCollection = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName);
                    databaseCollection.DeleteOne(filter);                    
                }
            }
        }
        #endregion

        #region Constructor
        public DeleteWindowViewModel(IUserDialogService dialogs)
        {               
            _dialogs = dialogs;
            var documents = conMgmnt.GetCollection<MasterFollowupDocument>(conMgmnt.DbName).Find(FilterDefinition<MasterFollowupDocument>.Empty).ToList(); ;
            _documents = documents.Select(x => x.DocumentName).ToList();
        }
        #endregion

    }
}
