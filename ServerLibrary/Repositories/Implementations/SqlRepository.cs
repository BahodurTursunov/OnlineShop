using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class SqlRepository<T>(ApplicationDbContext db, ILogger<SqlRepository<T>> logger) : ISqlRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _db = db;
        private readonly ILogger<SqlRepository<T>> _logger = logger;

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                _logger.LogWarning("Попытка добавить пустой объект в базу данных");
                return null;
            }

            try
            {
                await _db.AddAsync(entity, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Объект {entity.GetType().Name} успешно добавлен в базу данных");
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при добавлении объекта {entity?.GetType().Name} в базу данных");
                return null;
            }
        }

        public async Task<T> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Попытка удаления объекта {typeof(T).Name} с ID {id}");

            var item = await _db.Set<T>().FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                _logger.LogWarning($"Объект {typeof(T).Name} с ID {id} не найден для удаления");
                return null;
            }

            _db.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Объект {typeof(T).Name} с ID {id} успешно удалён");
            return item;

        }

        public IQueryable<T> GetAll(CancellationToken cancellationToken) => _db.Set<T>();

        public async Task<T> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Чтение объекта {typeof(T).Name} с ID {id}");

            var item = await _db.Set<T>().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (item == null)
            {
                _logger.LogWarning($"Объект {typeof(T).Name} с ID {id} не найден");
            }

            return item;
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                _logger.LogWarning("Попытка обновить пустой объект");
                return null;
            }

            var existingItem = await _db.Set<T>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == entity.Id, cancellationToken);

            if (existingItem != null)
            {
                _db.Update(entity);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Объект {entity.GetType().Name} успешно обновлен в базе данных");
            }
            else
            {
                return null;
            }

            return entity;
        }
    }
}
