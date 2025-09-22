using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Exceptions;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Cache;

namespace ServerLibrary.Services.Implementations
{
    public class UserService(ISqlRepository<User> repository, ILogger<UserService> logger, IRedisCacheService<User> cache, IEntityValidator<User> validator) : IUserService
    {
        private readonly ISqlRepository<User> _repository = repository;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IEntityValidator<User> _validator = validator;
        private readonly IRedisCacheService<User> _cache = cache;

        #region CRUD Operations
        public async Task<User> Create(User user, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to add user {user.Username} to the database.");

            _validator.Validate(user);

            if (await _repository.GetAll(cancellationToken).AnyAsync(e => e.Email == user.Email, cancellationToken))
            {
                _logger.LogWarning($"Attempt to create a user with an already existing email: {user.Email}");
                throw new UserMailAlreadyExistsException();
            }

            if (await _repository.GetAll(cancellationToken).AnyAsync(u => u.Username == user.Username, cancellationToken))
            {
                _logger.LogWarning($"User with username {user.Username} already exists.");
                throw new UsernameAlreadyExistsException();
            }

            /*  if (await _db.Users.AnyAsync(e => e.Email == user.Email, cancellationToken))
              {
                  _logger.LogWarning($"Attempt to create a user with an already existing email: {user.Email}");
                  throw new UserMailAlreadyExistsException();
              }*/

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            var createdUser = await _repository.CreateAsync(user, cancellationToken);

            _logger.LogInformation($"User {user.Username} was successfully added to the database.");
            return createdUser;
        }

        public async Task<User> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to delete user with ID: {id}");

            var deletedUser = await _repository.DeleteAsync(id, cancellationToken);

            await InvalidateUserCacheAsync(id, cancellationToken);

            _logger.LogInformation($"User with ID {id} was successfully deleted.");

            return deletedUser;
        }

        public IQueryable<User> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all users from the database.");

            return _repository.GetAll(cancellationToken);
        }

        public async Task<User> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to retrieve user with ID {id}.");
            var cacheKey = $"user:{id}";

            var user = await _cache.GetAsync(cacheKey, cancellationToken);
            if (user != null)
            {
                _logger.LogInformation($"User with ID {id} has been retrieved from the cache.");
                return user;
            }

            user = await _repository.GetById(id, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning($"User with ID {id} was not found in the database.");
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            // Кэшируем только если нашли
            await _cache.SetAsync(cacheKey, user, absoluteExpiration: TimeSpan.FromMinutes(10), slidingExpiration: TimeSpan.FromMinutes(5), cancellationToken: cancellationToken);
            _logger.LogInformation($"User with ID {id} has been retrieved from DB and cached.");

            return user;
        }

        public async Task<User> Update(int id, User entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to update user with ID {id}.");

            var existingUser = await _repository.GetById(id, cancellationToken);

            if (existingUser is null)
            {
                _logger.LogWarning($"User with ID {id} was not found in the database.");
                throw new KeyNotFoundException($"User with ID {id} was not found in the database.");
            }

            if (!string.IsNullOrWhiteSpace(entity.Username) && existingUser.Username != entity.Username)
            {
                if (await _repository.GetAll(cancellationToken).AnyAsync(u => u.Username == entity.Username && u.Id != id, cancellationToken))
                {
                    _logger.LogWarning($"Attempt to update to an already existing username: {entity.Username}");
                    throw new InvalidOperationException("A user with this username already exists.");
                }
                existingUser.Username = entity.Username;
            }

            if (!string.IsNullOrWhiteSpace(entity.Email) && existingUser.Email != entity.Email)
            {
                if (await _repository.GetAll(cancellationToken).AnyAsync(u => u.Email == entity.Email && u.Id != id, cancellationToken))
                {
                    _logger.LogWarning($"Attempt to update to an already existing email: {entity.Email}");
                    throw new InvalidOperationException("A user with this email already exists.");
                }
                existingUser.Email = entity.Email;
            }

            existingUser.FirstName = entity.FirstName ?? existingUser.FirstName;
            existingUser.LastName = entity.LastName ?? existingUser.LastName;

            var updatedUser = await _repository.UpdateAsync(existingUser, cancellationToken);

            await InvalidateUserCacheAsync(id, cancellationToken);

            _logger.LogInformation($"User with ID {id} was successfully updated.");

            return updatedUser;
        }
        #endregion

        private async Task InvalidateUserCacheAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                string cacheKey = $"user:{userId}";
                await _cache.RemoveAsync(cacheKey, cancellationToken);
                _logger.LogInformation("Invalidated cache for user ID {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate cache for user ID {UserId}", userId);
            }
        }
    }
}
