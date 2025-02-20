using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Authentication.Commands.CommandValidators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.token).NotEmpty();
            RuleFor(x => x.refreshToken).NotEmpty();
        }
    }
}
