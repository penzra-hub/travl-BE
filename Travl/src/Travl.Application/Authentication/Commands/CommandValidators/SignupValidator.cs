using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Authentication.Commands.CommandValidators
{
    public class SignupValidator : AbstractValidator<SignupCommand>
    {
        public SignupValidator()
        {
            RuleFor(x => x.firstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.lastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.emailAddress)
                 .NotEmpty().WithMessage("Email is required")
                 .EmailAddress().WithMessage("Invalid email format")
                 .Must(email => email.Contains('@') && email.Contains('.'))
                 .WithMessage("Email must contain '@' and '.'")
                 .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.phoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .MaximumLength(11).WithMessage("Phone number cann not be more than 11 numbers");
        }
    }
}
