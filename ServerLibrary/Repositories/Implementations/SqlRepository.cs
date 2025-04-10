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

        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    _logger.LogWarning("Попытка добавить пустой объект в базу данных");
                    throw new ArgumentNullException(nameof(entity), "Объект не может быть пустым");
                }
                await _db.AddAsync(entity);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Объект {entity.GetType().Name} успешно добавлен в базу данных");
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при добавлении объекта {entity.GetType().Name} в базу данных");
                throw;
            }
        }

        public async Task<T> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Попытка удаления объекта {typeof(T).Name} с идентификатором {id} из базы данных");

                var item = await _db.Set<T>().FirstOrDefaultAsync(i => i.Id == id);

                if (item is null)
                {
                    _logger.LogWarning($"Объект {typeof(T).Name} с идентификатором {id} не найден в базе данных для удаления");
                    throw new KeyNotFoundException($"Объект {typeof(T).Name} с идентификатором {id} не найден в базе данных для удаления");
                }
                _db.Remove(item);
                await _db.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при удалении объекта {typeof(T).Name} из базы данных");
                throw;
            }
        }

        public IQueryable<T> GetAll()
        {
            return _db.Set<T>();
        }

        public async Task<T> GetById(int id)
        {
            _logger.LogInformation($"Попытка получения объекта {typeof(T).Name} с идентификатором {id} из базы данных");

            var item = await _db.Set<T>().FirstOrDefaultAsync(i => i.Id == id);
            if (item is null)
            {
                _logger.LogWarning($"Объект {typeof(T).Name} с идентификатором {id} не найден в базе данных");
                //throw new KeyNotFoundException($"Объект {typeof(T).Name} с идентификатором {id} не найден в базе данных");
                return null;
            }
            _logger.LogInformation($"Данные объекта {typeof(T).Name} с идентификатором {id} успешно прочитаны из базы данных");
            return item!;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _logger.LogInformation($"Попытка обновления объекта {entity.GetType().Name} в базе данных");
                var existingItem = await _db.Set<T>().FirstOrDefaultAsync(i => i.Id == entity.Id);
                if (existingItem != null)
                {
                    _db.Update(entity);
                    _db.SaveChanges();
                    _logger.LogInformation($"Объект {entity.GetType().Name} успешно обновлен в базе данных");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при обновлении объекта {entity.GetType().Name} в базе данных");
                throw;
            }
            return entity;
        }
    }
}
