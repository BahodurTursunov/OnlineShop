using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class UserValidation : AbstractValidator<User>
    {
        public UserValidation()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull();
            RuleFor(x => x.FirstName).NotEmpty().Length(0, 20);
            RuleFor(x => x.LastName).NotEmpty().Length(0, 20);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Username).NotNull().Length(1, 20).NotEmpty();
            RuleFor(x => x.PasswordHash).NotEmpty().NotNull().MinimumLength(6).WithMessage("Пароль должен иметь больше 6 символов");
        }
    }
}