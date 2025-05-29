using BaseLibrary.DTOs;
using BaseLibrary.Entities;

namespace ServerLibrary.Services.Contracts
{
    public interface IUserService : IBaseService<User>
    {
        Task<AuthResponseDTO> RegistegAsync(RegisterUserDTO dto, CancellationToken cancellationToken);
        Task<AuthResponseDTO> LoginAsync(LoginUserDTO dto, CancellationToken cancellationToken);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken);
        Task<UserDTO> GetUserProfileAsync(int userId, CancellationToken cancellationToken);
    }
}
