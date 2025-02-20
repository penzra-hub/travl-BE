using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;
using Travl.Domain.Context;
using Travl.Domain.Entities;

namespace Travl.Application.Authentication.Commands.Handler
{
    public class RequestActivationTokenCommandHandler : IRequestHandler<RequestActivationTokenCommand, IResult<ApiResponse<string>>>
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IStringHashingService _hashingService;
        private readonly IConfiguration _config;
        private readonly ILogger<RequestActivationTokenCommandHandler> _logger;
        public RequestActivationTokenCommandHandler(IEmailService emailService, ApplicationContext context, UserManager<AppUser> userManager,
            ITokenService tokenService, IStringHashingService hashingService, IConfiguration config, ILogger<RequestActivationTokenCommandHandler> logger)
        {
            _emailService = emailService;
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _hashingService = hashingService;
            _config = config;
            _logger = logger;
        }

        public async Task<IResult<ApiResponse<string>>> Handle(RequestActivationTokenCommand request, CancellationToken cancellationToken)
        {
            var checkUser = await _userManager.FindByEmailAsync(request.email);
            if(checkUser == null || checkUser.FirstName == null)
            {
                return await Result<ApiResponse<string>>.FailAsync($"User with email: {request.email} does not exist!");
            }
            try
            {
                int tokenExpiry = int.Parse(_config.GetSection("RefreshTokenConstants")["OTPExpiryInMin"]!);
                var generateToken = _tokenService.GenerateOtp();
                checkUser.Otp = _hashingService.CreateAESStringHash(generateToken);
                checkUser.OtpExpiration = DateTime.UtcNow.AddMinutes(tokenExpiry);

                var sendOtpOverEmail = await _emailService.SendOTPMail(request.email, checkUser.FirstName, generateToken, tokenExpiry.ToString());
                if (sendOtpOverEmail)
                {
                    await _userManager.UpdateAsync(checkUser);
                    await _context.SaveChangesAsync();

                    return await Result<ApiResponse<string>>.SuccessAsync($"Otp Sent to {request.email} successfull, expires in {tokenExpiry}Mins");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error Occured here: {ex.Message} on trace: {ex.StackTrace}");
                return await Result<ApiResponse<string>>.FailAsync("An Error occured while trying to send you OTP, please try again or contact Travl support!");
            }
            return await Result<ApiResponse<string>>.FailAsync("An Error occured while trying to send you OTP, please try again or contact Travl support!");
        }
    }
}
