using AspNetCoreHero.Results;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Authentication.Models;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;
using Travl.Domain.Context;

namespace Travl.Application.Authentication.Commands.Handler
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IResult<ApiResponse<RefreshToken>>>
    {
        private readonly ApplicationContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly IValidator<RefreshTokenCommand> _validator;
        public RefreshTokenCommandHandler(ApplicationContext context, ITokenService tokenService, ILogger<RefreshTokenCommandHandler> logger, IValidator<RefreshTokenCommand> validator)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
            _validator = validator;
        }

        public async Task<IResult<ApiResponse<RefreshToken>>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            //validate command
            var validateCommand = await _validator.ValidateAsync(request);
            if (!validateCommand.IsValid)
            {
                throw new ValidationException(validateCommand.Errors);
            }

            //fetch user!
            var checkIfUserExist = await _context.Users.FirstOrDefaultAsync(
                u => u.RefreshToken == request.refreshToken && u.RefreshTokenExpiryTime < DateTime.UtcNow && u.Token == request.token && u.IsTokenValid,
                cancellationToken
            );

            if(checkIfUserExist == null || checkIfUserExist.UserName == null || checkIfUserExist.PasswordHash == null)
            {
                return await Result<ApiResponse<RefreshToken>>.FailAsync("Refresh token or Token has been invalidated!");
            }

            try
            {
                var userToken = await _tokenService.GenerateUserToken(checkIfUserExist);
                var newResfreshToken = await _tokenService.GenerateRefreshToken(checkIfUserExist.UserName, checkIfUserExist.PasswordHash);

                checkIfUserExist.Token = userToken;
                checkIfUserExist.IsTokenValid = true;
                checkIfUserExist.RefreshToken = newResfreshToken.NewRefreshToken;
                checkIfUserExist.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(newResfreshToken.ExpiresIn);

                _context.Users.Update(checkIfUserExist);
                await _context.SaveChangesAsync();
                var authResult = new ApiResponse<RefreshToken>
                {
                    isSuccess = true,
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Refresh Token Generated Successfully!",
                    Data = new RefreshToken
                    {
                        ExpiresIn = newResfreshToken.ExpiresIn,
                        AccessToken = userToken,
                        NewRefreshToken = newResfreshToken.NewRefreshToken,
                    }
                };
                return await Result<ApiResponse<RefreshToken>>.SuccessAsync(authResult);
            }
            catch (Exception ex)
            {

                _logger.LogError("An Error occured here: " + ex.Message);
                throw;
            }
        }
    }
}
