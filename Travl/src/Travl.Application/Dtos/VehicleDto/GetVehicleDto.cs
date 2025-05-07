namespace Travl.Application.Dtos.DriverDto;

public class GetVehicleDto
{
    public string Id { get; set; }
    public string Model { get; set; }
    public string LicensePlateNo { get; set; }
    public DateOnly? Year { get; set; }
    public string Color { get; set; }
    public string EngineNumber { get; set; }
    public ICollection<string> VehicleDocumentUrl { get; set; }
    
    // Driver info
    public string DriverId { get; set; }
    public string DriverName { get; set; }
}