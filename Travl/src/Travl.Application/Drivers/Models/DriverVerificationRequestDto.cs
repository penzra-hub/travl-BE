using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Models
{
    public class DriverVerificationRequestDto
    {
        [Required]
        public string AppUserId { get; set; } = null!;
        public IdentificationType IdentificationType { get; set; }
        public string IdentificationUrl { get; set; } = null!;
        public string? IdentificationNo { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseUrl { get; set; }
        public DateTime? ExpiryDate { get; set; }
        //public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    }
}
