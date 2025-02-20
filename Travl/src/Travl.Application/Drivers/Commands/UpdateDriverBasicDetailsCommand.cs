using AspNetCoreHero.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = AspNetCoreHero.Results.IResult;

namespace Travl.Application.Drivers.Commands
{
    public class UpdateDriverBasicDetailsCommand : IRequest<IResult>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UpdatedAt { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
