using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MsgBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ProdInfoSys.DI
{
    public class UserDialogServices : IUserDialogService
    {
        public bool ShowConfirmation(string message, string title = "Confirmation")
        {
            var result = MsgBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public void ShowErrorInfo(string message, string title = "Error")
        {
            MsgBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string message, string title = "Information")
        {
            MsgBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
