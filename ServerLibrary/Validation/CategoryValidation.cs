using BaseLibrary.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ServerLibrary.Validation
{
    public class CategoryValidation : AbstractValidator<Category>
    {
        public CategoryValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The category name must not be empty.")
                .MinimumLength(3).WithMessage("The category name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("The category name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("The category name can only contain letters, numbers, spaces, and hyphens.");
        }
    }
}