using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Authentication.Models;
using Travl.Domain.Commons;

namespace Travl.Application.Authentication.Commands
{
    public record RefreshTokenCommand(
        string email,
        string token,
        string refreshToken
        ) : IRequest<IResult<ApiResponse<RefreshToken>>>;
}
