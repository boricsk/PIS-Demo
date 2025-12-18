using ProdInfoSys.ViewModels;
using System.Windows;

namespace ProdInfoSys.Windows
{
    /// <summary>
    /// Interaction logic for DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        public DeleteWindow(DeleteWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
