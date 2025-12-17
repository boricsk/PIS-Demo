using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProdInfoSys.Interfaces
{
    public interface IWindowFactory
    {
        T Create<T>() where T : Window;
        //T CreateUserControl<T>() where T : UserControl;
    }
}
