using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Exceptions;
using ServerLibrary.Services.Contracts.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServerLibrary.Services.Implementations.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _dbContext;


        public AuthService(IConfiguration configuration, ILogger<AuthService> logger, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<string> LoginAsync(LoginUserDTO dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Неверный логин или пароль");
                throw new UnauthorizedAccessException("Неверный логин или пароль");
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation($"Пользователь {user.Username} успешно вошел в систему");

            return token;
        }

        public async Task<UserDTO> RegisterAsync(RegisterUserDTO dto)
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
                RoleId = 0,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            //return GenerateJwtToken(user);
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string Generate(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

    }
}