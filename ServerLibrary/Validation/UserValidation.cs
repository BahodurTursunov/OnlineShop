using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class UserValidation : AbstractValidator<User>
    {
        public UserValidation()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(1, 20)
                .WithMessage("Имя не должно превышать 20 символов.")
                .NotEqual("string").WithMessage("Заполните имя корректно");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(1, 20)
                .WithMessage("Фамилия не должна превышать 20 символов.")
                .NotEqual("string").WithMessage("Заполните фамилию корректно");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithErrorCode("Почта не может быть пустой");

            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(1, 20)
                .WithMessage("Имя пользователя не должна превышать 20 символов")
                .NotEqual("string").WithMessage("Заполните имя пользователя корректно");

            RuleFor(x => x.PasswordHash)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Пароль не должен быть менее 6 символов");
        }
    }
}