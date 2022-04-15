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
            RuleFor(x => x.Currency).Must((x) => x.Length > 2  && x.Length < 4 && Enum.IsDefined(typeof(PermittedCurrencies),
                Char.ToString(x[0]).ToUpper() + x.Substring(1, x.Length - 1).ToLower())).
                WithMessage(Messages.NotPermittedCurrency);
        }
    }
    
    public class BankAccountValidate : IValidator<BankAccount>
    {
        public bool IsCalled { get; set; }

        public BankAccountValidate()
        {
            IsCalled = false;
        }
        public ValidationResult Validate(IValidationContext context)
        {
            IsCalled = true;
            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = new CancellationToken())
        {
            IsCalled = true;
            return new ValidationResult();
        }

        public IValidatorDescriptor CreateDescriptor()
        {
            return new ValidatorDescriptor<BankAccount>(new List<IValidationRule>());
        }

        public bool CanValidateInstancesOfType(Type type)
        {
            return true;
        }

        public ValidationResult Validate(BankAccount instance)
        {
            IsCalled = true;
            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateAsync(BankAccount instance, CancellationToken cancellation = new CancellationToken())
        {
            IsCalled = true;
            return new ValidationResult();
        }
    }

    

}