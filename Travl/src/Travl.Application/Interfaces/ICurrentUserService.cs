namespace Travl.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserRole { get; }
        string? UserEmail { get; }
        string? UserPhoneNumber { get; set; }
        string FullName { get; }
        string? UserType { get; set; }
    }
}
