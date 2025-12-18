using System.Windows;

namespace ProdInfoSys.Classes
{
    //A beágyazott ablakokban lévő DataGirid beállításainak szabályozását végző osztály mivel a data grid
    //nem a grid nem örökli a data contextet.

    /// <summary>
    /// Provides a binding proxy that enables data binding to elements that are not part of the visual or logical tree,
    /// such as embedded DataGrid controls.
    /// </summary>
    /// <remarks>Use this class to facilitate data binding scenarios where the target element does not inherit
    /// the data context, such as when binding within templates or nested controls. The BindingProxy acts as an
    /// intermediary, allowing data to be passed to controls that otherwise would not have access to the desired data
    /// context.</remarks>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// Creates a new instance of the BindingProxy class.
        /// </summary>
        /// <remarks>This method is called by the Freezable base class to create a new instance of the
        /// derived BindingProxy class. It is typically not called directly from user code.</remarks>
        /// <returns>A new instance of BindingProxy.</returns>
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        /// <summary>
        /// Gets or sets the data content associated with this element.
        /// </summary>
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        /// Identifies the Data dependency property.
        /// </summary>
        /// <remarks>This field is used to register and reference the Data property with the WPF property
        /// system. It is typically used when calling methods such as SetValue or GetValue on instances of
        /// BindingProxy.</remarks>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy));

    }
}
