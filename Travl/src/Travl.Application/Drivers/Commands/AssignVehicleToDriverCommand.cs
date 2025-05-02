using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

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
