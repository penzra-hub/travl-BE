using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Enums;

namespace Travl.Application.Dtos.DriverDto
{
    public class DriverDetailDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public List<VehicleDto> Vehicles { get; set; }     
        public List<UserVerificationDto> KycDetails { get; set; }
        
    }
      
}
