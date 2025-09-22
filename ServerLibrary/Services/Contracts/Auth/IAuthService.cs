using BaseLibrary.DTOs;

namespace ServerLibrary.Services.Contracts.Auth
{
    public interface IAuthService
    {
        Task<CreateUserDTO> RegisterAsync(RegisterUserDTO dto, CancellationToken cancellationToken);
        Task<AuthResponseDTO> LoginAsync(LoginUserDTO dto, CancellationToken cancellationToken);
        Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenDTO dto, CancellationToken cancellationToken);
    }
}