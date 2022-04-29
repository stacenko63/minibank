using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Login).NotEmpty().WithMessage(Messages.EmptyLogin);
            RuleFor(x => x.Login).Must(it => !it.Contains(" ")).WithMessage(Messages.LoginWithSpaces);
            RuleFor(x => x.Login.Length).LessThanOrEqualTo(20)
                .WithMessage(Messages.LoginWithLengthMoreThan20);
            RuleFor(x => x.Login).Matches(@"^[a-zA-Z0-9]+$").WithMessage(Messages.LoginFormat);
            
            RuleFor(x => x.Email).NotEmpty().WithMessage(Messages.EmptyEmail);
            RuleFor(x => x.Email).Must(it => !it.Contains(" ")).WithMessage(Messages.EmailWithSpaces);
            RuleFor(x => x.Email).Matches(@"^[a-zA-Z0-9@.]+$")
                 .WithMessage(Messages.EmailFormat);
            RuleFor(x => x.Email).Must(it => it.EndsWith("@mail.ru")).WithMessage(Messages.EmailNotMailRu);
        }
    }
}