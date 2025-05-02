using AspNetCoreHero.Results;
using MediatR;
using Travl.Domain.Commons;

namespace Travl.Application.Authentication.Commands
{
    public record RequestActivationTokenCommand(
        string email
        ) : IRequest<IResult<ApiResponse<string>>>;
}
