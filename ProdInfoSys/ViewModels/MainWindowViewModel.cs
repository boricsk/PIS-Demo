using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProdInfoSys.Classes;
using ProdInfoSys.CommandRelay;
using ProdInfoSys.DI;
using ProdInfoSys.Enums;
using ProdInfoSys.Interfaces;
using ProdInfoSys.Models;
using ProdInfoSys.Models.ErpDataModels;
using ProdInfoSys.Models.NonRelationalModels;
using ProdInfoSys.ViewModels.Nested;
using ProdInfoSys.Windows;
using ProdInfoSys.Windows.Nested;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ProdInfoSys.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Dependency injection
        private readonly IUserDialogService _dialogs;
        private readonly IWindowFactory _windowFactory;
        private readonly IUserControlFunctions _userControlFunctions;
        private readonly IConnectionManagement _connectionManagement;
        #endregion

        private ObservableCollection<ErpMachineCenter> _erpMachineCenters;
        private List<string> _inspectionMachines = new List<string>();
        private List<string> _manualMachines = new List<string>();
        private List<string> _machines = new List<string>();
        private TreeNodeModel _selectedTreeNode = new TreeNodeModel();

        public ObservableCollection<TreeNodeModel> TreeNodes { get; set; } = new ObservableCollection<TreeNodeModel>();

        #region PropChangedInterface
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>This event is typically raised by the implementation of the INotifyPropertyChanged
        /// interface to notify subscribers that a property value has changed. Handlers attached to this event receive
        /// information about which property was changed.</remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event to notify listeners that a property value has changed.
        /// </summary>
        /// <remarks>Call this method in a property's setter to notify subscribers that the property's
        /// value has changed. This is commonly used to implement the INotifyPropertyChanged interface in data-binding
        /// scenarios.</remarks>
        /// <param name="propertyName">The name of the property that changed. This value is optional and is automatically provided when called from
        /// a property setter.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region WPF Communication

        private string? _time;
        public string? Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(); }
        }

        private object _selectedContent;
        public object SelectedContent
        {
            get => _selectedContent;
            set { _selectedContent = value; OnPropertyChanged(); }
        }

        private string _rootName;
        public string RootName
        {
            get => _rootName;
            set { _rootName = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command Relay
        public ICommand SelectMenuCommand => new ProjectCommandRelay(SelectMenu);
        /// <summary>
        /// Updates the selected menu item and displays the corresponding content based on the specified tree node
        /// parameter.
        /// </summary>
        /// <remarks>If the parameter is not a TreeNodeModel, the method performs no action. The displayed
        /// content is determined by the name and hierarchy of the selected tree node. This method also updates the
        /// RootName property and notifies listeners of the change.</remarks>
        /// <param name="param">An object representing the selected tree node. Must be of type TreeNodeModel to update the menu selection
        /// and content.</param>
        private void SelectMenu(object param)
        {
            if (param is TreeNodeModel selected)
            {
                _selectedTreeNode = selected;
                _rootName = selected.GetRoot().Name;
                OnPropertyChanged(nameof(RootName));

                if (selected.Name == "Headcount")
                {
                    var hc = new HeadcountViewModel(_dialogs, _userControlFunctions, _connectionManagement);
                    hc.Init(selected.GetRoot());
                    var view = new HeadcountUserControl { DataContext = hc };
                    SelectedContent = view;
                }
                if (_inspectionMachines.Contains(selected.Name))
                {
                    var hc = new InspectionViewModel(_dialogs, _userControlFunctions, _connectionManagement);
                    hc.Init(selected.GetRoot(), selected.Name);
                    var view = new InspectionUserControl { DataContext = hc };
                    SelectedContent = view;
                }
                if (_manualMachines.Contains(selected.Name))
                {
                    var hc = new ManualViewModel(_dialogs, _userControlFunctions, _connectionManagement);
                    hc.Init(selected.GetRoot(), selected.Name);
                    var view = new ManualUserControl { DataContext = hc };
                    SelectedContent = view;
                }
                if (_machines.Contains(selected.Name))
                {
                    var hc = new MachineViewModel(_dialogs, _userControlFunctions, _connectionManagement);
                    hc.Init(selected.GetRoot(), selected.Name);
                    var view = new MachineUserControl { DataContext = hc };
                    SelectedContent = view;
                }

                if (selected.Parent != null)
                {
                    if (selected.Parent.Name == "Status reports")
                    {
                        var hc = new StatusReportViewModel(_dialogs, _userControlFunctions, _connectionManagement);
                        hc.Init(selected.GetRoot(), selected.Name);
                        var view = new StatusReportUserControl { DataContext = hc };
                        SelectedContent = view;
                    }

                    if (selected.Parent.Name == "Status reports QRQC")
                    {
                        var hc = new StatusReportQrqcViewModel();
                        hc.Init(selected.GetRoot(), selected.Name);
                        var view = new StatusReportQrqcUserControl { DataContext = hc };
                        SelectedContent = view;
                    }
                }
            }
        }
        public ICommand? ShowStatusReport => new ProjectCommandRelay(_ => ShowingStatusReport());
        /// <summary>
        /// Displays the status report dialog for the currently selected document, if available.
        /// </summary>
        /// <remarks>If no document is selected, an error message is shown to the user. This method is
        /// intended to be called in response to user actions that require viewing a status report.</remarks>
        private void ShowingStatusReport()
        {
            if (!_rootName.IsNullOrEmpty())
            {

                NewStatusReportViewModel statusReportViewModel = new NewStatusReportViewModel(_dialogs);
                statusReportViewModel.Init(_rootName);

                var window = _windowFactory.Create<StatusReportWindow>();
                window.DataContext = statusReportViewModel;
                window.ShowDialog();
                LoadTree();
            }
            else
            {
                _dialogs.ShowErrorInfo($"Nincs kiválasztva dokumentum!", "MainWindowViewModel");
            }
        }

        public ICommand? ShowQRQCStatusReport => new ProjectCommandRelay(_ => ShowingQRQCStatusReport());
        /// <summary>
        /// Displays the QRQC status report dialog for the currently selected document, if available.
        /// </summary>
        /// <remarks>If no document is selected, an error message is shown to the user. This method is
        /// intended to be called in response to user actions that require viewing the QRQC status report.</remarks>
        private void ShowingQRQCStatusReport()
        {
            if (!_rootName.IsNullOrEmpty())
            {
                NewQrqcStatusReportViewModel statusReportViewModel = new NewQrqcStatusReportViewModel(_dialogs);
                statusReportViewModel.Init(_rootName);

                var window = _windowFactory.Create<NewQrqcStatusReportWindow>();
                window.DataContext = statusReportViewModel;
                window.ShowDialog();
                LoadTree();
            }
            else
            {
                _dialogs.ShowErrorInfo($"Nincs kiválasztva dokumentum!", "MainWindowViewModel");
            }
        }

        public ICommand? ShowNewDocument => new ProjectCommandRelay(_ => ShowingNewDocument());
        /// <summary>
        /// Displays the dialog for adding a new document and refreshes the document tree after the dialog is closed.
        /// </summary>
        private void ShowingNewDocument()
        {
            var window = _windowFactory.Create<AddNewDocument>();
            window.ShowDialog();
            LoadTree();
        }
        public ICommand? ShowMeetingMemoWindow => new ProjectCommandRelay(_ => ShowingMeetingMemoWindow());
        /// <summary>
        /// Displays the meeting memo window as a modal dialog.
        /// </summary>
        /// <remarks>This method blocks execution until the meeting memo window is closed. Use this method
        /// when user input or review of meeting memos is required before proceeding.</remarks>
        private void ShowingMeetingMemoWindow()
        {
            var window = _windowFactory.Create<MeetingMemoWindow>();
            window.ShowDialog();
        }
        public ICommand? ShowSetupWindow => new ProjectCommandRelay(_ => ShowingSetupWindow());
        /// <summary>
        /// Displays the setup window as a modal dialog.
        /// </summary>
        /// <remarks>This method blocks execution until the setup window is closed by the user. Use this
        /// method when user input or configuration is required before proceeding.</remarks>
        private void ShowingSetupWindow()
        {
            var window = _windowFactory.Create<SetupWindow>();
            window.ShowDialog();
        }
        public ICommand? DeleteDocument => new ProjectCommandRelay(_ => DeletingDocument());
        /// <summary>
        /// Handles the process of displaying the document deletion dialog and refreshing the document tree after
        /// deletion.
        /// </summary>
        private void DeletingDocument()
        {
            var window = _windowFactory.Create<DeleteWindow>();
            window.ShowDialog();
            LoadTree();
        }

        public ICommand? SendMemo => new ProjectCommandRelay(_ => SendingMemo());
        /// <summary>
        /// Sends a meeting memo email to all configured recipients after user confirmation.
        /// </summary>
        /// <remarks>This method retrieves all open meeting minutes, builds an email message, and sends it
        /// using SMTP settings from the application configuration. If the user does not confirm the action, no email is
        /// sent. If an error occurs during the sending process, an error dialog is displayed to the user.</remarks>
        private void SendingMemo()
        {
            if (_dialogs.ShowConfirmation("Biztosan küldeni szeretné a meeting memo-t?", "Email küldés"))
            {
                try
                {
                    var databaseCollection = _connectionManagement.GetCollection<MeetingMinutes>(_connectionManagement.MeetingMemo);
                    var allDocuments = databaseCollection.Find(FilterDefinition<MeetingMinutes>.Empty).ToList();
                    BuildEmail email = new BuildEmail(allDocuments.Where(i => i.status == "Nyitott").OrderBy(i => i.pic).ToList());

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(RegistryManagement.ReadStringRegistryKey("SMTPUser"));
                    foreach (var item in SetupManagement.GetEmailList())
                    {
                        mail.To.Add(item);
                    }

                    mail.Subject = "QRQC Meeting memo";
                    mail.IsBodyHtml = true;
                    mail.Body = email.BuidEmailMessage();

                    SmtpClient smtp = new SmtpClient(
                        RegistryManagement.ReadStringRegistryKey("SMTPServer"),
                        int.Parse(RegistryManagement.ReadStringRegistryKey("SMTPPort")));
                    smtp.Credentials = new NetworkCredential(RegistryManagement.ReadStringRegistryKey("SMTPUser"), SetupManagement.GetSmtpPassword());
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    _dialogs.ShowErrorInfo($"Küldés sikertelen a következő hiba miatt : {ex.Message}", "MainWindowViewModel");
                }
            }
        }

        #endregion

        #region Constructor
        public MainWindowViewModel(IUserDialogService dialog,
            IWindowFactory windowFactory,
            IUserControlFunctions userControlFunctions,
            IConnectionManagement connectionManagement
            )
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var loading = new StartWindow();
            loading.Show();

            _dialogs = dialog;
            _windowFactory = windowFactory;
            _userControlFunctions = userControlFunctions;
            _connectionManagement = connectionManagement;
            try
            {
                TimerInit();
                LoadTree();
                LoadDataWithDapper();
                _inspectionMachines = _erpMachineCenters
                    .Where(m => m.MachineType == EnumMachineType.FFCInspectionMachine || m.MachineType == EnumMachineType.FFCManualInspection)
                    .Select(m => m.Workcenter)
                    .ToList();

                _manualMachines = _erpMachineCenters
                    .Where(m => m.MachineType == EnumMachineType.FFCManualProcess)
                    .Select(m => m.Workcenter)
                    .ToList();

                _machines = _erpMachineCenters
                    .Where(m => m.MachineType == EnumMachineType.FFCMachineProcess)
                    .Select(m => m.Workcenter)
                    .ToList();

            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"{ex.Message}", "MainWindowViewModel");
                //MessageBox.Show($"{ex.Message}", "MainWindowViewModel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                loading.Close();
                // visszaállítjuk az alapértelmezett viselkedést
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }

        }
        #endregion

        #region Private methods
        /// <summary>
        /// Initializes and starts a timer that triggers an event at regular intervals.
        /// </summary>
        private void TimerInit()
        {
            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(1);
            tmr.Tick += Tmr_Tick;
            tmr.Start();
        }

        /// <summary>
        /// Handles the timer tick event by updating the current time and notifying listeners of the change.
        /// </summary>
        /// <param name="sender">The source of the event, typically the timer that triggered the tick.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void Tmr_Tick(object? sender, EventArgs e)
        {
            _time = DateTime.Now.ToString();
            OnPropertyChanged(nameof(Time));
        }
        /// <summary>
        /// Loads and organizes hierarchical data into the <see cref="TreeNodes"/> collection.
        /// </summary>
        /// <remarks>This method retrieves all documents from the database and constructs a tree structure
        /// based on the relationships and categories within the documents. Each document is represented as a root node,
        /// with child nodes for specific categories such as "Headcount", "Machine process followup", "Inspection
        /// process followup", "Manual process followup", "Status reports", and "Status reports QRQC". The tree
        /// structure is updated and sorted in descending order by node name.</remarks>
        private void LoadTree()
        {
            //ConnectionManagement conMgmnt = new ConnectionManagement();
            var databaseCollection = _connectionManagement.GetCollection<MasterFollowupDocument>(_connectionManagement.DbName);
            var allDocuments = databaseCollection.Find(FilterDefinition<MasterFollowupDocument>.Empty).ToList();
            TreeNodes.Clear();
            foreach (var doc in allDocuments)
            {
                var docNode = new TreeNodeModel { Name = doc.DocumentName };

                if (doc.Headcount?.Any() == true)
                {
                    var headNode = new TreeNodeModel { Name = "Headcount" };
                    docNode.Children.Add(headNode);
                }

                if (doc.MachineFollowups?.Any() == true)
                {
                    var machineFollowupNode = new TreeNodeModel { Name = "Machine process followup" };
                    foreach (var b in doc.MachineFollowups)
                        machineFollowupNode.Children.Add(new TreeNodeModel { Name = b.Workcenter });
                    docNode.Children.Add(machineFollowupNode);
                }

                if (doc.InspectionFollowups.Any() == true)
                {
                    var commentsNode = new TreeNodeModel { Name = "Inspection process followup" };
                    foreach (var i in doc.InspectionFollowups)
                        commentsNode.Children.Add(new TreeNodeModel { Name = i.Workcenter });
                    docNode.Children.Add(commentsNode);
                }

                if (doc.ManualFollowups?.Any() == true)
                {
                    var commentsNode = new TreeNodeModel { Name = "Manual process followup" };
                    foreach (var c in doc.ManualFollowups)
                        commentsNode.Children.Add(new TreeNodeModel { Name = c.Workcenter });
                    docNode.Children.Add(commentsNode);
                }

                if (doc.StatusReports?.Any() == true)
                {
                    var statusNode = new TreeNodeModel { Name = "Status reports" };
                    foreach (var sn in doc.StatusReports)
                        statusNode.Children.Add(new TreeNodeModel { Name = sn.ReportName.ToString() });
                    docNode.Children.Add(statusNode);
                }

                if (doc.StatusReportsQRQC?.Any() == true)
                {
                    var statusNode = new TreeNodeModel { Name = "Status reports QRQC" };
                    foreach (var sn in doc.StatusReportsQRQC)
                        statusNode.Children.Add(new TreeNodeModel { Name = sn.ReportName.ToString() });
                    docNode.Children.Add(statusNode);
                }


                TreeNodes.Add(docNode);
                SetParents(docNode);
                TreeNodes = new ObservableCollection<TreeNodeModel>(
                        TreeNodes.OrderByDescending(t => t.Name)
                    );

                OnPropertyChanged(nameof(TreeNodes));
            }
        }

        private void LoadDataWithDapper()
        {
            try
            {
                DapperFunctions df = new DapperFunctions();
                _erpMachineCenters = df.GetErpMachineCenters();
            }
            catch (Exception ex)
            {
                _dialogs.ShowErrorInfo($"Hiba történt : {ex.Message}", "Gépcsoportlista betöltés");
            }
        }
        private void SetParents(TreeNodeModel parent)
        {
            foreach (var child in parent.Children)
            {
                child.Parent = parent;
                SetParents(child);
            }
        }
        #endregion
    }
}
