using BaseLibrary.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ServerLibrary.Services.Implementations
{
    public class CategoryService(
        ISqlRepository<Category> repository,
        ILogger<CategoryService> logger,
        IValidator<Category> validator,
        ApplicationDbContext db) : ICategoryService
    {
        private readonly ISqlRepository<Category> _repository = repository;
        private readonly ILogger<CategoryService> _logger = logger;
        private readonly ApplicationDbContext _db = db;
        private readonly IValidator<Category> _validator = validator;

        public async Task<Category> Create(Category entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                _logger.LogWarning("Category entity is null");
                throw new ArgumentNullException(nameof(entity));
            }

            ValidationResult validationResult = await _validator.ValidateAsync(entity, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning($"Category validation failed: {errors}");
                throw new ValidationException(errors);
            }
            var existing = _db.Categories
             .AsEnumerable() // executes SQL and compares in memory
             .FirstOrDefault(c => c.Name.Equals(entity.Name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                _logger.LogWarning($"Category with name {entity.Name} already exists.");
                throw new InvalidOperationException($"Category with name {entity.Name} already exists.");
            }

            await _db.Categories.AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Category {entity.Name} was created successfully.");
            return entity;
        }

        public async Task<Category> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting category with ID {id}");

            var category = await _db.Categories.FindAsync(id, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} was not found.");
                throw new KeyNotFoundException($"Category with ID {id} was not found.");
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Category with ID {id} was deleted successfully.");
            return category;
        }

        public IQueryable<Category> GetAll(CancellationToken cancellationToken)
        {
            return _repository.GetAll(cancellationToken);
        }

        public async Task<Category> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching category with ID {id}");
            var category = await _repository.GetById(id, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID {id} was not found.");
                throw new KeyNotFoundException($"Category with ID {id} was not found.");
            }

            return category;
        }

        public async Task<Category> Update(int id, Category entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to update category with ID {id}");

            if (entity == null)
            {
                _logger.LogWarning("Provided category entity is null.");
                throw new ArgumentNullException(nameof(entity));
            }

            ValidationResult validationResult = await _validator.ValidateAsync(entity, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(x => x.ErrorMessage));
                _logger.LogWarning($"Category validation failed: {errors}");
                throw new FluentValidation.ValidationException(errors);
            }

            var existing = await _db.Categories.FindAsync(id, cancellationToken);

            if (existing == null)
            {
                _logger.LogWarning($"Category with ID {id} was not found.");
                throw new KeyNotFoundException($"Category with ID {id} was not found.");
            }

            // Check if another category with the same name already exists
            var nameConflict = _db.Categories
                .AsEnumerable()
                .FirstOrDefault(c =>
                    c.Id != id &&
                    c.Name.Equals(entity.Name, StringComparison.OrdinalIgnoreCase));

            if (nameConflict != null)
            {
                _logger.LogWarning($"Another category with name {entity.Name} already exists.");
                throw new InvalidOperationException($"Another category with name {entity.Name} already exists.");
            }

            // Apply changes
            existing.Name = entity.Name;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing, cancellationToken);

            _logger.LogInformation($"Category with ID {id} was updated successfully.");
            return existing;
        }

    }
}
