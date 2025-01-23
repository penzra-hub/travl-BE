using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class Passenger : BaseEntity
    {
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? PreferredPaymentMethod { get; set; }
        public bool HasActiveSubscription { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public ICollection<UserVerification>? UserVerification { get; set; }
    }
}
