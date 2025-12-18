using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Models.NonRelationalModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProdInfoSys.ViewModels
{
    /// <summary>
    /// Represents the view model for the delete document window, providing properties and commands to manage document
    /// deletion operations in the UI.
    /// </summary>
    /// <remarks>This view model is intended for use in WPF applications following the MVVM pattern. It
    /// exposes the list of available documents, tracks the selected document, and provides a command to delete the
    /// selected document. The class implements INotifyPropertyChanged to support data binding. The deletion operation
    /// prompts the user for confirmation before removing the document from the underlying data store.</remarks>
    public class DeleteWindowViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        #endregion

        ConnectionManagement conMgmnt = new ConnectionManagement();

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by implementations of the INotifyPropertyChanged
        /// interface to notify subscribers that a property value has changed. Handlers attached to this event receive
        /// information about which property changed via the PropertyChangedEventArgs parameter. This event is commonly
        /// used in data binding scenarios to update UI elements when underlying data changes.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in a property's setter to notify subscribers that the property's
        /// value has changed. This is commonly used to implement the INotifyPropertyChanged interface in data-binding
        /// scenarios.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and will be automatically set to the caller
        /// member name if not specified.</param>
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
        /// <summary>
        /// Prompts the user for confirmation and deletes the selected document from the database if confirmed.
        /// </summary>
        /// <remarks>This method displays a confirmation dialog before deleting the document. The deletion
        /// is only performed if the user confirms the action. No action is taken if no document is selected.</remarks>
        private void DeletingDocument()
        {
            if (!_selectedDocuments.IsNullOrEmpty())
            {
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
