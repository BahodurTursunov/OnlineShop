using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Exceptions;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class UserService(ISqlRepository<User> repository, ILogger<UserService> logger, ApplicationDbContext db) : IUserService
    {
        private readonly ISqlRepository<User> _repository = repository;
        private readonly ILogger<UserService> _logger = logger;
        private readonly ApplicationDbContext _db = db;
        public async Task<User> Create(User entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Попытка добавления пользователя {entity.Username} в базу данных");

            if (string.IsNullOrWhiteSpace(entity.Username) || string.IsNullOrWhiteSpace(entity.Email))
            {
                _logger.LogWarning("Ошибка валидации: Username и Email обязательны.");
                throw new ArgumentException("Username и Email не могут быть пустыми.");
            }

            if (await _db.Users.AnyAsync(u => u.Username == entity.Username, cancellationToken))
            {
                _logger.LogWarning($"Пользователь с логином {entity.Username} уже существует.");
                throw new UsernameAlreadyExitstException(/*$"Пользователь с логином {entity.Username} уже существует."*/);
            }

            if (await _db.Users.AnyAsync(e => e.Email == entity.Email, cancellationToken))
            {
                _logger.LogWarning($"Попытка создания пользователя с уже существующим Email: {entity.Email}");

                throw new UserMailAlreadyExistException("Пользователь с таким Email уже существует.");

            }

            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);
            await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation($"Пользователь {entity.Username} успешно добавлен в базу данных");
            return entity;
        }

        public async Task<User> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Попытка удаления пользователя ID: {id}");

            var user = await _repository.GetById(id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь с ID {id} не найден");
                throw new KeyNotFoundException($"Пользователь с ID {id} не найден");
            }

            await _repository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation($"Пользователь ID {id} успешно удален");

            return user;
        }

        public IQueryable<User> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Попытка получения всех пользователей из базы данных");
            return _repository.GetAll(cancellationToken);
        }

        public async Task<User> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Попытка получения пользователя с идентификатором {id} из базы данных");
            var user = await _repository.GetById(id, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning($"Пользователь с идентификатором {id} не найден в базе данных");
            }
            return user!;
        }

        public async Task<User> Update(int id, User entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Попытка обновления пользователя ID {id}");

            var existingUser = await _repository.GetById(id, cancellationToken);

            if (existingUser is null)
            {
                _logger.LogWarning($"Пользователь с идентификатором {id} не найден в базе данных");
                throw new KeyNotFoundException($"Пользователь с идентификатором {id} не найден в базе данных");
            }

            if (!string.IsNullOrWhiteSpace(entity.Username))
            {
                if (await _db.Users.AnyAsync(u => u.Username == entity.Username && u.Id != id, cancellationToken))
                {
                    _logger.LogWarning($"Попытка обновления на уже существующий логин: {entity.Username}");
                    throw new InvalidOperationException("Пользователь с таким логином уже существует.");
                }
                existingUser.Username = entity.Username;
            }


            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                if (await _db.Users.AnyAsync(u => u.Email == entity.Email && u.Id != id, cancellationToken))
                {
                    _logger.LogWarning($"Попытка обновления на уже существующий Email: {entity.Email}");
                    throw new InvalidOperationException("Пользователь с такой почтой уже существует.");
                }
                existingUser.Email = entity.Email;
            }

            existingUser.FirstName = entity.FirstName ?? existingUser.FirstName;
            existingUser.LastName = entity.LastName ?? existingUser.LastName;

            await _repository.UpdateAsync(existingUser, cancellationToken);
            _logger.LogInformation($"Пользователь ID {id} успешно обновлен");

            return existingUser;
        }
    }
}
