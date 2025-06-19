using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название продукта не может быть пустым.")
                .MaximumLength(100).WithMessage("Название продукта не должно превышать 100 символов.")
                .NotEqual("string").WithMessage("Название не должно быть 'string' по умолчанию.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов.")
                .NotEqual("string").WithMessage("Описание не должно быть 'string' по умолчанию.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше нуля.");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Скидка не может быть отрицательной.")
                .LessThanOrEqualTo(x => x.Price).WithMessage("Скидка не может быть больше цены.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Категория обязательна для указания.");
        }
    }
}
