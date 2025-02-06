namespace Travl.Application.Authentication.Models
{
    public class AuthToken
    {
        public string UserId { get; set; }
        public string UserToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
