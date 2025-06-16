using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class CategoryValidation : AbstractValidator<Category>
    {
        public CategoryValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("The title must not exceed 100 characters.")
                .WithErrorCode("The title must not be empty.");
        }
    }
}