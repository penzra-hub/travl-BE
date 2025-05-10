using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Domain.Entities;

namespace Travl.Application.Drivers.Queries
{
    public class GetAssignedTripsQuery : IRequest<IResult<List<Ride>>>
    {
    }
}
