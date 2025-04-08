using BaseLibrary.Entities;

namespace ServerLibrary.Services.Contracts.Auth
{
    public interface IAuthService
    {
        Task<string> Register(User user, string password);
        Task<string> Login(string username, string password);
    }
}
