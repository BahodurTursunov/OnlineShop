using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class OrderValidation : AbstractValidator<Order>
    {
        public OrderValidation()
        {
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0);
        }
    }
}