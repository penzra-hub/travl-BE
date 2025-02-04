using Travl.Domain.Commons;

namespace Travl.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailVm email);

        Task<bool> SendContactUsConfirm(string sendTo, string firstName, string lastName, string cusEmail,
            string userNumber, string message);

        Task<bool> SendEmailAsync(List<EmailVm> emailVms);
        Task<bool> SendOTPMail(string cusEmail, string cusFirstName, string otp, string expirationTime);
        Task<bool> SendEmailVerificationAsync(string toEmail, string cusFirstName, string otp, string expiryDuration);
        Task<bool> SendWelcomeEmailAsync(string cusEmail, string cusFirstName);
        Task<bool> SendPasswordResetEmailAsync(string cusEmail, string cusFirstName, string token, string expiryDuration);
        Task<bool> SendPasswordResetSuccess(string cusEmail, string cusFirstName);
        Task<bool> SendOtpVerificationEmailAsync(string toEmail, string cusFirstName, string otp, string expiryDuration);
        Task<bool> SendContactUsInternalEmailAsync(string toEmail, string userName, string userEmail, string userNumber, string userMessage);
        Task<bool> SendContactUsUserEmailAsync(string toEmail, string userName);
        Task<bool> SendNewStaffPasswordCreationEmailAsync(string toEmail, string staffName, string createPasswordLink);
    }
}
