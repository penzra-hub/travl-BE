using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Commons;

namespace Travl.Application.Authentication.Commands
{
    public record RequestActivationTokenCommand(
        string email
        ) : IRequest<IResult<ApiResponse<string>>>;
}
