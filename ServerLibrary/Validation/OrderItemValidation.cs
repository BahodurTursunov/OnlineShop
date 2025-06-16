using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class OrderItemValidation : AbstractValidator<OrderItem>
    {
        public OrderItemValidation()
        {
            RuleFor(x => x.Quantity)
                .NotEmpty();

            RuleFor(x => x.UnitPrice)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("The price must be positive and greater than 0.");
        }
    }
}
