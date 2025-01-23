using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class UserVerification : BaseEntity
    {
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public IdentificationType IdentificationType { get; set; }
        public string? IdentificationUrl { get; set; }
        public string? IdentificationNo { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseUrl { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
