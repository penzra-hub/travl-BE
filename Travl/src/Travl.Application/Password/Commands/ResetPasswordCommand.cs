using AspNetCoreHero.Results;
using MediatR;

namespace Travl.Application.Password.Commands;

public record ResetPasswordCommand(string Email) : IRequest<IResult>;
