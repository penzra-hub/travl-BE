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
    public class GetSingleDriverForAdminQueryHandler : IRequestHandler<GetSingleDriverForAdminQuery, IResult<SingleDriverDetailForAdminDto>>
    {
        private readonly IDriverRepository _driverRepository;
        public GetSingleDriverForAdminQueryHandler(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }
        public async Task<IResult<SingleDriverDetailForAdminDto>> Handle(GetSingleDriverForAdminQuery request, CancellationToken cancellationToken)
        {
            return await _driverRepository.GetDriverForAdminAsync(request.DriverId);
        }
    }
}
