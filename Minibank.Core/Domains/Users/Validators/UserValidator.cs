using FluentValidation;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Login).NotNull().NotEmpty().WithMessage("Incorrect login!");
            RuleFor(x => x.Login).Matches(@"[^\s]").WithMessage("Login must not have some spaces!");
            RuleFor(x => x.Login.Length).LessThanOrEqualTo(20)
                .WithMessage("Login's length must not be more than 20 symbols!");
        }
    }
}