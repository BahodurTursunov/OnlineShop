using BaseLibrary.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class ProductService(
        ISqlRepository<Product> repository,
        ILogger<ProductService> logger,
        IValidator<Product> validator) : IProductService
    {
        private readonly ISqlRepository<Product> _repository = repository;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IValidator<Product> _validator = validator;

        public async Task<Product> Create(Product entity, CancellationToken cancellationToken)
        {
            ValidateProduct(entity);

            Product created = await _repository.CreateAsync(entity, cancellationToken);

            if (created == null)
            {
                _logger.LogError("Failed to create product: {Name}", entity.Name);
                throw new InvalidOperationException($"Failed to create product '{entity.Name}'");
            }

            _logger.LogInformation("Created product: {Name}", created.Name);
            return created;
        }

        public async Task<Product> Update(int id, Product entity, CancellationToken cancellationToken)
        {
            ValidateProduct(entity);

            Product? existing = await _repository.GetById(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.Price = entity.Price;
            existing.CategoryId = entity.CategoryId;
            existing.Stock = entity.Stock;
            existing.Discount = entity.Discount;
            existing.UpdatedAt = DateTime.UtcNow;

            Product updated = await _repository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("Updated product ID {Id}", updated.Id);

            return updated;
        }

        public async Task<Product> Delete(int id, CancellationToken cancellationToken)
        {
            Product? existing = await _repository.GetById(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            _logger.LogInformation("Deleting product ID {Id}", id);
            return await _repository.DeleteAsync(id, cancellationToken);
        }

        public IQueryable<Product> GetAll(CancellationToken cancellationToken)
        {
            return _repository.GetAll(cancellationToken);
        }

        public async Task<Product> GetById(int id, CancellationToken cancellationToken)
        {
            Product? product = await _repository.GetById(id, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            return product;
        }

        public async Task<Product> GetByName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
            }

            Product? product = await _repository.GetAll(cancellationToken)
                .FirstOrDefaultAsync(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase), cancellationToken);

            if (product == null)
            {
                _logger.LogWarning("Product with name '{Name}' not found", name);
                throw new KeyNotFoundException($"Product with name '{name}' not found.");
            }

            return product;
        }

        private void ValidateProduct(Product product)
        {
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(product);

            if (!validationResult.IsValid)
            {
                string errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Product validation failed: {Errors}", errors);
                throw new ValidationException(errors);
            }
        }
    }
}
