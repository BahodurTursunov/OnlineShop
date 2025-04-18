using AutoMapper;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Exceptions;
using ServerLibrary.Services.Contracts.Auth;

namespace ServerLibrary.Services.Implementations.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;


        public AuthService(ILogger<AuthService> logger, ApplicationDbContext dbContext, IJwtService jwtService, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginUserDTO dto, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"Пользователь с логином {dto.Username} не найден.");
                throw new UnauthorizedAccessException("Неверный логин или пароль");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Неверный логин или пароль");
                throw new UnauthorizedAccessException("Your password is incorrect!");
            }

            var token = await _jwtService.GenerateTokenAsync(user, cancellationToken);

            var person = _mapper.Map<AuthResponseDTO>(user);
            var response = _mapper.Map<AuthResponseDTO>(user);
            response.AccessToken = token.AccessToken;
            response.RefreshToken = token.RefreshToken;
            response.ExpiresAt = token.ExpiresAt;

            _logger.LogInformation($"Пользователь {user.Username} успешно вошел в систему");

            return response;
        }

        public async Task<UserDTO> RegisterAsync(RegisterUserDTO dto, CancellationToken cancellationToken)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
            {
                _logger.LogWarning("Пользователь с таким логином или почтой уже существует");
                throw new UsernameAlreadyExitstException();
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PasswordHash = Generate(dto.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            //return GenerateJwtToken(user);
        }

        /*public string GenerateJwtToken(User user, CancellationToken cancellationToken)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
*/

        public string Generate(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public async Task<UserDTO> GetUserProfileAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден");
            }

            return new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                throw new ArgumentException("Refresh токен обязателен.");
            }

            // Валидация и получение пользователя по refresh токену
            var user = await _jwtService.ValidateRefreshTokenAsync(dto.RefreshToken, cancellationToken);

            if (user == null)
            {
                throw new SecurityTokenException("Недействительный или истёкший refresh токен.");
            }

            // Генерация новых access и refresh токенов
            var tokens = await _jwtService.GenerateTokenAsync(user, cancellationToken);

            return tokens;
        }


    }
}