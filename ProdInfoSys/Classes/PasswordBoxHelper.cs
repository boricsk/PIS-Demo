using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides attached properties and helper methods to enable data binding of the password value for a PasswordBox
    /// control in WPF applications.
    /// </summary>
    /// <remarks>The PasswordBox control in WPF does not support direct data binding of its password for
    /// security reasons. The PasswordBoxHelper class enables binding scenarios by exposing an attached property that
    /// synchronizes the password value between the PasswordBox and a bound data source. Use this helper when you need
    /// to bind the password to a view model or other data context.</remarks>
    public class PasswordBoxHelper
    {
        /// <summary>
        /// Identifies the BoundPassword attached dependency property, which enables data binding of the password value
        /// for a PasswordBox control.
        /// </summary>
        /// <remarks>This property allows the password of a PasswordBox to be bound in XAML, overcoming
        /// the limitation that the Password property itself is not a dependency property and cannot be bound directly.
        /// Use this property in conjunction with appropriate logic to synchronize the bound value and the PasswordBox
        /// content securely.</remarks>
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        /// Sets the value of the bound password attached property on the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object on which to set the bound password property. Cannot be null.</param>
        /// <param name="value">The password value to set. Can be null or empty to clear the password.</param>
        public static void SetBoundPassword(DependencyObject obj, string value) => obj.SetValue(BoundPasswordProperty, value);

        private static bool _isUpdating;

        /// <summary>
        /// Handles changes to the bound password property and updates the associated PasswordBox control accordingly.
        /// </summary>
        /// <remarks>This method is typically used in attached property scenarios to synchronize the
        /// PasswordBox's Password property with a bound value. It ensures that the PasswordBox reflects the current
        /// value of the bound property without causing recursive updates.</remarks>
        /// <param name="d">The DependencyObject that is the target of the property change. Expected to be a PasswordBox.</param>
        /// <param name="e">The event data that contains information about the property change, including the old and new values.</param>
        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
                if (!_isUpdating)
                    passwordBox.Password = e.NewValue as string ?? string.Empty;
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Handles the PasswordChanged event for a PasswordBox to synchronize the bound password value.
        /// </summary>
        /// <remarks>This method is typically used in data binding scenarios to keep the bound password
        /// property in sync with the PasswordBox control. It should be attached to the PasswordChanged event of a
        /// PasswordBox.</remarks>
        /// <param name="sender">The source of the event, expected to be a PasswordBox instance.</param>
        /// <param name="e">The event data associated with the password change.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _isUpdating = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _isUpdating = false;
            }
        }
    }
}
