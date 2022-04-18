using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Enums;

namespace Minibank.Core.Domains.BankAccounts.Validators
{
    public class BankAccountValidator : AbstractValidator<BankAccount>
    {
        public BankAccountValidator()
        {
            RuleFor(x => x.Balance).GreaterThanOrEqualTo(0).WithMessage(Messages.NegativeStartBalance);
            RuleFor(x => x.Currency).NotNull().NotEmpty().WithMessage(Messages.IncorrectCurrency);
            
            RuleFor(x => x.Currency).Must((x) => x.Length > 2  && x.Length < 4 
                                                               && Enum.IsDefined(typeof(PermittedCurrencies),
                Char.ToString(x[0]).ToUpper() + x.Substring(1, x.Length - 1).ToLower())).
                WithMessage(Messages.NotPermittedCurrency);
        }
    }
}