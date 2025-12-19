using System;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public interface IAlertService
    {
        Task ShowAlert(string message, string title = "", string cancel = "");
        Task ShowMessage(string message, string title = "", string cancel = "");

        Task<bool> ShowMessageConfirmation(string message, string title = "", string accept = "",
            string cancel = "");
        Task DisplayError(Exception exception, string friendlyMessage = null);
        Task<string> DisplayActionSheet(string Title, string cancel, string o, string[] items);

        void Toast(string message);
    }
}