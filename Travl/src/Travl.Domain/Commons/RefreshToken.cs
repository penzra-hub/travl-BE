namespace Travl.Domain.Commons
{
    public class RefreshToken
    {
        public long ExpiresIn { get; set; }

        public string RefreshAccessToken { get; set; }
    }
}
