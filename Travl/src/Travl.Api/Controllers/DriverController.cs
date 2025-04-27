using Microsoft.AspNetCore.Mvc;
using Travl.Application.Drivers.Commands;
using Travl.Application.Drivers.Queries;

namespace Travl.Api.Controllers
{
    public class DriverController : ApiController
    {

        [HttpGet("GetDriverById/{driverId}")]
        public async Task<IActionResult> GetDriverById(string driverId)
        {
            var query = new GetDriverForPassengerQuery()
            {
                driverId = driverId
            };
            return await Initiate(() => Mediator.Send(query));

        }


        [HttpPut("update-driver")]
        public async Task<IActionResult> UpdateDriver([FromForm] UpdateDriverBasicDetailsCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }

        [HttpPost("CompleteDriverProfile")]
        public async Task<IActionResult> SubmitVerification([FromForm] SubmitDriverVerificationCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
