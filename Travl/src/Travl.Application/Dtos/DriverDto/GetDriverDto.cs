using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Dtos.DriverDto
{
    public class GetDriverDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string VerificationStatus { get; set; }
        public string VehicleInfo { get; set; }
    }
}
