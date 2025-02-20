using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Common;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Commons;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Authentication.Commands.Handler
{
    public class SignupCommandHandler : IRequestHandler<SignupCommand, ApiResponse<Guid>>
    {
        private readonly ApplicationContext _context; 
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<SignupCommand> _validator;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<SignupCommandHandler> _logger;
        private readonly IStringHashingService _hashingToken;
        private readonly IConfiguration _configuration;
        private readonly IDriverRepository _driverRepository;
        public SignupCommandHandler(ApplicationContext context, IValidator<SignupCommand> validator, UserManager<AppUser> userManager, IEmailService emailService,
            ITokenService tokenService, ILogger<SignupCommandHandler> logger, IStringHashingService hashingToken, IConfiguration configuration, IDriverRepository driverRepository)
        {
            _context = context;
            _validator = validator;
            _userManager = userManager;
            _emailService = emailService;
            _tokenService = tokenService;
            _logger = logger;
            _hashingToken = hashingToken;
            _configuration = configuration;
            _driverRepository = driverRepository;
        }

        public async Task<ApiResponse<Guid>> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            //check if user exist
            var response = new ApiResponse<Guid>();
            var validateRequest = _validator.Validate(request);
            if (!validateRequest.IsValid)
            {
                throw new ValidationException(validateRequest.Errors);
            }

            var user = _context.Users.FirstOrDefault(n => n.Email == request.emailAddress || n.PhoneNumber == request.phoneNumber);
            if (user != null)
            {
                response.Message = $"User with Email: {user.Email} or Phone number: {user.PhoneNumber} already exist!";
                return response;
            }

            //generate otp
            var otp = _tokenService.GenerateOtp();
            int otpExpiry = int.Parse(_configuration.GetSection("RefreshTokenConstants")["OTPExpiryInMin"]!);
            //create user
            var newAppUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.firstName,
                LastName = request.lastName,
                Email = request.emailAddress,
                EmailConfirmed = false,
                Gender = request.gender,
                PhoneNumber = request.phoneNumber,
                PhoneNumberConfirmed = false,
                PasswordHash = _userManager.PasswordHasher.HashPassword(null, request.password),
                UserName = request.username,
                UserType = request.userType,
                Status = Status.Active,
                AccessLevel = AccessLevel.User,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                NormalizedEmail = request.emailAddress,
                Avatar = request.avater,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                LockoutEnabled = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                Otp = _hashingToken.CreateAESStringHash(otp),
                OtpExpiration = DateTime.UtcNow.AddMinutes(otpExpiry),
            };

            try
            {
                // send verification mail / sms
                 var sendEmail = await _emailService.SendOtpVerificationEmailAsync(request.emailAddress, request.firstName, otp, otpExpiry.ToString());

                using var transactions = await _context.Database.BeginTransactionAsync();
                try
                {

                    await _context.Users.AddAsync(newAppUser);
                    await _context.SaveChangesAsync();

                    // Create driver profile for user whose role is driver
                    if (newAppUser.UserType == UserType.Driver)
                        await _driverRepository.CreateDriverProfile(newAppUser);

                    await transactions.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transactions.RollbackAsync();
                    _logger.LogError($"An error occured here: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured here while trying to create driver: {ex.Message}");
                throw;
            }

            //return response
            response.isSuccess = true;
            response.ResponseCode = ResponseCodes.SUCCESS;
            response.Message = "Registration Successfull";
            response.Data = Guid.Parse(newAppUser.Id);
            return response;
        }
    }
}
