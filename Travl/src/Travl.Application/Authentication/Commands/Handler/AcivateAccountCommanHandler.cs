using AspNetCoreHero.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;
using Travl.Domain.Context;

namespace Travl.Application.Authentication.Commands.Handler
{
    public class AcivateAccountCommanHandler : IRequestHandler<ActivateAccountCommand, IResult<ApiResponse<string>>>
    {
        private readonly IStringHashingService _hashingService;
        private readonly ApplicationContext _context;
        private readonly ILogger<AcivateAccountCommanHandler> _logger;
        public AcivateAccountCommanHandler(IStringHashingService hashingService, ApplicationContext context, ILogger<AcivateAccountCommanHandler> logger)
        {
            _hashingService = hashingService;
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<ApiResponse<string>>> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
        {
            var checkUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.email);
            if(checkUser == null)
            {
                return await Result<ApiResponse<string>>.FailAsync($"User with email: {request.email} does not exist!");
            }
            if(checkUser.OtpExpiration < DateTime.UtcNow)
            {
                return await Result<ApiResponse<string>>.FailAsync("OTP has expired, try again, this time: Faster!!");
            }
            if(checkUser.Otp == _hashingService.CreateAESStringHash(request.otp))
            {
                checkUser.EmailConfirmed = true;
                try
                {
                    _context.Users.Update(checkUser);
                    await _context.SaveChangesAsync();

                    return await Result<ApiResponse<string>>.SuccessAsync("Account has been activated Successfully!");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An Error Occured here: {ex.Message} from trace: {ex.StackTrace}");
                    return await Result<ApiResponse<string>>.FailAsync("An Error occured, please contact Travl Support!");
                }
            }
            return await Result<ApiResponse<string>>.FailAsync("Failed to activate account at the moment! try again!");
        }
    }
}
