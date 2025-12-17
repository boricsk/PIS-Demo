using ProdInfoSys.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
