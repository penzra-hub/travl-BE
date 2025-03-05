using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        [Required] 
        public string Model { get; set; }
        [Required] public string LicensePlateNo { get; set; }
        [Required] public string Color { get; set; }
        [Required] public string EngineNumber { get; set; }
        [Required] public IEnumerable<IFormFile> VehicleDocumentUrl { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Year must be in YYYY-MM-DD format.")]
        public string Year { get; set; }
    }
}
