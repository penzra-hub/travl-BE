using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Password.Commands.Handlers;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, IResult<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly int ExpirationTimeInHours = 3;

    public RequestPasswordResetCommandHandler(UserManager<AppUser> userManager, ILogger<RequestPasswordResetCommandHandler> logger, IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<IResult<string>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Password reset request: User with email {Email} was not found", request.Email);
            return await Result<string>.FailAsync("If the email exists in our system, you will receive a password reset link shortly.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        if (token == null)
        {
            return await Result<string>.FailAsync("Failed to generate password reset link. Please try again later.");
        }

        await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, token, ExpirationTimeInHours.ToString(), TimeFormat.Hour);

        user.Token = token;
        user.TokenExpirationTime = DateTime.UtcNow.AddHours(ExpirationTimeInHours);
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(token, "A password reset link has been sent to your email address. Please check your inbox.");
    }

}