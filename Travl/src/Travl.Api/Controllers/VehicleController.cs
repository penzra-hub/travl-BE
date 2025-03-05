using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travl.Application.Drivers.Commands;
using Travl.Application.Drivers.Models;

namespace Travl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ApiController
    {
        [HttpPost("driver/add-vehicle")]
        public async Task<IActionResult> AssignVehicle([FromForm] AssignVehicleToDriverCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
        
        [HttpPut("driver/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(string vehicleId, [FromForm] UpdateVehicleDto vehicleDto)
        {

            var command = new UpdateDriverVehicleCommand(vehicleId, vehicleDto);
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
