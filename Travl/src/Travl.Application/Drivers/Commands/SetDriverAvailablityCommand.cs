using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Drivers.Commands
{
    public class SetDriverAvailablityCommand : IRequest<IResult>
    {
        public bool IsAvailable { get; set; }
    }
}
