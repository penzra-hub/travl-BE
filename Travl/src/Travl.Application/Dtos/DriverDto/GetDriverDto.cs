using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Enums;

namespace Travl.Application.Dtos.DriverDto
{
    public class GetDriverDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string VerificationStatus { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public DateOnly? Year { get; set; }
        public string PlateNumber { get; set; }
    }
}
