using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Commons;
using Travl.Domain.Context;
using Travl.Domain.Entities;

namespace Travl.Application.Authentication.Commands.Handler
{
    public class LogoutCommanHandler : IRequestHandler<LogoutCommand, IResult<string>>
    {
        public UserManager<AppUser> _userManager;
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContext;
        public LogoutCommanHandler(UserManager<AppUser> userManager, IHttpContextAccessor httpContext, ApplicationContext context)
        {
            _userManager = userManager;
            _httpContext = httpContext;
            _context = context;
        }
        public (string, string) Functionalities()
        {
            var Email = _httpContext.HttpContext.Items["Email"]?.ToString();
            var Role = _httpContext.HttpContext.Items["Role"]?.ToString() ?? "";

            return (Email, Role);
        }

        public async Task<IResult<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            string logedInUserEmail = Functionalities().Item1;
            var user = await _userManager.FindByEmailAsync(logedInUserEmail);
            var checkUserStatus = await _context.Users.FirstOrDefaultAsync(u => u.Email == logedInUserEmail && u.IsTokenValid && u.RefreshTokenExpiryTime > DateTime.UtcNow);

            if(user != null && checkUserStatus != null)
            {
                user.IsTokenValid = false;
                user.RefreshTokenExpiryTime = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();

                var authResult = new ApiResponse<string>
                {
                    isSuccess = true,
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Logout Sucessful"
                };
                return await Result<string>.SuccessAsync("Logout successful");
            }
            return await Result<string>.SuccessAsync("Logout Successful");
        }
    }
}
