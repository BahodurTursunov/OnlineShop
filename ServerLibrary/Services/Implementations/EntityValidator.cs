using FluentValidation;
using Microsoft.Extensions.Logging;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class EntityValidator<T>(IValidator<T> validator, ILogger<EntityValidator<T>> logger)
        : IEntityValidator<T>
        where T : class
    {
        public void Validate(T entity)
        {
            var validationResult = validator.Validate(entity);
            if(!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Validation failed: {Errors}", errors);
                throw new ValidationException(errors);
            }
        }
    }
}
