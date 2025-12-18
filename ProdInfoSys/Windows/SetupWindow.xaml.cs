using ProdInfoSys.ViewModels;
using System.Windows;

namespace ProdInfoSys.Windows
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow(SetupWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        public SetupWindow()
        {

        }
    }
}
