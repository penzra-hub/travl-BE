using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Name => FirstName + " " + LastName;
        public Gender Gender { get; set; }
        public bool IsActive { get; set; }
        public string? PublicId { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Status Status { get; set; } = Status.Active;
        public string StatusDesc => Status.ToString();
        public DateTime LastLoginDate { get; set; }
        [MaxLength(2048)]
        public string? Token { get; set; }
        public bool IsTokenValid { get; set; }
        public AccessLevel AccessLevel { get; set; } = AccessLevel.User;
        public string AccessLevelDesc => AccessLevel.ToString();
        public string? Otp { get; set; }
        public DateTime OtpExpiration { get; set; }
        public UserType UserType { get; set; }
        public int LoginCount { get; set; } = 0;

    }
}
