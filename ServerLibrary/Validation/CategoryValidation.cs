using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class CategoryValidation : AbstractValidator<Category>
    {
        public CategoryValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(100)
                .WithMessage("Название не должно превышать 100 символов");
        }
    }

}