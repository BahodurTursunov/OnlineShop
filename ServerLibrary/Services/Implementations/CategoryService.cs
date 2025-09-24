using BaseLibrary.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ServerLibrary.Services.Implementations
{
    public class CategoryService(
        ISqlRepository<Category> repository,
        ILogger<CategoryService> logger,
        IValidator<Category> validator) : ICategoryService
    {
        private readonly ISqlRepository<Category> _repository = repository;
        private readonly ILogger<CategoryService> _logger = logger;
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
            var existing = await _repository.GetAll(cancellationToken)
    .AnyAsync(c => c.Name.ToLower() == entity.Name.ToLower(), cancellationToken);

            if (existing)
            {
                _logger.LogWarning($"Category with name {entity.Name} already exists.");
                throw new InvalidOperationException($"Category with name {entity.Name} already exists.");
            }

            var createdCategory = await _repository.CreateAsync(entity, cancellationToken);
            _logger.LogInformation($"Category {entity.Name} was created successfully.");
            return createdCategory;
        }

        public async Task<Category> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting category with ID {id}");

            var deleteCategory = await _repository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation($"Category with ID {id} was deleted successfully.");
            return deleteCategory;
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

            var existing = await _repository.GetById(id, cancellationToken);

            if (existing == null)
            {
                _logger.LogWarning($"Category with ID {id} was not found.");
                throw new KeyNotFoundException($"Category with ID {id} was not found.");
            }

            var nameConflict = await _repository.GetAll(cancellationToken)
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == entity.Name.ToLower(), cancellationToken);

            if (nameConflict != null)
            {
                _logger.LogWarning($"Another category with name {entity.Name} already exists.");
                throw new InvalidOperationException($"Another category with name {entity.Name} already exists.");
            }

            // Apply changes
            existing.Name = entity.Name;
            existing.UpdatedAt = DateTime.UtcNow;

            var updatedCategory = await _repository.UpdateAsync(existing, cancellationToken);

            _logger.LogInformation($"Category with ID {id} was updated successfully.");
            return updatedCategory;
        }

    }
}
