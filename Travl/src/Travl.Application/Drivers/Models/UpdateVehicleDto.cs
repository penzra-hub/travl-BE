using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Drivers.Models
{
    public class UpdateVehicleDto
    {
        public string? Model { get; set; }
        public string? LicensePlateNo { get; set; }
        public string? Color { get; set; }
        public string? EngineNumber { get; set; }
        public List<IFormFile>? VehicleDocumentUrl { get; set; }

        public string? Year { get; set; }
    }
}
