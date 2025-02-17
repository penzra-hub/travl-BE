using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travl.Application.Drivers.Commands;

namespace Travl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ApiController
    {
        [HttpPost("add-vehicle")]
        public async Task<IActionResult> AssignVehicle([FromBody] AssignVehicleToDriverCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
        
        [HttpPut("update-vehicle")]
        public async Task<IActionResult> UpdateVehicle([FromBody] UpdateDriverVehicleCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
