using ProdInfoSys.ViewModels;
using System.Windows;

namespace ProdInfoSys.Windows
{
    /// <summary>
    /// Interaction logic for AddNewDocument.xaml
    /// </summary>
    public partial class AddNewDocument : Window
    {
        public AddNewDocument(AddNewDocumentViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
