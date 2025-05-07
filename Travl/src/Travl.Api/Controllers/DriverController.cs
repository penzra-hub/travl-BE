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
                DriverId = driverId
            };
            return await Initiate(() => Mediator.Send(query));

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateDriver([FromForm] UpdateDriverBasicDetailsCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }

        [HttpPost("submit-activation-request")]
        public async Task<IActionResult> SubmitVerification([FromForm] SubmitDriverVerificationCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }

        [HttpPost("vehicle/submit-activation-request")]
        public async Task<IActionResult> AssignVehicle([FromForm] SubmitVehicleActivationCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
