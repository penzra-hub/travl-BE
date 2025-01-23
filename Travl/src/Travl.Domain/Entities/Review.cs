using Travl.Domain.Commons;

namespace Travl.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string? RideId { get; set; }
        public Ride? Ride { get; set; }
        public string? RatedById { get; set; }
        public AppUser? RatedBy { get; set; }
        public string? RatedForId { get; set; }
        public AppUser? RatedFor { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
