using AspNetCoreHero.Results;
using MediatR;

namespace Travl.Application.Password.Commands;

public class ResetPasswordCommand : IRequest<IResult<string>>
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    
}