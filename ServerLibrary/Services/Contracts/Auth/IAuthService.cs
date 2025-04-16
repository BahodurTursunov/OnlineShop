using BaseLibrary.DTOs;

namespace ServerLibrary.Services.Contracts.Auth
{
    public interface IAuthService
    {
        string Generate(string password, CancellationToken cancellationToken);
        bool Verify(string password, string passwordHash, CancellationToken cancellationToken);
        Task<UserDTO> RegisterAsync(RegisterUserDTO dto, CancellationToken cancellationToken);
        Task<string> LoginAsync(LoginUserDTO dto, CancellationToken cancellationToken);
        Task<UserDTO> GetUserProfileAsync(int userId, CancellationToken cancellationToken);
        Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenDTO dto, CancellationToken cancellationToken);


    }
}