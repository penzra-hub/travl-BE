using System.IdentityModel.Tokens.Jwt;

namespace Travl.Infrastructure.Utility
{
    public static class ExtensionMethods
    {
        public static JwtSecurityToken ExtractToken(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("Authorization header is null or empty");
            }

            var stream = str.Remove(0, 7);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var token = jsonToken as JwtSecurityToken;
            return token;
        }

        public static bool ValidateToken(this JwtSecurityToken accessToken, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("Invalid User Id");
            }

            if (accessToken == null)
            {
                throw new ArgumentException("Invalid Token Credentials");
            }

            var idClaim = accessToken.Claims.FirstOrDefault(claim => claim.Type == "Id");
            if (idClaim == null)
            {
                throw new ArgumentException($"Invalid Token Credentials. Claim with type 'Id' not found. Available claims: {string.Join(", ", accessToken.Claims.Select(c => c.Type))}");
            }

            if (userId != idClaim.Value)
            {
                throw new ArgumentException("Invalid Token Credentials. UserId does not match the token.");
            }

            return true;
        }
    }
}
