using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.Interfaces;
using Travl.Infrastructure.Implementations;

namespace Travl.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DriverController(IDriverService driverService, IHttpContextAccessor httpContextAccessor)
        {
            _driverService = driverService;
            _httpContextAccessor = httpContextAccessor; 
        }

        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity?.Claims.Select(c => new { c.Type, c.Value });

            return Ok(claims);
        }


        [HttpGet("{driverId}")]
        public async Task<IActionResult> Get(string? driverId)
        {
            // Get the user's role from claims

            var userId = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("User role not found");
            }

            GetDriverDto? driver = null;

            // Determine which method to call based on the user's role

            if (userRole.Equals("Admin"))
            {
                //driver = await _driverService.GetDriverForAdminAsync(driverId);
            }
            else if (userRole.Equals("Passenger"))
            {
                driver = await _driverService.GetDriverForPassengerAsync(driverId);
            }

            if (driver == null)
            {
                return NotFound("Driver not found");
            }

            return Ok(driver);

        }


        [HttpPut("update-driver/{id}")]
        public async Task<IActionResult> UpdateDriver([FromBody] UpdateDriverDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (userId == null) return BadRequest("User Id is null");

            var result = await _driverService.UpdateDriverAsync(userId, updateDto);

            if (!result.Succeeded)
            {
                return StatusCode(500, result.Message);
            }
            
            return Ok(result.Message);
        }
    }
}
