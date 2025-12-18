using ProdInfoSys.ViewModels;
using System.Windows;

namespace ProdInfoSys.Windows
{
    /// <summary>
    /// Interaction logic for MeetingMemoWindow.xaml
    /// </summary>
    public partial class MeetingMemoWindow : Window
    {
        public MeetingMemoWindow(MeetingMemoViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
