using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ISqlRepository<Product> _repository;
        private readonly ILogger<ProductService> _logger;
        private readonly ApplicationDbContext _db;

        public ProductService(ISqlRepository<Product> repository, ILogger<ProductService> logger, ApplicationDbContext db)
        {
            _db = db;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Product> Create(Product entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                _logger.LogWarning("Product entity cannot be null.");
                throw new ArgumentNullException(nameof(entity), "Product entity cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                _logger.LogWarning("Product name cannot be empty.");
                throw new ArgumentException("Product name cannot be empty.", nameof(entity.Name));
            }

            var created = await _repository.CreateAsync(entity, cancellationToken);
            if (created == null)
            {
                _logger.LogError($"Failed to add product {entity.Name} to the database.");
                throw new InvalidOperationException($"Failed to add product {entity.Name} to the database.");
            }

            _logger.LogInformation($"Product {entity.Name} was successfully added to the database.");
            return created;
        }

        public async Task<Product> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to delete product with ID {id}.");

            var existing = await _repository.GetById(id, cancellationToken);
            if (existing == null)
            {
                _logger.LogWarning($"Product with ID {id} was not found.");
                throw new KeyNotFoundException($"Product with ID {id} was not found.");
            }

            return await _repository.DeleteAsync(id, cancellationToken);
        }

        public IQueryable<Product> GetAll(CancellationToken cancellationToken)
        {
            return _repository.GetAll(cancellationToken);
        }

        public async Task<Product> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving product with ID {id}.");

            var product = await _repository.GetById(id, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} was not found.");
                throw new KeyNotFoundException($"Product with ID {id} was not found.");
            }

            return product;
        }

        public async Task<Product> GetByName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Product name cannot be null or empty.");
                throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
            }

            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase), cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"Product with name '{name}' was not found.");
                throw new KeyNotFoundException($"Product with name '{name}' was not found.");
            }

            return product;
        }

        public async Task<Product> Update(int id, Product entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Attempting to update product with ID {id}.");

            if (entity == null)
            {
                _logger.LogWarning("Product entity cannot be null.");
                throw new ArgumentNullException(nameof(entity), "Product entity cannot be null.");
            }

            var existingProduct = await _repository.GetById(id, cancellationToken);
            if (existingProduct == null)
            {
                _logger.LogWarning($"Product with ID {id} was not found.");
                throw new KeyNotFoundException($"Product with ID {id} was not found.");
            }

            // Check for duplicate product name (exclude self)
            var duplicate = await _db.Products
              .Where(p => p.Id != id && p.Name.ToLower() == entity.Name.ToLower())
              .FirstOrDefaultAsync(cancellationToken);

            if (duplicate != null)
            {
                _logger.LogWarning($"Product with name '{entity.Name}' already exists.");
                throw new InvalidOperationException($"Product with name '{entity.Name}' already exists.");
            }

            // Update allowed fields only
            existingProduct.Name = entity.Name;
            existingProduct.Description = entity.Description;
            existingProduct.Price = entity.Price;
            existingProduct.CategoryId = entity.CategoryId;
            existingProduct.Stock = entity.Stock;

            // Save changes via repository
            var updated = await _repository.UpdateAsync(existingProduct, cancellationToken);
            _logger.LogInformation($"Product with ID {id} was successfully updated.");

            return updated;
        }

    }
}
