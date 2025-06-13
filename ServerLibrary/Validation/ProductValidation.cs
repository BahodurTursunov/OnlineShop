using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull();
            RuleFor(x => x.Name).NotEmpty().Length(0, 50);
            RuleFor(x => x.Description).NotEmpty().Length(0, 500);
            RuleFor(x => x.Price).NotEmpty().NotNull().GreaterThan(0);
        }
    }

}