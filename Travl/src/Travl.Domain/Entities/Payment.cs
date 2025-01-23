using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
