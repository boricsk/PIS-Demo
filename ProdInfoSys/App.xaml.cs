using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProdInfoSys.Classes;
using ProdInfoSys.DI;
using ProdInfoSys.Interfaces;
using ProdInfoSys.ViewModels;
using ProdInfoSys.ViewModels.Nested;
using ProdInfoSys.Windows;
using ProdInfoSys.Windows.Nested;
using System.Windows;

namespace ProdInfoSys
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; }
        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Szolgáltatások regisztrálása
                    services.AddSingleton<IUserDialogService, UserDialogServices>();
                    services.AddSingleton<IUserControlFunctions, UserControlFunctions>();
                    services.AddSingleton<IConnectionManagement, ConnectionManagement>();

                    // ViewModel-ek és Window-k
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();

                    services.AddTransient<AddNewDocumentViewModel>();
                    services.AddTransient<AddNewDocument>();

                    services.AddTransient<DeleteWindowViewModel>();
                    services.AddTransient<DeleteWindow>();

                    services.AddTransient<MeetingMemoViewModel>();
                    services.AddTransient<MeetingMemoWindow>();

                    services.AddTransient<NewQrqcStatusReportViewModel>();
                    services.AddTransient<NewQrqcStatusReportWindow>();

                    services.AddTransient<NewStatusReportViewModel>();
                    services.AddTransient<StatusReportWindow>();

                    services.AddTransient<SetupWindowViewModel>();
                    services.AddTransient<SetupWindow>();

                    services.AddTransient<HeadcountViewModel>();
                    services.AddTransient<HeadcountUserControl>();

                    services.AddTransient<MachineViewModel>();
                    services.AddTransient<MachineUserControl>();

                    services.AddTransient<ManualViewModel>();
                    services.AddTransient<ManualUserControl>();

                    services.AddTransient<StatusReportViewModel>();
                    services.AddTransient<StatusReportWindow>();

                    //Az ablakok megnyitásához szükséges szolgáltatás
                    services.AddTransient<IWindowFactory, WindowFactory>();

                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = Host.Services.GetRequiredService<MainWindow>();
            //mainWindow.DataContext = Host.Services.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();
            base.OnStartup(e);
        }

    }
}
