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

        #region CRUD Operations
        public async Task<User> Create(User entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to add user {entity.Username} to the database.");

            if (string.IsNullOrWhiteSpace(entity.Username) || string.IsNullOrWhiteSpace(entity.Email))
            {
                _logger.LogWarning("Validation error: Username and Email are required.");
                throw new ArgumentException("Username and Email cannot be empty.");
            }

            if (await _db.Users.AnyAsync(u => u.Username == entity.Username, cancellationToken))
            {
                _logger.LogWarning($"User with username {entity.Username} already exists.");
                throw new UsernameAlreadyExistsException();
            }

            if (await _db.Users.AnyAsync(e => e.Email == entity.Email, cancellationToken))
            {
                _logger.LogWarning($"Attempt to create a user with an already existing email: {entity.Email}");
                throw new UserMailAlreadyExistsException();
            }

            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);
            await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation($"User {entity.Username} was successfully added to the database.");
            return entity;
        }

        public async Task<User> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to delete user with ID: {id}");

            var user = await _repository.GetById(id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} was not found.");
                throw new KeyNotFoundException($"User with ID {id} was not found.");
            }

            await _repository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation($"User with ID {id} was successfully deleted.");

            return user;
        }

        public IQueryable<User> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all users from the database.");
            return _repository.GetAll(cancellationToken);
        }

        public async Task<User> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to retrieve user with ID {id} from the database.");
            var user = await _repository.GetById(id, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning($"User with ID {id} was not found in the database.");
            }

            return user!;
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

            if (!string.IsNullOrWhiteSpace(entity.Username))
            {
                if (await _db.Users.AnyAsync(u => u.Username == entity.Username && u.Id != id, cancellationToken))
                {
                    _logger.LogWarning($"Attempt to update to an already existing username: {entity.Username}");
                    throw new InvalidOperationException("A user with this username already exists.");
                }
                existingUser.Username = entity.Username;
            }

            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                if (await _db.Users.AnyAsync(u => u.Email == entity.Email && u.Id != id, cancellationToken))
                {
                    _logger.LogWarning($"Attempt to update to an already existing email: {entity.Email}");
                    throw new InvalidOperationException("A user with this email already exists.");
                }
                existingUser.Email = entity.Email;
            }

            existingUser.FirstName = entity.FirstName ?? existingUser.FirstName;
            existingUser.LastName = entity.LastName ?? existingUser.LastName;

            await _repository.UpdateAsync(existingUser, cancellationToken);
            _logger.LogInformation($"User with ID {id} was successfully updated.");

            return existingUser;
        }
        #endregion


    }
}
