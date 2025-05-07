using AspNetCoreHero.Results;
using MediatR;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.IRepositories;

namespace Travl.Application.Vehicles.Queries.Handlers;

public class GetVehicleByIdQueryHandler : IRequestHandler<GetVehicleByIdQuery, IResult<GetVehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleByIdQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IResult<GetVehicleDto>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetVehicleWithDriverByIdAsync(request.Id);
    }
}