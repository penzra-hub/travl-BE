using AspNetCoreHero.Results;
using Microsoft.EntityFrameworkCore;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.IRepositories;
using Travl.Domain.Context;
using Travl.Domain.Entities;

namespace Travl.Infrastructure.Repositories;

public class VehicleRepository : RepositoryBase<Vehicle>, IVehicleRepository
{
    private readonly ApplicationContext _context;

    public VehicleRepository(ApplicationContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IResult<GetVehicleDto>> GetVehicleWithDriverByIdAsync(string id)
    {
        var vehicle = await _context.Vehicles
            .Where(v => v.Id == id)
            .Include(v => v.Driver)  // Assuming you need to include Driver details
            .Select(v => new GetVehicleDto
            {
                Id = v.Id,
                Model = v.Model,
                LicensePlateNo = v.LicensePlateNo,
                Year = v.Year,
                Color = v.Color,
                EngineNumber = v.EngineNumber,
                VehicleDocumentUrl = v.VehicleDocumentUrl,
                DriverId = v.Driver.Id,
                DriverName = v.Driver.AppUser.Name // Assuming the driver's name is stored in `AppUser`
            })
            .FirstOrDefaultAsync();

        if (vehicle == null)
        {
            return await Result<GetVehicleDto>.FailAsync("Vehicle not found");
        }

        return await Result<GetVehicleDto>.SuccessAsync(vehicle);
    }
}