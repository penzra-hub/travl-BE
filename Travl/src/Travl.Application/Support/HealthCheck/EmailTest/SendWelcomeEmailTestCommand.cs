using AspNetCoreHero.Results;
using MediatR;

namespace Travl.Application.Support.HealthCheck.EmailTest
{
    public class SendWelcomeEmailTestCommand : IRequest<IResult<string>>
    {
        public required string ToEmail { get; set; }
        public required string FirstName { get; set; }
    }
}
