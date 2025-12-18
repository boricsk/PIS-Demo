using Microsoft.Extensions.DependencyInjection;
using ProdInfoSys.Interfaces;
using System.Windows;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides a factory for creating window instances using dependency injection.
    /// </summary>
    /// <remarks>This class is typically used to instantiate window types that require services to be injected
    /// via the application's service provider. It abstracts the creation logic and ensures that dependencies are
    /// resolved according to the application's configuration.</remarks>
    public class WindowFactory : IWindowFactory
    {
        private readonly IServiceProvider _sp;
        public WindowFactory(IServiceProvider sp) => _sp = sp;
        public T Create<T>() where T : Window => _sp.GetRequiredService<T>();
    }
}
