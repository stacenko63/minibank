using System;
using System.Data;
using FluentValidation;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Enums;

namespace Minibank.Core.Domains.BankAccounts.Validators
{
    public class BankAccountValidator : AbstractValidator<BankAccount>
    {

        public BankAccountValidator()
        {
            RuleFor(x => x.Balance).GreaterThanOrEqualTo(0).WithMessage("StartBalance must be more than 0 or equal 0!");
            RuleFor(x => x.Currency).NotNull().NotEmpty().WithMessage("This currency is incorrect!");
            RuleFor(x => x.Currency).Must((x) => x.Length > 2  && x.Length < 4 && Enum.IsDefined(typeof(PermittedCurrencies),
                Char.ToString(x[0]).ToUpper() + x.Substring(1, x.Length - 1).ToLower())).
                WithMessage("This currency is unavailable at the moment!");
        }
    }
}