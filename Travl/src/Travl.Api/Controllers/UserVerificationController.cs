using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travl.Application.Drivers.Commands;

namespace Travl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVerificationController : ApiController
    {
        [HttpPost("driver-kyc/submit")]
        public async Task<IActionResult> SubmitVerification([FromForm] SubmitDriverVerificationCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
