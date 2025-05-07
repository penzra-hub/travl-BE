using AspNetCoreHero.Results;
using Travl.Application.Dtos.DriverDto;
using Travl.Domain.Entities;

namespace Travl.Application.IRepositories;

public interface IVehicleRepository : IRepositoryBase<Vehicle>
{
    Task<IResult<GetVehicleDto>> GetVehicleWithDriverByIdAsync(string id);

}