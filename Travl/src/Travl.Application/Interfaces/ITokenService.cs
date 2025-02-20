using Travl.Domain.Commons;
using Travl.Domain.Entities;

namespace Travl.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateUserToken(AppUser user);
        Task<AuthToken> GenerateAccessToken(AppUser user);

        Task<LoginDetails> DecodeRefreshToken(string refreshToken);
        Task<RefreshToken> GenerateRefreshToken(string userName, string password);

        string GenerateOtp(int length = 6);
    }
}
