using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string? RideId { get; set; }
        public Ride? Ride { get; set; }
        public string? PassengerId { get; set; }
        public Passenger? Passenger { get; set; }
        public int SeatsBooked { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public string? PaymentId { get; set; }
        public Payment? Payment { get; set; }
    }
}