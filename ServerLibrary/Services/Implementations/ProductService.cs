using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Cache;

namespace ServerLibrary.Services.Implementations
{
    public class ProductService(
        ISqlRepository<Product> repository,
        ILogger<ProductService> logger,
        IEntityValidator<Product> validator,
        IRedisCacheService cache) : IProductService

    {
        private readonly ISqlRepository<Product> _repository = repository;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IEntityValidator<Product> _validator = validator;
        private readonly IRedisCacheService _cache = cache;

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

        public IQueryable<Product> GetAll(CancellationToken cancellationToken)
        {
            return _repository.GetAll(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllCached(CancellationToken cancellationToken = default)
        {
            const string cacheKey = "products:all";

            var cached = await _cache.GetAsync<List<Product>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                _logger.LogInformation("Returned all products from cache.");
                return cached;
            }

            var products = await _repository.GetAll(cancellationToken).ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5), cancellationToken: cancellationToken);

            return products;
        }

        public async Task<Product> GetById(int id, CancellationToken cancellationToken)
        {
            string cacheKey = $"product:{id}";
            var cached = await _cache.GetAsync<Product>(cacheKey, cancellationToken);

            if (cached is not null)
            {
                _logger.LogInformation("Returned product Id {Id} from cache", id);
                return cached;
            }

            var product = await _repository.GetById(id, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {Id} not found", id);
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(2), cancellationToken);
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
    }
}
