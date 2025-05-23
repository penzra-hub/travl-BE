using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Dtos.DriverDto;

namespace Travl.Application.Drivers.Queries
{
    public class GetSingleDriverForAdminQuery : IRequest<IResult<SingleDriverDetailForAdminDto>>
    {
        public string DriverId { get; set; }
        public GetSingleDriverForAdminQuery(string driverId)
        {
            DriverId = driverId;
        }
    }
}
