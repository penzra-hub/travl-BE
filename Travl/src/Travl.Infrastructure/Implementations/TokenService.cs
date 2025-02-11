using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;
using Travl.Domain.Entities;
using Travl.Infrastructure.Utility;

namespace Travl.Infrastructure.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger,
            UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<string> GenerateUserToken(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim("Email", user.Email),
                new Claim("UserType", ((int)user.UserType).ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                authClaims.Add(new Claim("FirstName", user.FirstName));

            if (!string.IsNullOrWhiteSpace(user.LastName))
                authClaims.Add(new Claim("LastName", user.LastName));

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                authClaims.Add(new Claim("PhoneNumber", user.PhoneNumber));

            //Gets the roles of the logged in user and adds it to Claims
            var roles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            authClaims.AddRange(roles.Select(role => new Claim("Role", role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            _ = int.TryParse(_configuration["JwtSettings:TokenValidityInMinutes"], out var tokenValidityInMinutes);

            // Specifying JWTSecurityToken Parameters
            var token = new JwtSecurityToken
            (audience: _configuration["JwtSettings:Audience"],
                issuer: _configuration["JwtSettings:Issuer"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthToken> GenerateAccessToken(AppUser user)
        {
            try
            {
                _logger.LogInformation($"About to generate access token");
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("userid", user.Id),
                };

                if (user.FirstName != null && user.LastName != null)
                {
                    claims.Add(new Claim("name", user.FirstName + " " + user.LastName));
                }

                JwtSecurityToken token = new TokenBuilder()
                    .AddAudience(_configuration["JwtSettings:Audience"])
                    .AddIssuer(_configuration["JwtSettings:Issuer"])
                    .AddExpiry(Convert.ToInt32(_configuration["JwtSettings:TokenValidityInMinutes"]))
                    .AddKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]))
                    .AddClaims(claims)
                    .Build();

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new AuthToken()
                {
                    //AccessToken = accessToken,
                    //ExpiresIn = Convert.ToInt32(_configuration["JwtSettings:TokenValidityInMinutes"]),
                };
                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoginDetails> DecodeRefreshToken(string refreshToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var decodedValue = handler.ReadJwtToken(refreshToken);
                //var token = new JwtSecurityToken(refreshToken);

                //return login details
                var tokenParam = new LoginDetails()
                {
                    UserName = decodedValue.Claims.First(Claim => Claim.Type == "userName").Value,
                    Password = decodedValue.Claims.First(Claim => Claim.Type == "hash").Value
                };

                return await Task.FromResult(tokenParam);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RefreshToken> GenerateRefreshToken(string userName, string password)
        {
            try
            {
                List<Claim> myClaims = new List<Claim>()
                {
					//new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim("userName", userName),
                    new Claim("hash", password)
                };

                JwtSecurityToken token = new TokenBuilder()
                    .AddExpiry(Convert.ToInt32(_configuration["RefreshTokenConstants:ExpiryInMinutes"]))
                    .AddClaims(myClaims)
                    .Build();
                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new RefreshToken()
                {
                    AccessToken = accessToken,
                    ExpiresIn = Convert.ToInt32(_configuration["RefreshTokenConstants:ExpiryInMinutes"])
                };


                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GenerateOtp(int length = 6)
        {
            if (length < 4 || length > 10) throw new ArgumentException("OTP length must be between 4 and 10.");

            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            var otp = new StringBuilder();
            foreach (byte b in randomBytes)
            {
                otp.Append((b % 10).ToString());
            }

            return otp.ToString();
        }
    }
}
