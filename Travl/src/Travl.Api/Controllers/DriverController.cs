using Microsoft.AspNetCore.Authorization;
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

        
        [HttpGet]
        public async Task<IActionResult> GetAllDrivers()
        {
            var query = new GetDriversForAdminQuery();

            return await Initiate(() => Mediator.Send(query));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("get-single-driver/{driverId}")]
        public async Task<IActionResult> GetSingleDriver(string driverId)
        {
            var query = new GetSingleDriverForAdminQuery(driverId);
            return await Initiate(() => Mediator.Send(query));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("deactivate-driver/{driverId}")]
        public async Task<IActionResult> DeactivateDriver(string driverId)
        {
            var command = new DeactivateDriverCommand { DriverId = driverId };
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
