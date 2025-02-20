using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;
using Travl.Domain.Entities;
using Travl.Infrastructure.Utility;

namespace Travl.Infrastructure.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly AppSettings _appSettings;
        private IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;


        public EmailService(MailSettings mailSettings, AppSettings appSettings, IWebHostEnvironment env, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _mailSettings = mailSettings;
            _appSettings = appSettings;
            _appSettings.WebRootPath = env.WebRootPath;
            _configuration = configuration;
            _userManager = userManager;
        }

        private async Task<AppUser?> InitializeUserAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        // Send a single email
        public async Task<bool> SendEmail(EmailVm emailVm)
        {

            var email = new MimeMessage();
            var senderEmail = _mailSettings.From;
            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.To.Add(MailboxAddress.Parse(emailVm.ToEmail));
            email.Subject = emailVm.Subject;

            var builder = new BodyBuilder();

            // Attach Images as Linked Resources
            //var logoPath = Path.Combine(_appSettings.WebRootPath, "assets", "travl-logo.png");
            var instagramPath = Path.Combine(_appSettings.WebRootPath, "assets", "square-instagram-brands-solid.png");
            var linkedinPath = Path.Combine(_appSettings.WebRootPath, "assets", "linkedin-brands-solid.png");
            var facebookPath = Path.Combine(_appSettings.WebRootPath, "assets", "facebook-brands-solid.png");
            var twitterPath = Path.Combine(_appSettings.WebRootPath, "assets", "twitter-brands-solid.png");

            //var logo = builder.LinkedResources.Add(logoPath);
            //logo.ContentId = "travl-logo";

            var instagramIcon = builder.LinkedResources.Add(instagramPath);
            instagramIcon.ContentId = "instagram-icon";

            var linkedinIcon = builder.LinkedResources.Add(linkedinPath);
            linkedinIcon.ContentId = "linkedin-icon";

            var facebookIcon = builder.LinkedResources.Add(facebookPath);
            facebookIcon.ContentId = "facebook-icon";

            var twitterIcon = builder.LinkedResources.Add(twitterPath);
            twitterIcon.ContentId = "twitter-icon";

            if (emailVm.Attachments != null)
            {
                foreach (var file in emailVm.Attachments.Where(file => file.Length > 0))
                {
                    byte[] fileBytes;
                    await using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }

                    builder.Attachments.Add($"{file.FileName}-{Guid.NewGuid()}", fileBytes, ContentType.Parse(file.ContentType));
                }
            }

            builder.HtmlBody = emailVm.Body;
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                var host = _configuration["MailSettings:Host"];
                var port = int.Parse(_configuration["MailSettings:Port"]!);
                await smtp.ConnectAsync(host, port, SecureSocketOptions.Auto);
                //await smtp.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
                //await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_configuration["MailSettings:UserName"], _configuration["MailSettings:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Send multiple emails
        public async Task<bool> SendEmailAsync(List<EmailVm> emailVms)
        {
            try
            {
                foreach (var emailVm in emailVms)
                {
                    bool isSent = await SendEmail(emailVm);
                    if (!isSent)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Convert file to Base64 string 
        private static string ConvertFile(IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            return Convert.ToBase64String(fileBytes);
        }

        // Existing methods refactored to use EmailTemplate

        // 1. SendContactUsConfirm
        public async Task<bool> SendContactUsConfirm(string sendTo, string firstName, string lastName, string cusEmail, string userNumber, string message)
        {
            try
            {
                // Email to Lengoal Support
                var internalEmailBody = EmailTemplate.ContactUsInternalTemplate(firstName + " " + lastName, cusEmail, userNumber, message);
                var internalEmail = new EmailVm
                {
                    ToEmail = sendTo,
                    Body = internalEmailBody,
                    Subject = "New Contact Us Email"
                };

                // Email to Customer
                var userEmailBody = EmailTemplate.ContactUsUserTemplate(firstName);
                var userEmail = new EmailVm
                {
                    ToEmail = cusEmail,
                    Body = userEmailBody,
                    Subject = "Contact Us Inquiry"
                };

                var emails = new List<EmailVm> { internalEmail, userEmail };

                await SendEmailAsync(emails);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 2. SendOTPMail (Refactored to use EmailTemplate)
        public async Task<bool> SendOTPMail(string cusEmail, string cusFirstName, string otp, string expirationTime)
        {
            try
            {
                var emailBody = EmailTemplate.EmailVerificationTemplate(cusFirstName, otp, expirationTime);
                var email = new EmailVm
                {
                    ToEmail = cusEmail,
                    Body = emailBody,
                    Subject = "Complete your email verification for Travl"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 3. SendPasswordResetEmailAsync (Implemented previously)
        public async Task<bool> SendPasswordResetEmailAsync(string cusEmail, string cusFirstName, string token, string expiryDuration)
        {
            try
            {
                var user = InitializeUserAsync(cusEmail);
                string resetLink = $"{_appSettings.WebUrl}/reset-password?email={cusEmail}";
                var emailBody = EmailTemplate.PasswordResetTemplate(cusFirstName, resetLink, expiryDuration);
                var email = new EmailVm
                {
                    ToEmail = cusEmail,
                    Body = emailBody,
                    Subject = "Reset Your Password for Travl"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 4. SendPasswordResetSuccess (Implementing the newly added method)
        public async Task<bool> SendPasswordResetSuccess(string cusEmail, string cusFirstName)
        {
            try
            {
                var supportEmail = _mailSettings.Mail ?? "info@travl.co";
                var emailBody = EmailTemplate.PasswordResetSuccessTemplate(cusFirstName, supportEmail);
                var email = new EmailVm
                {
                    ToEmail = cusEmail,
                    Body = emailBody,
                    Subject = "Your Password Has Been Successfully Reset"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 5. SendEmailVerificationAsync
        public async Task<bool> SendEmailVerificationAsync(string toEmail, string cusFirstName, string otp, string expiryDuration)
        {
            try
            {
                var emailBody = EmailTemplate.EmailVerificationTemplate(cusFirstName, otp, expiryDuration);
                var email = new EmailVm
                {
                    ToEmail = toEmail,
                    Body = emailBody,
                    Subject = "Complete your email verification for Travl"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 6. SendWelcomeEmailAsync
        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string cusFirstName)
        {
            try
            {
                var emailBody = EmailTemplate.WelcomeTemplate(cusFirstName);
                var email = new EmailVm
                {
                    ToEmail = toEmail,
                    Body = emailBody,
                    Subject = "Welcome to Travl - Your Journey Starts Here!"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // 7. SendOtpVerificationEmailAsync
        public async Task<bool> SendOtpVerificationEmailAsync(string toEmail, string cusFirstName, string otp, string expiryDuration)
        {
            try
            {
                var emailBody = EmailTemplate.OtpVerificationTemplate(cusFirstName, otp, expiryDuration);
                var email = new EmailVm
                {
                    ToEmail = toEmail,
                    Body = emailBody,
                    Subject = "Verify your account with Travl"
                };
                return await SendEmail(email);
            } 
            catch (Exception ex)
            {
                return false;
            }
        }

        // 8. SendContactUsInternalEmailAsync
        public async Task<bool> SendContactUsInternalEmailAsync(string toEmail, string userName, string userEmail, string userNumber, string userMessage)
        {
            try
            {
                var emailBody = EmailTemplate.ContactUsInternalTemplate(userName, userEmail, userNumber, userMessage);
                var email = new EmailVm
                {
                    ToEmail = toEmail,
                    Body = emailBody,
                    Subject = "Contact Us Enquiry"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 9. SendContactUsUserEmailAsync
        public async Task<bool> SendContactUsUserEmailAsync(string toEmail, string userName)
        {
            try
            {
                var emailBody = EmailTemplate.ContactUsUserTemplate(userName);
                var email = new EmailVm
                {
                    ToEmail = toEmail,
                    Body = emailBody,
                    Subject = "Contact Us Inquiry"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // 10. SendNewStaffPasswordCreationEmailAsync
        public async Task<bool> SendNewStaffPasswordCreationEmailAsync(string toEmail, string staffName, string createPasswordLink)
        {
            try
            {
                var emailBody = EmailTemplate.NewStaffPasswordCreationTemplate(staffName, createPasswordLink);
                var email = new EmailVm
                {
                    ToEmail = toEmail,
                    Body = emailBody,
                    Subject = "Admin Creates a New Staff"
                };
                return await SendEmail(email);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
