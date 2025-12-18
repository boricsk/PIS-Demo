using System.Windows;

namespace ProdInfoSys.Interfaces
{
    /// <summary>
    /// Defines a factory for creating instances of Windows Presentation Foundation (WPF) windows.
    /// </summary>
    /// <remarks>Implementations of this interface provide a way to instantiate window objects, typically to
    /// support dependency injection or to decouple window creation from application logic. This can be useful for
    /// testing, modular application design, or when window construction requires additional setup.</remarks>
    public interface IWindowFactory
    {
        T Create<T>() where T : Window;
    }
}
