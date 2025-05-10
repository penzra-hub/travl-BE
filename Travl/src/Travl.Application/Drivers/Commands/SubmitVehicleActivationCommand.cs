using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Travl.Application.Drivers.Commands
{
    public class SubmitVehicleActivationCommand : IRequest<IResult<string>>
    {
       
        public string Model { get; set; }
        public string LicensePlateNo { get; set; }
        public string Color { get; set; }
        public string EngineNumber { get; set; }
        public IEnumerable<IFormFile> VehicleDocuments { get; set; }
        public string Year { get; set; }
    }
}
