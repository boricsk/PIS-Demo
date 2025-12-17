using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.DI
{
    public interface IUserDialogService
    {
        void ShowInfo(string message, string title = "Information");
        void ShowErrorInfo(string message, string title = "Error");
        bool ShowConfirmation(string message, string title = "Confirmation");
    }
}
