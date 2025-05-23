using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Enums;

namespace Travl.Application.Dtos.DriverDto
{
    public class SingleDriverDetailForAdminDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DateTime VerificationDate { get; set; }
        public Status Status { get; set; }
        public List<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();
        public List<UserVerificationDto> KycDetails { get; set; } = new List<UserVerificationDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
