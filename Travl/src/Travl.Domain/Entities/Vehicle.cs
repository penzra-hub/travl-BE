using Travl.Domain.Commons;

namespace Travl.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public string? DriverId { get; set; }
        public virtual Driver? Driver { get; set; }
        public string? Model { get; set; }
        public string? LicensePlateNo { get; set; }
        public DateOnly? Year { get; set; }
        public string? Color { get; set; }
        public string? EngineNumber { get; set; }
        public ICollection<string>? VehicleDocumentUrl { get; set; }

    }
}
