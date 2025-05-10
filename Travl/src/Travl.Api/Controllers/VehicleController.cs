using Microsoft.AspNetCore.Mvc;
using Travl.Application.Drivers.Commands;
using Travl.Application.Drivers.Models;

namespace Travl.Api.Controllers
{
    public class VehicleController : ApiController
    {
        
        [HttpPut("UpdateVehicle/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(string vehicleId, [FromForm] UpdateVehicleDto vehicleDto)
        {
            var command = new UpdateDriverVehicleCommand(vehicleId, vehicleDto);
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
