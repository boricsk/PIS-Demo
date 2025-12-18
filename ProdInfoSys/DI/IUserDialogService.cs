namespace ProdInfoSys.DI
{
    public interface IUserDialogService
    {
        /// <summary>
        /// Displays an informational message to the user in a dialog box.
        /// </summary>
        /// <param name="message">The message text to display in the dialog box. Cannot be null or empty.</param>
        /// <param name="title">The title of the dialog box. If not specified, defaults to "Information".</param>
        void ShowInfo(string message, string title = "Information");
        void ShowErrorInfo(string message, string title = "Error");
        bool ShowConfirmation(string message, string title = "Confirmation");
    }
}
