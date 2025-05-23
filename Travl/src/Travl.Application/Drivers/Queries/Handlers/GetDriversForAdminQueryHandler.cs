using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.IRepositories;

namespace Travl.Application.Drivers.Queries.Handlers
{
    public class GetDriversForAdminQueryHandler : IRequestHandler<GetDriversForAdminQuery, IResult<IEnumerable<DriverDetailDto>>>
    {
        private readonly IDriverRepository _driverRepository;
        public GetDriversForAdminQueryHandler (IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }
        public async Task<IResult<IEnumerable<DriverDetailDto>>> Handle(GetDriversForAdminQuery request, CancellationToken cancellationToken)
        {
            var drivers = await _driverRepository.GetAllDriversAsync();

            if (drivers == null)
            {
                return Result<IEnumerable<DriverDetailDto>>.Success(drivers, "No drivers available");
            }
            return Result<IEnumerable<DriverDetailDto>>.Success(drivers, "Drivers retrieved successfully.");
        }
    }
}
