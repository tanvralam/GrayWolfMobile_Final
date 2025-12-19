using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IAuthService
    {
        string CurrentUserLogin { get; }
        bool IsLoggedIn { get; }

        Task LoginAsync(string login, string password);
        Task LogoutAsync();
    }
}
