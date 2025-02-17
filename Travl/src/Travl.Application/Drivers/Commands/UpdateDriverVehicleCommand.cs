using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Drivers.Commands
{
    public class UpdateDriverVehicleCommand : IRequest<IResult>
    {
        public string VehicleId { get; set; }
        public string? Model { get; set; }
         public string? LicensePlateNo { get; set; }
        public string? Color { get; set; }
        public string? EngineNumber { get; set; }
        public ICollection<string>? VehicleDocumentUrl { get; set; }

        //[Required] public DateOnly Year { get; set; }
    }
}
