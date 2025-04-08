using BaseLibrary.DTOs;

namespace ServerLibrary.Services.Contracts.Auth
{
    public interface IAuthService
    {
        string Generate(string password);
        bool Verify(string password, string passwordHash);
        Task<UserDTO> RegisterAsync(RegisterUserDTO dto);
        Task<string> LoginAsync(LoginUserDTO dto);
    }
}