using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Travl.Application.Interfaces;
using Travl.Domain.Entities;

namespace Travl.Infrastructure.Implementations
{
    public class UserValidationService : IUserValidationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserValidationService> _logger;

        public UserValidationService(UserManager<AppUser> userManager, ILogger<UserValidationService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email uniqueness");
                throw;
            }
        }

        public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
                return user == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking phone number uniqueness");
                throw;
            }
        }
    }
}
