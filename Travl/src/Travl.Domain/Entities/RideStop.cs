using Travl.Domain.Commons;

namespace Travl.Domain.Entities
{
    public class RideStop : BaseEntity
    {
        public string? RideId { get; set; }
        public Ride? Ride { get; set; }
        public string? Location { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
    }
}
