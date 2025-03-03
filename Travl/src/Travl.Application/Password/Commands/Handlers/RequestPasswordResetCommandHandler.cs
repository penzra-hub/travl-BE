using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Domain.Entities;
using IResult = AspNetCoreHero.Results.IResult;

namespace Travl.Application.Password.Commands.Handlers;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, IResult>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public RequestPasswordResetCommandHandler(UserManager<AppUser> userManager, ILogger<RequestPasswordResetCommandHandler> logger,
        IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<IResult> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Forgot password request: User with email {Email} was not found", request.Email);
            return Result.Fail("If the email exists, a password reset link will be sent to your email.");
        }
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        if (token == null)
        {
            return Result.Fail("Failed to generate link");
        }
        
        var webUrl = _configuration["AppSettings:WebUrl"];
        if (string.IsNullOrEmpty(webUrl))
        {
            _logger.LogError("The Web Url is not set in the appsettings.json file");
            return Result.Fail("Password reset service is currently unavailable.");
        }
        
        var resetLink = $"{webUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

        await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, token, "180");
        
        return Result.Success("Password reset link has been sent to your email.");
    }
    
}