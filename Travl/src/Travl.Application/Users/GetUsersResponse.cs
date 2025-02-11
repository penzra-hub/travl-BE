using AutoMapper;
using Travl.Application.Common.Mappings;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Users
{
    public class GetUsersResponse : IMapFrom<AppUser>
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public bool IsActive { get; set; }
        public string? PublicId { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Password { get; set; }
        public Status Status { get; set; } = Status.Active;
        public string StatusDesc => Status.ToString();
        public DateTime LastLoginDate { get; set; }
        public string? Token { get; set; }
        public bool IsTokenValid { get; set; }
        public AccessLevel AccessLevel { get; set; } = AccessLevel.User;
        public string AccessLevelDesc => AccessLevel.ToString();
        public string? Otp { get; set; }
        public string? Email { get; internal set; }
        public string? UserName { get; internal set; }
        public string? PhoneNumber { get; internal set; }
        public UserType UserType { get; set; }
        public int LoginCount { get; set; }
        public string? Role { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AppUser, GetUsersResponse>().ReverseMap();
        }
    }
}