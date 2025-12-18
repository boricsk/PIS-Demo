using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Models;
using ProdInfoSys.Models.NonRelationalModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProdInfoSys.ViewModels
{
    public class SetupWindowViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private IUserDialogService _dialogs;
        private IConnectionManagement _connectionManagement;
        #endregion

        private PisSetup? _setupData = new PisSetup();

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
        /// <remarks>Call this method in the setter of a property to notify subscribers that the
        /// property's value has changed. This is commonly used to implement the INotifyPropertyChanged interface in
        /// data-binding scenarios.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and is automatically provided when called from
        /// a property setter.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication
        private bool _isSubconView;
        public bool IsSubconView
        {
            get => _isSubconView;
            set { _isSubconView = value; OnPropertyChanged(); }
        }

        private int _fontSize = 12;
        public int FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        private List<string> _documents;
        public List<string> Documents
        {
            get => _documents;
            set { _documents = value; OnPropertyChanged(); }
        }
        private string _selectedDocument;
        public string SelectedDocument
        {
            get => _selectedDocument;
            set { _selectedDocument = value; OnPropertyChanged(); }
        }
        private TransfWorkday _transferredWorkday;
        public TransfWorkday TransferredWorkday
        {
            get => _transferredWorkday;
            set { _transferredWorkday = value; OnPropertyChanged(); }
        }
        private ObservableCollection<TransferredWorkday> _transferredWorkdays;
        public ObservableCollection<TransferredWorkday> TransferredWorkdays
        {
            get => _transferredWorkdays;
            set { _transferredWorkdays = value; OnPropertyChanged(); }
        }
        private TransferredWorkday _selectedTransferredWorkdays;
        public TransferredWorkday SelectedTransferredWorkdays
        {
            get => _selectedTransferredWorkdays;
            set { _selectedTransferredWorkdays = value; OnPropertyChanged(); }
        }

        private string _erpConnection;
        public string ErpConnection
        {
            get => _erpConnection;
            set { _erpConnection = value; OnPropertyChanged(); }
        }

        private string _mongoConnection;
        public string MongoConnection
        {
            get => _mongoConnection;
            set { _mongoConnection = value; OnPropertyChanged(); }
        }

        private string _testEmailAddr;
        public string TestEmailAddr
        {
            get => _testEmailAddr;
            set { _testEmailAddr = value; OnPropertyChanged(); }
        }

        private string _smtpServer;
        public string SmtpServer
        {
            get => _smtpServer;
            set { _smtpServer = value; OnPropertyChanged(); }
        }

        private string _smtpServerUser;
        public string SmtpServerUser
        {
            get => _smtpServerUser;
            set { _smtpServerUser = value; OnPropertyChanged(); }
        }

        private string _smtpPort;
        public string SmtpPort
        {
            get => _smtpPort;
            set { _smtpPort = value; OnPropertyChanged(); }
        }
        private string _smtpPassw;
        public string SmtpPassw
        {
            get => _smtpPassw;
            set { _smtpPassw = value; OnPropertyChanged(); }
        }

        private string _selectedEmail;
        public string SelectedEmail
        {
            get => _selectedEmail;
            set { _selectedEmail = value; OnPropertyChanged(); }
        }

        private string _selectedLeaderEmail;
        public string SelectedLeaderEmail
        {
            get => _selectedLeaderEmail;
            set { _selectedLeaderEmail = value; OnPropertyChanged(); }
        }

        private string _newEmail;
        public string NewEmail
        {
            get => _newEmail;
            set { _newEmail = value; OnPropertyChanged(); }
        }

        private string _newLeaderEmail;
        public string NewLeaderEmail
        {
            get => _newLeaderEmail;
            set { _newLeaderEmail = value; OnPropertyChanged(); }
        }

        private string _selectedWorkcenter;
        public string SelectedWorkcenter
        {
            get => _selectedWorkcenter;
            set { _selectedWorkcenter = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _projWorkcenters;
        public ObservableCollection<string> ProjWorkcenters
        {
            get => _projWorkcenters;
            set { _projWorkcenters = value; OnPropertyChanged(); }
        }

        private string _cbSelectedNewProjWorkcenter;
        public string CbSelectedNewProjWorkcenter
        {
            get => _cbSelectedNewProjWorkcenter;
            set { _cbSelectedNewProjWorkcenter = value; OnPropertyChanged(); }
        }

        private string _projSelectedWorkcenter;
        public string ProjSelectedWorkcenter
        {
            get => _projSelectedWorkcenter;
            set { _projSelectedWorkcenter = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string>? _emailList;
        public ObservableCollection<string>? EmailList
        {
            get => _emailList;
            set { _emailList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _leaderEmailList;
        public ObservableCollection<string> LeaderEmailList
        {
            get => _leaderEmailList;
            set { _leaderEmailList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _availWorkcenterList;
        public ObservableCollection<string> AvailWorkcenterList
        {
            get => _availWorkcenterList;
            set { _availWorkcenterList = value; OnPropertyChanged(); }
        }

        private string _selectedNewWorkcenter;
        public string SelectedNewWorkcenter
        {
            get => _selectedNewWorkcenter;
            set { _selectedNewWorkcenter = value; OnPropertyChanged(); }
        }

        private string _updateLocation;
        public string UpdateLocation
        {
            get => _updateLocation;
            set { _updateLocation = value; OnPropertyChanged(); }
        }

        private string _installLocation;
        public string InstallLocation
        {
            get => _installLocation;
            set { _installLocation = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _prodMeetingWorkcenters;
        public ObservableCollection<string> ProdMeetingWorkcenters
        {
            get => _prodMeetingWorkcenters;
            set { _prodMeetingWorkcenters = value; OnPropertyChanged(); }
        }

        private string _avgSelectedNewWorkcenter;
        public string AvgSelectedNewWorkcenter
        {
            get => _avgSelectedNewWorkcenter;
            set { _avgSelectedNewWorkcenter = value; OnPropertyChanged(); }
        }

        private string _cbSelectedNewAvgWorkcenter;
        public string CbSelectedNewAvgWorkcenter
        {
            get => _cbSelectedNewAvgWorkcenter;
            set { _cbSelectedNewAvgWorkcenter = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _avgWorkcenters;
        public ObservableCollection<string> AvgWorkcenters
        {
            get => _avgWorkcenters;
            set { _avgWorkcenters = value; OnPropertyChanged(); }
        }

        private string _erpUser;
        public string ErpUser
        {
            get => _erpUser;
            set { _erpUser = value; OnPropertyChanged(); }
        }

        private string _erpUserPw;
        public string ErpUserPw
        {
            get => _erpUserPw;
            set { _erpUserPw = value; OnPropertyChanged(); }
        }

        private string _apiServer;
        public string ApiServer
        {
            get => _apiServer;
            set { _apiServer = value; OnPropertyChanged(); }
        }

        private string _erpCompany;
        public string ErpCompany
        {
            get => _erpCompany;
            set { _erpCompany = value; OnPropertyChanged(); }
        }

        private string _erpEnv;
        public string ErpEnv
        {
            get => _erpEnv;
            set { _erpEnv = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommand
        public ICommand AddDate => new ProjectCommandRelay(_ => AddingDate());
        /// <summary>
        /// Adds a new transferred workday to the collection using the current values of the source workday.
        /// </summary>
        /// <remarks>This method creates a new instance of the transferred workday with dates converted to
        /// the DateOnly type and appends it to the TransferredWorkdays collection. Intended for internal use within the
        /// class to manage transferred workday records.</remarks>
        private void AddingDate()
        {
            var d = new TransferredWorkday()
            {
                FromDay = DateOnly.FromDateTime(_transferredWorkday.FromDay),
                ToDay = DateOnly.FromDateTime(_transferredWorkday.ToDay)
            };
            TransferredWorkdays.Add(d);

        }

        public ICommand SaveDate => new ProjectCommandRelay(_ => SavingDate());
        /// <summary>
        /// Saves the current set of transferred workdays to the database, replacing any existing records.
        /// </summary>
        /// <remarks>This method deletes all existing workday records before saving the current set. Use
        /// with caution, as this operation will remove all previous data in the workday collection.</remarks>
        private void SavingDate()
        {
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<TrWorkday>(conMgmnt.TrWorkdaysDbName);
            MongoDbOperations<TrWorkday> db = new MongoDbOperations<TrWorkday>(databaseCollection);

            var trWorkdays = new TrWorkday()
            {
                TransferredWorkdays = TransferredWorkdays.ToList(),
            };

            _ = db.DeleteAll();
            _ = db.AddNewDocument(trWorkdays);
        }

        public ICommand SaveConnections => new ProjectCommandRelay(_ => SavingConnections());
        /// <summary>
        /// Validates and saves the current ERP and MongoDB connection settings to the registry, displaying appropriate
        /// user feedback based on the outcome.
        /// </summary>
        /// <remarks>This method checks that all required connection fields are populated and verifies the
        /// MongoDB connection before saving the settings. If validation fails or the connection cannot be established,
        /// an error message is displayed to the user. This method is intended for internal use within the setup
        /// workflow and does not return a value.</remarks>
        private void SavingConnections()
        {

            if (_erpConnection.IsNullOrEmpty() || _mongoConnection.IsNullOrEmpty())
            {
                _dialogs.ShowErrorInfo("Nincs kitöltve mindn szükséges mező!", "SetupViewModel");
            }
            else
            {
                if (_connectionManagement.PingConnection(_mongoConnection))
                {
                    RegistryManagement.WriteStringRegistryKey("MongoConStringLocal", _mongoConnection);
                    RegistryManagement.WriteStringRegistryKey("ERPConString", DpApiStorage.Encrypt(_erpConnection));
                    RegistryManagement.WriteStringRegistryKey("UpdateLocation", _updateLocation);
                    RegistryManagement.WriteStringRegistryKey("InstallLocation", _installLocation);

                    _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");

                }
                else
                {
                    _dialogs.ShowErrorInfo("A megadott kapcsolat nem létezik!", "SetupViewModel");
                }
            }
        }

        public ICommand SaveOAuth => new ProjectCommandRelay(_ => SaveingOAuth());
        /// <summary>
        /// Saves the current OAuth-related configuration values to the registry after validating that all required
        /// fields are populated.
        /// </summary>
        /// <remarks>If any required configuration field is missing or empty, an error message is
        /// displayed and no values are saved. On successful save, an informational message is shown to the
        /// user.</remarks>
        private void SaveingOAuth()
        {
            if (_erpUser.IsNullOrEmpty() ||
                _erpUserPw.IsNullOrEmpty() ||
                _apiServer.IsNullOrEmpty() ||
                _erpCompany.IsNullOrEmpty() ||
                _erpEnv.IsNullOrEmpty()
                )
            {
                _dialogs.ShowErrorInfo("Nincs kitöltve mindn szükséges mező!", "SetupViewModel");

            }
            else
            {
                RegistryManagement.WriteStringRegistryKey("ErpUser", _erpUser);
                RegistryManagement.WriteStringRegistryKey("ErpUserPw", DpApiStorage.Encrypt(_erpUserPw));
                RegistryManagement.WriteStringRegistryKey("ApiServer", _apiServer);
                RegistryManagement.WriteStringRegistryKey("ErpCompany", _erpCompany);
                RegistryManagement.WriteStringRegistryKey("ErpEnv", _erpEnv);

                _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
            }
        }
        public ICommand SaveView => new ProjectCommandRelay(_ => SavingView());
        /// <summary>
        /// Saves the current view settings to the registry and displays a confirmation message to the user.
        /// </summary>
        private void SavingView()
        {
            RegistryManagement.WriteBoolRegistryKey("SubconView", _isSubconView);
            RegistryManagement.WriteIntRegistryKey("FontSize", _fontSize);
            _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
        }
        public ICommand SaveCurrentDocument => new ProjectCommandRelay(_ => SavingCurrentDocument());
        /// <summary>
        /// Saves the currently selected document and displays a notification indicating the result of the operation.
        /// </summary>
        /// <remarks>If an error occurs during the save process, an error message is displayed to the
        /// user. This method updates the setup data to reflect the current document selection before saving.</remarks>
        private void SavingCurrentDocument()
        {
            try
            {
                _setupData.ActualFollowup = SelectedDocument;
                SetupManagement.SaveSetupData(_setupData);
                _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Dokumentum mentés");
            }
        }

        public ICommand SaveAvgDocument => new ProjectCommandRelay(_ => SavingAvgDocument());
        /// <summary>
        /// Saves the current average manual workcenter data and displays a notification indicating the result of the
        /// operation.
        /// </summary>
        /// <remarks>If an error occurs during the save process, an error message is displayed to the
        /// user. This method is intended to be used within the view model to persist setup data and inform the user of
        /// success or failure.</remarks>
        private void SavingAvgDocument()
        {
            try
            {
                _setupData.AvgManualWorkcenters = new ObservableCollection<string>(_avgWorkcenters);
                SetupManagement.SaveSetupData(_setupData);
                _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Gépcsoport átlag");
            }
        }

        public ICommand SaveEmailList => new ProjectCommandRelay(_ => SavingEmailList());
        /// <summary>
        /// Saves the current list of email addresses to the setup data and notifies the user of the result.
        /// </summary>
        /// <remarks>Displays an informational dialog if the save operation succeeds, or an error dialog
        /// if an exception occurs during the save process.</remarks>
        private void SavingEmailList()
        {
            try
            {
                _setupData.EmailList = new ObservableCollection<string>(_emailList);
                SetupManagement.SaveSetupData(_setupData);
                _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Email lista mentés");
            }
        }

        public ICommand SaveLeaderEmailList => new ProjectCommandRelay(_ => SavingLeaderEmailList());
        /// <summary>
        /// Saves the current list of leader email addresses to the setup data and displays a notification indicating
        /// the result.
        /// </summary>
        /// <remarks>If the save operation fails, an error message is displayed to the user. This method
        /// updates the setup data with the latest leader email list before saving.</remarks>
        private void SavingLeaderEmailList()
        {
            try
            {
                _setupData.LeaderEmailList = new ObservableCollection<string>(_leaderEmailList);
                SetupManagement.SaveSetupData(_setupData);
                _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "Vezetői email lista mentés");
            }
        }

        public ICommand SaveProdMeetingWorkcenters => new ProjectCommandRelay(_ => SavingProdMeetingWorkcenters());
        /// <summary>
        /// Saves the current list of production meeting workcenters to the setup data and displays a confirmation
        /// message to the user.
        /// </summary>
        private void SavingProdMeetingWorkcenters()
        {
            _setupData.ProdMeetingWorkcenters = new ObservableCollection<string>(_prodMeetingWorkcenters);
            SetupManagement.SaveSetupData(_setupData);
            _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
        }

        public ICommand SaveEmailConfig => new ProjectCommandRelay(_ => SavingEmailConfig());
        /// <summary>
        /// Saves the current email configuration settings to persistent storage after validating required fields.
        /// </summary>
        /// <remarks>Displays an error message if any required email configuration fields are missing or
        /// if the SMTP port is invalid. On successful validation, updates the email configuration and stores the
        /// settings securely. This method is intended for internal use within the setup workflow.</remarks>
        private void SavingEmailConfig()
        {
            if (string.IsNullOrEmpty(_smtpPassw) || _smtpPort.IsNullOrEmpty() || _smtpServer.IsNullOrEmpty())
            {
                _dialogs.ShowErrorInfo("Nincs kitöltve mindn szükséges mező!", "SetupViewModel");
            }
            else
            {
                if (!int.TryParse(_smtpPort, out int port))
                {
                    _dialogs.ShowErrorInfo("A portszám hibásan van megadva!", "SetupViewModel");
                    return;
                }

                _setupData.EmailList = new ObservableCollection<string>(_emailList);
                RegistryManagement.WriteStringRegistryKey("EmailPw", DpApiStorage.Encrypt(_smtpPassw.ToString()));
                RegistryManagement.WriteStringRegistryKey("SMTPServer", _smtpServer);
                RegistryManagement.WriteStringRegistryKey("SMTPPort", _smtpPort);
                RegistryManagement.WriteStringRegistryKey("SMTPUser", _smtpServerUser);
                _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
            }
        }

        public ICommand DeleteSelectedDate => new ProjectCommandRelay(_ => DeletingSelectedDate());
        /// <summary>
        /// Removes the currently selected transferred workday from the collection, if one is selected.
        /// </summary>
        /// <remarks>If no transferred workday is selected, the method performs no action. This method is
        /// typically used to support user-driven deletion scenarios in UI workflows.</remarks>
        private void DeletingSelectedDate()
        {
            if (SelectedTransferredWorkdays != null)
            {
                TransferredWorkdays.Remove(SelectedTransferredWorkdays);
            }
        }

        public ICommand AddNewEmail => new ProjectCommandRelay(_ => AddingNewEmail());
        /// <summary>
        /// Adds the new email address to the email list if it is not already present.
        /// </summary>
        private void AddingNewEmail()
        {
            if (_newEmail != null)
            {
                if (!_emailList.Contains(_newEmail))
                {
                    _emailList.Add(_newEmail);
                    _newEmail = string.Empty;
                    OnPropertyChanged(nameof(NewEmail));
                }
            }
        }

        public ICommand AddNewLeaderEmail => new ProjectCommandRelay(_ => AddingNewLeaderEmail());
        /// <summary>
        /// Adds the current new leader email to the leader email list if it is not already present and resets the new
        /// leader email value.
        /// </summary>
        /// <remarks>This method is typically called after a user enters a new leader email address to
        /// ensure it is added to the collection only once. After adding, the new leader email input is cleared and
        /// property change notification is raised for data binding scenarios.</remarks>
        private void AddingNewLeaderEmail()
        {
            if (_newLeaderEmail != null)
            {
                if (!_leaderEmailList.Contains(_newLeaderEmail))
                {
                    _leaderEmailList.Add(_newLeaderEmail);
                    _newLeaderEmail = string.Empty;
                    OnPropertyChanged(nameof(NewLeaderEmail));
                }
            }
        }

        public ICommand AddNewWorkcenter => new ProjectCommandRelay(_ => AddingNewWorkcenter());
        /// <summary>
        /// Adds the selected workcenter to the production meeting workcenters collection if it is not already present.
        /// </summary>
        /// <remarks>This method updates the collection of production meeting workcenters and raises a
        /// property changed notification for data binding scenarios. No action is taken if the selected workcenter is
        /// null or already exists in the collection.</remarks>
        private void AddingNewWorkcenter()
        {
            if (_selectedNewWorkcenter != null)
            {
                if (!_prodMeetingWorkcenters.Contains(_selectedNewWorkcenter))
                {
                    _prodMeetingWorkcenters.Add(_selectedNewWorkcenter);

                    OnPropertyChanged(nameof(ProdMeetingWorkcenters));
                }
            }
        }

        public ICommand ProjAddNewWorkcenter => new ProjectCommandRelay(_ => AddingProjectorNewWorkcenter());
        /// <summary>
        /// Adds the currently selected new projector workcenter to the collection if it is not already present.
        /// </summary>
        /// <remarks>This method updates the collection of projector workcenters and raises a property
        /// change notification for data binding scenarios. No action is taken if the selected workcenter is null or
        /// already exists in the collection.</remarks>
        private void AddingProjectorNewWorkcenter()
        {
            if (_cbSelectedNewProjWorkcenter != null)
            {
                if (!_projWorkcenters.Contains(_cbSelectedNewProjWorkcenter))
                {
                    _projWorkcenters.Add(_cbSelectedNewProjWorkcenter);
                    OnPropertyChanged(nameof(ProjWorkcenters));
                }
            }
        }

        public ICommand ProjRemoveWorkcenter => new ProjectCommandRelay(_ => ProjRemovingWorkcenter());
        /// <summary>
        /// Removes the currently selected workcenter from the project workcenters collection, if one is selected.
        /// </summary>
        private void ProjRemovingWorkcenter()
        {
            if (_projSelectedWorkcenter != null)
            {
                _projWorkcenters.Remove(_projSelectedWorkcenter);
            }
        }

        public ICommand SaveProjDocument => new ProjectCommandRelay(_ => SavingProjDocument());
        /// <summary>
        /// Saves the current projector workcenter configuration and displays a confirmation message to the user.
        /// </summary>
        /// <remarks>This method updates the setup data with the current list of projector workcenters and
        /// persists the changes. After saving, it notifies the user that the operation was successful. This method is
        /// intended for internal use within the view model and does not return a value.</remarks>
        private void SavingProjDocument()
        {
            _setupData.ProjectorWorkcenters = new ObservableCollection<string>(_projWorkcenters);
            SetupManagement.SaveSetupData(_setupData);
            _dialogs.ShowInfo("A mentés sikeres!", "SetupViewModel");
        }

        public ICommand AvgAddNewWorkcenter => new ProjectCommandRelay(_ => AvgAddingNewWorkcenter());
        /// <summary>
        /// Adds the currently selected new average workcenter to the collection if it is not already present.
        /// </summary>
        /// <remarks>Raises a property change notification for the average workcenters collection when a
        /// new workcenter is added. This method has no effect if the selected workcenter is null or already exists in
        /// the collection.</remarks>
        private void AvgAddingNewWorkcenter()
        {
            if (_cbSelectedNewAvgWorkcenter != null)
            {
                if (!_avgWorkcenters.Contains(_cbSelectedNewAvgWorkcenter))
                {
                    _avgWorkcenters.Add(_cbSelectedNewAvgWorkcenter);

                    OnPropertyChanged(nameof(AvgWorkcenters));
                }
            }
        }

        public ICommand RemoveEmail => new ProjectCommandRelay(_ => RemovingEmail());
        /// <summary>
        /// Removes the currently selected email from the email list, if one is selected.
        /// </summary>
        private void RemovingEmail()
        {
            if (_selectedEmail != null)
            {
                _emailList.Remove(_selectedEmail);
            }
        }

        public ICommand RemoveLeaderEmail => new ProjectCommandRelay(_ => RemovingLeaderEmail());
        /// <summary>
        /// Removes the currently selected leader email from the leader email list, if one is selected.
        /// </summary>
        private void RemovingLeaderEmail()
        {
            if (_selectedLeaderEmail != null)
            {
                _leaderEmailList.Remove(_selectedLeaderEmail);
            }
        }

        public ICommand RemoveWorkcenter => new ProjectCommandRelay(_ => RemovingWorkcenter());
        /// <summary>
        /// Removes the currently selected workcenter from the collection of production meeting workcenters, if one is
        /// selected.
        /// </summary>
        private void RemovingWorkcenter()
        {
            if (_selectedWorkcenter != null)
            {
                _prodMeetingWorkcenters.Remove(_selectedWorkcenter);
            }
        }

        public ICommand AvgRemoveWorkcenter => new ProjectCommandRelay(_ => AvgRemovingWorkcenter());
        /// <summary>
        /// Removes the currently selected workcenter from the collection of workcenters, if one is selected.
        /// </summary>
        private void AvgRemovingWorkcenter()
        {
            if (_avgSelectedNewWorkcenter != null)
            {
                _avgWorkcenters.Remove(_avgSelectedNewWorkcenter);
            }
        }

        public ICommand SendTestMail => new ProjectCommandRelay(_ => SendingTestMail());
        /// <summary>
        /// Sends a test email to the configured test email address to verify SMTP settings and email delivery
        /// functionality.
        /// </summary>
        /// <remarks>This method displays a success or error message to the user based on the outcome of
        /// the email sending operation. It is intended for use in scenarios where validating email configuration is
        /// required, such as during application setup or diagnostics.</remarks>
        private void SendingTestMail()
        {
            try
            {
                BuildEmail email = new BuildEmail(testEmailAddr: _testEmailAddr);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(RegistryManagement.ReadStringRegistryKey("SMTPUser"));
                mail.To.Add(_testEmailAddr);
                mail.Subject = "PIS Test Email";
                mail.IsBodyHtml = true;
                mail.Body = email.BuildTestEmail();

                SmtpClient smtp = new SmtpClient(
                    RegistryManagement.ReadStringRegistryKey("SMTPServer"),
                    int.Parse(RegistryManagement.ReadStringRegistryKey("SMTPPort")));
                smtp.Credentials = new NetworkCredential(RegistryManagement.ReadStringRegistryKey("SMTPUser"), SetupManagement.GetSmtpPassword());
                smtp.EnableSsl = true;
                smtp.Send(mail);
                _dialogs.ShowInfo("Teszt email küldés sikeres!", "SetupViewModel");
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"Küldés sikertelen a következő hiba miatt : {ex.Message}", "SetupWindowViewModel");
            }
        }
        #endregion

        #region Constructor
        public SetupWindowViewModel(IUserDialogService dialogs, IConnectionManagement connectionManagement)
        {
            _dialogs = dialogs;
            _connectionManagement = connectionManagement;
            SetDefaults();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Initializes or resets the object's properties and related data collections to their default values.
        /// </summary>
        /// <remarks>This method loads configuration values from the registry and other sources, sets up
        /// internal collections, and ensures that dependent properties are updated to reflect the default state. It
        /// should be called to ensure the object is in a consistent, initialized state before use.</remarks>
        private void SetDefaults()
        {
            TransferredWorkdays = new ObservableCollection<TransferredWorkday>();
            TransferredWorkday = new TransfWorkday();
            _emailList = new();
            _leaderEmailList = new ObservableCollection<string>();
            _prodMeetingWorkcenters = new ObservableCollection<string>();
            _avgWorkcenters = new ObservableCollection<string>();
            _projWorkcenters = new ObservableCollection<string>();
            TransferredWorkday.ToDay = DateTime.Today;
            TransferredWorkday.FromDay = DateTime.Today;
            _mongoConnection = RegistryManagement.ReadStringRegistryKey("MongoConStringLocal");
            _smtpServer = RegistryManagement.ReadStringRegistryKey("SMTPServer");
            _smtpPort = RegistryManagement.ReadStringRegistryKey("SMTPPort");
            _isSubconView = RegistryManagement.ReadBoolRegistryKey("SubconView");
            _fontSize = RegistryManagement.ReadIntRegistryKey("FontSize");
            _erpUser = RegistryManagement.ReadStringRegistryKey("ErpUser");

            if (!RegistryManagement.ReadStringRegistryKey("SMTPUser").IsNullOrEmpty())
            {
                _smtpServerUser = RegistryManagement.ReadStringRegistryKey("SMTPUser");
                OnPropertyChanged(nameof(SmtpServerUser));
            }
            else
            {
                RegistryManagement.WriteStringRegistryKey("SMTPUser", "noreply@sumi-electric.hu");
                _smtpServerUser = RegistryManagement.ReadStringRegistryKey("SMTPUser");
                OnPropertyChanged(nameof(SmtpServerUser));
            }

            if (!RegistryManagement.ReadStringRegistryKey("ErpUserPw").IsNullOrEmpty())
            {
                _erpUserPw = SetupManagement.GetErpUserPassword();
            }
            _apiServer = RegistryManagement.ReadStringRegistryKey("ApiServer");
            _erpCompany = RegistryManagement.ReadStringRegistryKey("ErpCompany");
            _erpEnv = RegistryManagement.ReadStringRegistryKey("ErpEnv");
            _updateLocation = RegistryManagement.ReadStringRegistryKey("UpdateLocation");
            _installLocation = RegistryManagement.ReadStringRegistryKey("InstallLocation");

            if (!RegistryManagement.ReadStringRegistryKey("EmailPw").IsNullOrEmpty())
            {
                _smtpPassw = SetupManagement.GetSmtpPassword();
                OnPropertyChanged(nameof(SmtpPassw));
            }

            if (!RegistryManagement.ReadStringRegistryKey("ERPConString").IsNullOrEmpty())
            {
                _erpConnection = SetupManagement.GetErpConString();
                OnPropertyChanged(nameof(ErpConnection));
            }

            _setupData = SetupManagement.LoadSetupData();

            if (_setupData.EmailList != null)
            {
                _emailList = new ObservableCollection<string>(_setupData.EmailList);
            }
            if (_setupData.LeaderEmailList != null)
            {
                _leaderEmailList = new ObservableCollection<string>(_setupData.LeaderEmailList);
            }
            if (_setupData.ProdMeetingWorkcenters != null)
            {
                _prodMeetingWorkcenters = new ObservableCollection<string>(_setupData.ProdMeetingWorkcenters);
            }
            if (_setupData.AvgManualWorkcenters != null)
            {
                _avgWorkcenters = new ObservableCollection<string>(_setupData.AvgManualWorkcenters);
            }
            if (_setupData.ProjectorWorkcenters != null)
            {
                _projWorkcenters = new ObservableCollection<string>(_setupData.ProjectorWorkcenters);
            }

            var documents = _connectionManagement.GetCollection<MasterFollowupDocument>(_connectionManagement.DbName).Find(FilterDefinition<MasterFollowupDocument>.Empty).ToList();
            _documents = documents.OrderByDescending(x => x.DocumentName).Select(x => x.DocumentName).ToList();
            _selectedDocument = _setupData.ActualFollowup;

            LoadTrWorkdays();
            OnPropertyChanged();
            LoadDataWithDapper();
        }

        /// <summary>
        /// Loads the list of available work centers from the ERP system using Dapper and updates the
        /// AvailWorkcenterList property.
        /// </summary>
        /// <remarks>If an error occurs while retrieving data, an error dialog is displayed to inform the
        /// user. This method is intended for internal use and does not return a value.</remarks>
        private void LoadDataWithDapper()
        {
            try
            {
                DapperFunctions df = new DapperFunctions();
                AvailWorkcenterList = new ObservableCollection<string>(df.GetErpMachineCenters().Select(x => x.Workcenter).ToList());
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"Hiba történt : {ex.Message}", "Gépcsoportlista betöltés");
            }
        }

        /// <summary>
        /// Loads all transferred workday records from the database and adds them to the current collection.
        /// </summary>
        /// <remarks>This method retrieves all documents from the transferred workdays collection in the
        /// database. For each document, it adds any associated transferred workday entries to the in-memory collection.
        /// This method is intended for internal use and does not return a value.</remarks>
        private void LoadTrWorkdays()
        {
            var workday = new TransferredWorkday();
            ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = conMgmnt.GetCollection<TrWorkday>(conMgmnt.TrWorkdaysDbName);

            var allDocuments = databaseCollection.Find(FilterDefinition<TrWorkday>.Empty).ToList();
            foreach (var document in allDocuments)
            {
                if (document.TransferredWorkdays != null)
                {
                    foreach (var tr in document.TransferredWorkdays)
                    {
                        workday = new TransferredWorkday()
                        {
                            FromDay = tr.FromDay,
                            ToDay = tr.ToDay,
                        };
                        TransferredWorkdays.Add(tr);
                    }
                }
            }
        }
        #endregion
    }
}
