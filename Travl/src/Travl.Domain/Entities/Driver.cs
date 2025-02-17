using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class Driver : BaseEntity
    {
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public virtual ICollection<Vehicle>? Vehicles { get; set; }
        public VerificationStatus VerificationStatus {get; set;}
        public DateTime VerificationDate { get; set; }
        public ICollection<Ride>? Rides { get; set; }
        public ICollection<UserVerification>? UserVerification { get; set; }
    }
}
