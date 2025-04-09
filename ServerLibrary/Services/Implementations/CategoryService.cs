using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ISqlRepository<Category> _repository;
        private readonly ILogger<CategoryService> _logger;
        private readonly ApplicationDbContext _db;

        public CategoryService(
            ISqlRepository<Category> repository,
            ILogger<CategoryService> logger,
            ApplicationDbContext db)
        {
            _repository = repository;
            _logger = logger;
            _db = db;
        }

        public async Task<Category> Create(Category entity)
        {
            var existing = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.Equals(entity.Name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                _logger.LogWarning($"Категория с именем {entity.Name} уже существует.");
                throw new InvalidOperationException($"Категория с именем {entity.Name} уже существует.");
            }

            await _db.Categories.AddAsync(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Категория {entity.Name} успешно создана.");
            return entity;
        }

        public async Task<Category> Delete(int id)
        {
            _logger.LogInformation($"Удаление категории с ID {id}");

            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning($"Категория с ID {id} не найдена.");
                throw new KeyNotFoundException($"Категория с ID {id} не найдена.");
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Категория с ID {id} успешно удалена.");
            return category;
        }

        public IQueryable<Category> GetAll()
        {
            return _repository.GetAll();
        }

        public async Task<Category> GetById(int id)
        {
            _logger.LogInformation($"Поиск категории с ID {id}");
            var category = await _repository.GetById(id);

            if (category == null)
            {
                _logger.LogWarning($"Категория с ID {id} не найдена.");
                throw new KeyNotFoundException($"Категория с ID {id} не найдена.");
            }

            return category;
        }

        public async Task<Category> Update(int id, Category entity)
        {
            _logger.LogInformation($"Попытка обновления категории с ID {id}");

            var existing = await _db.Categories.FindAsync(id);
            if (existing == null)
            {
                _logger.LogWarning($"Категория с ID {id} не найдена.");
                throw new KeyNotFoundException($"Категория с ID {id} не найдена.");
            }

            _db.Entry(existing).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Категория с ID {id} успешно обновлена.");
            return existing;
        }
    }
}
