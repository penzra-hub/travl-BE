using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.IRepositories;

namespace Travl.Application.Drivers.Queries.Handlers
{
    public class GetDriverForAdminQueryHandler : IRequestHandler<GetDriversForAdminQuery, IResult<IEnumerable<DriverDetailDto>>>
    {
        private readonly IDriverRepository _driverRepository;

        public GetDriverForAdminQueryHandler(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        public async Task<IResult<IEnumerable<DriverDetailDto>>> Handle(GetDriversForAdminQuery request, CancellationToken cancellationToken)
        {

            var drivers = await _driverRepository.GetAllDriversAsync();

            if (drivers == null)
            {
                return Result<IEnumerable<DriverDetailDto>>.Success(drivers, "No Drivers Available");
            }
            return Result<IEnumerable<DriverDetailDto>>.Success(drivers, "Drivers Retrieved successfully");
        }
    }
}
