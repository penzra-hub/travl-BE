using Microsoft.AspNetCore.Mvc;
using Travl.Application.Drivers.Commands;
using Travl.Application.Drivers.Models;
using Travl.Application.Vehicles.Queries;

namespace Travl.Api.Controllers
{
    public class VehicleController : ApiController
    {
        [HttpPost("add-vehicle")]
        public async Task<IActionResult> AssignVehicle([FromForm] AssignVehicleToDriverCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }

        [HttpPut("UpdateVehicle/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(string vehicleId, [FromForm] UpdateVehicleDto vehicleDto)
        {
            var command = new UpdateDriverVehicleCommand(vehicleId, vehicleDto);
            return await Initiate(() => Mediator.Send(command));
        }
        
        [HttpGet("GetVehicleById/{vehicleId}")]
        public async Task<IActionResult> GetVehicleById(string vehicleId)
        {
            var query = new GetVehicleByIdQuery(vehicleId);
            return await Initiate(() => Mediator.Send(query));
        }
    }
}
