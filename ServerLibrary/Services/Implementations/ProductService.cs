using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;
using System.Text.Json;

namespace ServerLibrary.Services.Implementations
{
    public class ProductService(
        ISqlRepository<Product> repository,
        ILogger<ProductService> logger,
        IEntityValidator<Product> validator,
        IDistributedCache cache) : IProductService

    {
        private readonly ISqlRepository<Product> _repository = repository;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IEntityValidator<Product> _validator = validator;
        private readonly IDistributedCache _cache = cache;

        public async Task<Product> Create(Product entity, CancellationToken cancellationToken)
        {
            _validator.Validate(entity);

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
            _validator.Validate(entity);

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
            string cacheKey = $"product: {id}";
            var cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cachedProduct))
            {
                _logger.LogInformation($"Returned product Id {id} from cache");
                return JsonSerializer.Deserialize<Product>(cachedProduct);
            }

            var product = await _repository.GetById(id, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                },
                cancellationToken);

            await _cac
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
    }
}
