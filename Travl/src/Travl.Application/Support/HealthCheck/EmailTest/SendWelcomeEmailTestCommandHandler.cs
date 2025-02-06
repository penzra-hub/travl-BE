using AspNetCoreHero.Results;
using MediatR;
using Travl.Application.Interfaces;

namespace Travl.Application.Support.HealthCheck.EmailTest
{
    public class SendWelcomeEmailTestCommandHandler : IRequestHandler<SendWelcomeEmailTestCommand, IResult<string>>
    {
        private readonly IEmailService _emailService;

        public SendWelcomeEmailTestCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IResult<string>> Handle(SendWelcomeEmailTestCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail) || string.IsNullOrWhiteSpace(request.FirstName))
            {
                return Result<string>.Fail("Recipient email and first name are required.");
            }

            try
            {

                // Send the email using the EmailService
                bool isSent = await _emailService.SendWelcomeEmailAsync(request.ToEmail, request.FirstName);

                if (isSent)
                {
                    return Result<string>.Success("Welcome email sent successfully.");
                }
                else
                {
                    return Result<string>.Fail("Failed to send welcome email.");
                }
            }
            catch (Exception ex)
            {
                return Result<string>.Fail("An error occurred while sending the welcome email.");
            }
        }
    }
}
