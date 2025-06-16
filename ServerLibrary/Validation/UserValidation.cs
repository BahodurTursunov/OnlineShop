using BaseLibrary.Entities;
using FluentValidation;

namespace ServerLibrary.Validation
{
    public class UserValidation : AbstractValidator<User>
    {
        public UserValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(1, 20)
                .WithMessage("The name must not exceed 20 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(1, 20)
                .WithMessage("The lastname must not exceed 20 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithErrorCode("Email can not be empty");

            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(1, 20)
                .WithMessage("The username must not exceed 20 characters");

            RuleFor(x => x.PasswordHash)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("The password must be longer than 6 characters.");
        }
    }
}