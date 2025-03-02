using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;

namespace Travl.Api.Extensions
{
    public class CustomJwtAuthentication
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomJwtAuthentication> _logger;
        private readonly IConfiguration _configuration;

        public CustomJwtAuthentication(RequestDelegate next, ILogger<CustomJwtAuthentication> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var userId = context.User.Identity != null ? context.User.Identity.IsAuthenticated ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null : null;
            //log visits to sme application
            //await _authRepo.LogVisitsAsync(ipAddress, userAgent, userId);
            var allowedRoutes = new List<string>
            {
                "/api/v1/Authentication/login",
                "/api/v1/Authentication/signup",
                "/api/v1/HealthCheck/WelcomeEmailTest",
                "/api/v1/Authentication/request-activation-token",
                "/api/v1/Authentication/activate-account",
                "/api/v1/Password/request-password-reset"
            };

            if (allowedRoutes.Contains(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var result = new ApiResponse<string>
                {
                    ResponseCode = "06",
                    isSuccess = false,
                    Message = "Token is not provided!"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            //check if token has been invalidated
            if (!await serviceProvider.GetRequiredService<IUserValidationService>().IsTokenValidAsync(token))
            {
                context.Response.StatusCode = 401;
                var result = new ApiResponse<string>
                {
                    ResponseCode = "06",
                    isSuccess = false,
                    Message = "Token has been invalidated!"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                context.User = principal;
                context.Items["Email"] = principal.FindFirst("Email")?.Value;
                //context.Items["UserName"] = principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
                //context.Items["UserRole"] = principal.FindFirst(ClaimTypes.Role)?.Value;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogError("Token expired.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var result = new ApiResponse<string>
                {
                    ResponseCode = "06",
                    isSuccess = false,
                    Message = "Token is Expired"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var result = new ApiResponse<string>
                {
                    ResponseCode = "06",
                    isSuccess = false,
                    Message = "Invalid Authorization or Expired token"
                };
                await context.Response.WriteAsJsonAsync(result);
                return;
            }

            await _next(context);
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var responseObj = new ApiResponse<string>
                {
                    isSuccess = false,
                    ResponseCode = "06",
                    Message = "You are not authorized to perform this function, contact Support"
                };
                var jsonresp = JsonConvert.SerializeObject(responseObj);
                await context.Response.WriteAsync(jsonresp);
            }
        }
    }
}
