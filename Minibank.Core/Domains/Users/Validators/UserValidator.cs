using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using FluentValidation;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Login).NotEmpty().WithMessage("Login must not be empty!");
            RuleFor(x => x.Login).Must(it => !it.Contains(" ")).WithMessage("Login must not have some spaces!");
            RuleFor(x => x.Login.Length).LessThanOrEqualTo(20)
                .WithMessage("Login's length must not be more than 20 symbols!");
            RuleFor(x => x.Login).Matches(@"^git [a-zA-Z0-9]+$").WithMessage("Login is not in correct format!");
            RuleFor(x => x).Must((x) => !userRepository.ContainsLogin(x.Login).Result)
                .WithMessage("This login is already used!");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must not be empty!");
            RuleFor(x => x.Email).Must(it => !it.Contains(" ")).WithMessage("Email must not have some spaces!");
            RuleFor(x => x.Email).Matches(@"^[a-zA-Z0-9@.]+$").Must(it => it.EndsWith("@mail.ru"))
                 .WithMessage("Email is not in correct format!");


            RuleFor(x => x).Must((x) => !userRepository.ContainsEmail(x.Email).Result)
                .WithMessage("This email is already used!");
        }
    }
}