using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Travl.Domain.Enums;
using IResult = AspNetCoreHero.Results.IResult;

namespace Travl.Application.Drivers.Commands
{
    public class SubmitDriverVerificationCommand : IRequest<IResult>
    {
        public string LicenseNumber { get; set; }
        public IFormFile LicensePhoto { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
