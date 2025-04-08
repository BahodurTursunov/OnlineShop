using BaseLibrary.Entities;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Implementations;

namespace ServerLibrary.Functions
{
    public class UserFunctions
    {
        private readonly ISqlRepository<User> _repository;
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _db;

        public UserFunctions(ISqlRepository<User> repository, ILogger<UserService> logger, ApplicationDbContext db)
        {
            _db = db;
            _repository = repository;
            _logger = logger;
        }
        public async Task IsUsernameUnique(User entity)
        {

        }
    }
}
