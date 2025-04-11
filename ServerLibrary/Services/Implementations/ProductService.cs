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
        public async Task<Product> Create(Product entity)
        {
            var product = await _repository.CreateAsync(entity);
            if (product == null)
            {
                _logger.LogError($"Ошибка при добавлении продукта {entity.Name} в базу данных");
                throw new Exception($"Ошибка при добавлении продукта {entity.Name} в базу данных");
            }
            //await _db.AddAsync(entity);
            //await _db.SaveChangesAsync();

            _logger.LogInformation($"Продукт {entity.Name} успешно добавлен в базу данных");

            return entity;
        }

        public Task<Product> Delete(int id)
        {
            _logger.LogInformation($"Продукт с ID {id} удаляется из базы данных");
            return _repository.DeleteAsync(id);
        }

        public IQueryable<Product> GetAll()
        {
            return _repository.GetAll();
        }

        public async Task<Product> GetById(int id)
        {
            _logger.LogInformation($"Попытка получения товара с идентификатором {id} из базы данных");
            var product = await _repository.GetById(id);

            if (product is null)
            {
                _logger.LogWarning($"Товар с идентификатором {id} не найден в базе данных");
                throw new ArgumentNullException("Такого товара не существует");
            }
            return product;
        }

        public async Task<Product> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Имя товара не может быть пустым");
            }

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (product == null)
            {
                throw new KeyNotFoundException($"Товар с названием {name} не найден");
            }
            return product;
        }

        public async Task<Product> Update(int id, Product entity)
        {
            try
            {
                _logger.LogInformation($"Попытка обновления продукта {id}");

                var existingProduct = _repository.GetById(id);

                if (existingProduct is null)
                {
                    _logger.LogWarning($"Продукт с идентификатором {id} не найден в базе данных");
                    throw new KeyNotFoundException($"Продукт с идентификатором {id} не найден в базе данных");
                }
                /*_db.Entry(existingProduct).CurrentValues.SetValues(entity);*/

                return await _repository.UpdateAsync(entity);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
