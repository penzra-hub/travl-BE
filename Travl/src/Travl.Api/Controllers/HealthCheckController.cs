using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travl.Application.Support.HealthCheck.EmailTest;

namespace Travl.Api.Controllers
{
    [AllowAnonymous]
    public class HealthCheckController : ApiController
    {
        [HttpPost("WelcomeEmailTest")]
        public async Task<IActionResult> SendWelcomeEmailTest(SendWelcomeEmailTestCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
