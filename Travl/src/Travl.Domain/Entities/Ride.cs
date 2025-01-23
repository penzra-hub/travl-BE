using Travl.Domain.Commons;
using Travl.Domain.Enums;

namespace Travl.Domain.Entities
{
    public class Ride : BaseEntity
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerSeat { get; set; }
        public RideType RideType { get; set; } = RideType.Shared;
        public string? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}