using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Travl.Domain.Enums;
using IResult = AspNetCoreHero.Results.IResult;

namespace Travl.Application.Drivers.Commands
{
    public class SubmitDriverVerificationCommand : IRequest<IResult>
    {
        [Required(ErrorMessage = "Identification type is required")]
        public IdentificationType IdentificationType { get; set; }

        [Required(ErrorMessage = "Document Image is required")]
        public IFormFile? DocumentImage { get; set; } = null;
        public string? IdentificationNo { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseUrl { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
