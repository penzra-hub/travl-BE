using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Authentication.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, IResult<AuthToken>>
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(ApplicationContext context, UserManager<AppUser> userManager,
            ILogger<LoginCommandHandler> logger, ITokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<IResult<AuthToken>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authResult = new AuthToken();
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogInformation($"Login not successful. User does not exist: {request.Email}");
                    return await Result<AuthToken>.FailAsync("User does not exist");
                }

                if (user.UserType != request.UserType)
                {
                    _logger.LogInformation($"Login not successful. User does not exist: {request.Email}");
                    return await Result<AuthToken>.FailAsync("Unauthorized");
                }

                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    _logger.LogInformation($"Login not successful. Incorrect password: {request.Email}");
                    return await Result<AuthToken>.FailAsync("Incorrect password");
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogInformation($"Login not successful. User is yet to confirm email: {request.Email}");
                    return await Result<AuthToken>.FailAsync("Please confirm your email");
                }

                if (user.Status == Status.Inactive || user.Status == Status.Deactivated)
                {
                    _logger.LogInformation(
                        $"Login not successful. You have been disabled by Administrator. Contact your administrator for more information: {request.Email}");
                    return await Result<AuthToken>.FailAsync(
                        "You have been disabled by Administrator. Contact your administrator for more information");
                }

                _logger.LogInformation($"User Login was successful: {request.Email}");

                authResult.UserId = user.Id;
                authResult.UserToken = await _tokenService.GenerateUserToken(user);

                var refreshToken = await _tokenService.GenerateRefreshToken(request.Email, request.Password);
                authResult.RefreshToken = refreshToken.RefreshAccessToken;

                await _userManager.ResetAccessFailedCountAsync(user);

                // Increment the login count
                user.LoginCount++;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync(cancellationToken);

                return await Result<AuthToken>.SuccessAsync(authResult, "login successful");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error logging in : " + ex?.Message + ex?.InnerException?.Message);
                return await Result<AuthToken>.FailAsync("Error logging in : " + ex?.Message +
                                                         ex?.InnerException?.Message);
            }
        }
    }
}
