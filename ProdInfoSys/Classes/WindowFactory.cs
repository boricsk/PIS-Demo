using Microsoft.Extensions.DependencyInjection;
using ProdInfoSys.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProdInfoSys.Classes
{
    public class WindowFactory: IWindowFactory
    {
        private readonly IServiceProvider _sp;
        public WindowFactory(IServiceProvider sp) => _sp = sp;
        public T Create<T>() where T : Window => _sp.GetRequiredService<T>();
        //public T CreateUserControl<T>() where T : UserControl => _sp.GetRequiredService<T>();
    }
}
