namespace Travl.Application.Interfaces
{
    public interface IUserValidationService
    {
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
        Task<bool> IsTokenValidAsync(string token);
    }
}
