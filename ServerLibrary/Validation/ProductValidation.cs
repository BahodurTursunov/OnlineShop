using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("The name cannot be empty")
                .Length(0, 100)
                .WithMessage("The name must not be empty and must not exceed 100 characters.")
                .NotEqual("string");

            RuleFor(x => x.Description)
                .NotEmpty()
                .Length(0, 500)
                .WithMessage("The description must not exceed 500 characters.")
                .NotEqual("string");

            RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0);

            //RuleFor(x => x.Category)
            //    .SetValidator(new CategoryValidation());
        }
    }
}