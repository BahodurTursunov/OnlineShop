using BaseLibrary.DTOs;
using BaseLibrary.Entities;

namespace ServerLibrary.Services.Contracts.Auth
{
    public interface IJwtService
    {
        Task<AuthResponseDTO> GenerateTokenAsync(User user, CancellationToken cancellationToken);
        Task<User> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
