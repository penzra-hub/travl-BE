using AspNetCoreHero.Results;
using MediatR;
using Travl.Application.Authentication.Models;

namespace Travl.Application.Authentication.Commands
{
    public partial class LoginCommand : UserAuth, IRequest<IResult<AuthToken>>
    {
    }
}
