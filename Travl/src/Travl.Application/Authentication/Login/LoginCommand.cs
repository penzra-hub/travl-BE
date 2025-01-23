using AspNetCoreHero.Results;
using MediatR;

namespace Travl.Application.Authentication.Login
{
    public partial class LoginCommand : UserAuth, IRequest<IResult<AuthToken>>
    {
    }
}
