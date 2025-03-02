using AspNetCoreHero.Results;
using MediatR;

namespace Travl.Application.Password.Commands;

public record RequestPasswordResetCommand(string Email) : IRequest<IResult>;
