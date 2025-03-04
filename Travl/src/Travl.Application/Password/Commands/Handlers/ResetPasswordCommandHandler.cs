using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Domain.Entities;

namespace Travl.Application.Password.Commands.Handlers;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand,IResult<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly IEmailService _emailService;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager, ILogger<ResetPasswordCommandHandler> logger, IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<IResult<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Reset password request: User with email {Email} not found}", request.Email);
            return await Result<string>.FailAsync("Invalid request.");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return await Result<string>.FailAsync("Passwords do not match");
        }
        
        var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!resetResult.Succeeded)
        {
            var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
            _logger.LogError("Reset password failed: {Errors}", errors);
            return await Result<string>.FailAsync("Password reset failed");
        }
        
        await _emailService.SendPasswordResetSuccess(user.Email,user.FirstName);
        
        return await Result<string>.SuccessAsync( "Password has been reset successfully.");
        
        
    }
    
}