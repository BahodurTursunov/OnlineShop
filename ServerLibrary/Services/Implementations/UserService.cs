using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Implementations;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class UserService(SqlRepository<User> repository, ILogger<UserService> logger, ApplicationDbContext db) : IUserService
    {
        private readonly SqlRepository<User> _repository = repository;
        private readonly ILogger<UserService> _logger = logger;
        private readonly ApplicationDbContext _db = db;
        public async Task<User> Create(User entity)
        {
            try
            {
                _logger.LogInformation($"Попытка добавления пользователя {entity.Username} в базу данных");

                if (string.IsNullOrWhiteSpace(entity.Username) || string.IsNullOrWhiteSpace(entity.Email))
                {
                    _logger.LogWarning("Ошибка валидации: Username и Email обязательны.");
                    throw new ArgumentException("Username и Email не могут быть пустыми.");
                }

                if (await _db.Users.AnyAsync(u => u.Username == entity.Username))
                {
                    _logger.LogWarning($"Пользователь с логином {entity.Username} уже существует.");
                    throw new ArgumentException($"Пользователь с логином {entity.Username} уже существует.");
                }

                if (await _db.Users.AnyAsync(e => e.Email == entity.Email))
                {
                    _logger.LogWarning($"Попытка создания пользователя с уже существующим Email: {entity.Email}");
                    throw new InvalidOperationException("Пользователь с таким Email уже существует.");
                }

                await _repository.CreateAsync(entity);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Пользователь {entity.Username} успешно добавлен в базу данных");
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при добавлении пользователя {entity.Username} в базу данных");
                throw;
            }
        }

        public async Task<User> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Попытка удаления пользователя ID: {Id}", id);

                var user = await _repository.GetById(id);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь с ID {Id} не найден", id);
                    return null;
                }

                await _repository.DeleteAsync(id);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Пользователь ID {Id} успешно удален", id);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя ID {Id}", id);
                throw;
            }
        }

        public IQueryable<User> GetAll()
        {
            _logger.Equals("Попытка получения всех пользователей из базы данных");
            return _repository.GetAll();
        }

        public async Task<User> GetById(int id)
        {
            _logger.LogInformation($"Попытка получения пользователя с идентификатором {id} из базы данных");
            var user = await _repository.GetById(id);

            if (user is null)
            {
                _logger.LogWarning($"Пользователь с идентификатором {id} не найден в базе данных");
                return null;
            }
            return user;
        }

        public async Task<User> Update(int id, User entity)
        {
            try
            {
                _logger.LogInformation($"Попытка обновления пользователя ID {id}");

                var existingUser = await _repository.GetById(id);
                if (existingUser is null)
                {
                    _logger.LogWarning($"Пользователь с идентификатором {id} не найден в базе данных");
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(entity.Username))
                {
                    if (await _db.Users.AnyAsync(u => u.Username == entity.Username && u.Id != id))
                    {
                        _logger.LogWarning("Попытка обновления на уже существующий Username: {Username}", entity.Username);
                        throw new InvalidOperationException("Пользователь с таким Username уже существует.");
                    }
                    existingUser.Username = entity.Username;
                }


                if (!string.IsNullOrWhiteSpace(entity.Email))
                {
                    if (await _db.Users.AnyAsync(u => u.Email == entity.Email && u.Id != id))
                    {
                        _logger.LogWarning("Попытка обновления на уже существующий Email: {Email}", entity.Email);
                        throw new InvalidOperationException("Пользователь с таким Email уже существует.");
                    }
                    existingUser.Email = entity.Email;
                }

                existingUser.FirstName = entity.FirstName ?? existingUser.FirstName;
                existingUser.LastName = entity.LastName ?? existingUser.LastName;

                _repository.UpdateAsync(existingUser);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Пользователь ID {Id} успешно обновлен", id);

                return existingUser;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении пользователя ID {id}");
                throw;
            }
        }
    }
}
