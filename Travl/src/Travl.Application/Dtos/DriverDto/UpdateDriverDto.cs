using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Enums;

namespace Travl.Application.Dtos.DriverDto
{
    public class UpdateDriverDto
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }    
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UpdatedAt { get; set; }
        public IFormFile? Avatar { get; set; }
                      
    }
}
