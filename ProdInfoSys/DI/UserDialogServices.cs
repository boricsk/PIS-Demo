using System.Windows;
using MsgBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ProdInfoSys.DI
{
    /// <summary>
    /// Provides methods for displaying common user dialogs, such as confirmation prompts, informational messages, and
    /// error notifications.
    /// </summary>
    /// <remarks>This class implements the IUserDialogService interface to standardize user interaction
    /// dialogs across the application. Dialogs are typically modal and block user interaction with the parent window
    /// until dismissed.</remarks>
    public class UserDialogServices : IUserDialogService
    {
        public bool ShowConfirmation(string message, string title = "Confirmation")
        {
            var result = MsgBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Displays an error message dialog box with the specified message and title.
        /// </summary>
        /// <param name="message">The error message to display in the dialog box. Cannot be null.</param>
        /// <param name="title">The title of the dialog box. Defaults to "Error" if not specified.</param>
        public void ShowErrorInfo(string message, string title = "Error")
        {
            MsgBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Displays an informational message box with the specified message and title.
        /// </summary>
        /// <param name="message">The text to display in the message box. Cannot be null or empty.</param>
        /// <param name="title">The title to display in the message box caption. If not specified, defaults to "Information".</param>
        public void ShowInfo(string message, string title = "Information")
        {
            MsgBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
