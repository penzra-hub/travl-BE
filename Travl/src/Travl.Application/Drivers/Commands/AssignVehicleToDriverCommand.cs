using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Commands
{
    public class AssignVehicleToDriverCommand : IRequest<IResult<string>>
    {
        [Required] public string Model { get; set; }
        [Required] public string LicensePlateNo { get; set; }
        [Required] public string Color { get; set; }
        [Required] public string EngineNumber { get; set; }
        [Required] public ICollection<string> VehicleDocumentUrl { get; set; }

        //[Required] public DateOnly Year { get; set; }
    }
}
