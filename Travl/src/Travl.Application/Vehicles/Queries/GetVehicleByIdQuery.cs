using AspNetCoreHero.Results;
using MediatR;
using Travl.Application.Dtos.DriverDto;

namespace Travl.Application.Vehicles.Queries;

public class GetVehicleByIdQuery : IRequest<IResult<GetVehicleDto>>
{
    public string Id { get; set; }

    public GetVehicleByIdQuery(string id)
    {
        Id = id;
    }
}