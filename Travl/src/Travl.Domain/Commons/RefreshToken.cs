namespace Travl.Domain.Commons
{
    public class RefreshToken
    {
        public long ExpiresIn { get; set; }
        public string AccessToken { get; set; }
        public string NewRefreshToken { get; set; }
    }
}
