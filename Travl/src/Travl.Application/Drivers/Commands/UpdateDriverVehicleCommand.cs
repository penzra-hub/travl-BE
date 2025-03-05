using MediatR;
using Microsoft.AspNetCore.Http;
using IResult = AspNetCoreHero.Results.IResult;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Drivers.Models;

namespace Travl.Application.Drivers.Commands
{
    public class UpdateDriverVehicleCommand : IRequest<IResult>
    {
        public string VehicleId { get; set; }
        public UpdateVehicleDto VehicleDto { get; set; }

        public UpdateDriverVehicleCommand(string vehicleId, UpdateVehicleDto vehicleDto)
        {
            VehicleId = vehicleId;
            VehicleDto = vehicleDto;
        }
    }
}
