using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travl.Application.DTOs;
using Travl.Domain.Entities;

namespace Travl.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PasswordController(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity?.Claims.Select(c => new { c.Type, c.Value });

            return Ok(claims);
        }


        [Authorize]
        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto passwordDto)
        {
            
            if(!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }

            if(passwordDto.NewPassword != passwordDto.ConfirmPassword)
            {
                
                return BadRequest(new
                {
                    message = "New password and confirmation password do not match"
                });
            }

            //  var userId = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c=>c.Type=="Id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                
                return NotFound(new { message = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, passwordDto.OldPassword, passwordDto.NewPassword);

            if (!result.Succeeded)
            {
                
                return BadRequest(result.Errors);
            }
           
                return Ok(new { message = "Password updated successfully" });
            
        }
    }
}
