using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Application.Drivers.Commands.CommandValidators
{
    public class SubmitVehicleActivationValidator : AbstractValidator<SubmitVehicleActivationCommand>
    {
        public SubmitVehicleActivationValidator()
        {
            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required")
                .MaximumLength(50).WithMessage("Model should not exceed 50 characters");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required")
                .MaximumLength(30).WithMessage("Color should not exceed 30 characters")
                .Matches(@"^[A-Za-z\s]+$").WithMessage("Color should only contain letters");

            RuleFor(x => x.EngineNumber)
                .NotEmpty().WithMessage("Engine Number is required")
                .Length(6, 20).WithMessage("Engine Number must be between 6 and 20 characters")
                .Matches(@"^[A-Z0-9\-]+$").WithMessage("Engine Number must be alphanumeric, numbers or upper case alphebets");

            RuleFor(x => x.LicensePlateNo)
                .NotEmpty().WithMessage("License Plate Number is required")
                .MaximumLength(15).WithMessage("License Plate Number must not exceed 15 characters")
                .Matches(@"^[A-Z0-9]{3,5}[-]?[A-Z0-9]{3,5}$").WithMessage("License Plate Number must be alphanumeric");

            RuleFor(x => x.VehicleDocuments)
                .NotNull().WithMessage("Vehicle document is required")
                .Must(docs => docs.Any() && docs.All(f => f.Length > 0)).WithMessage("Each document must be a valid file")
                .Must(docs => docs.All(f => f.ContentType.StartsWith("image/") || f.ContentType == "application/pdf"))
                .WithMessage("Only image or PDF files are allowed for vehicle documents");

            RuleFor(x => x.Year)
                .NotEmpty().WithMessage("Year is required")
                .Must(BeAValidDate).WithMessage("Year must be a valid date in YYYY-MM-DD")
                .Must(BeWithinValidRange).WithMessage($"Year must be between 1886 and {DateTime.UtcNow.Year + 1}");
        }

        private bool BeAValidDate(string date)
        {
            return DateOnly.TryParse(date, out _);
        }

        private bool BeWithinValidRange(string date)
        {
            if (!DateOnly.TryParse(date, out var parsed)) return false;
            var year = parsed.Year;
            return year >= 1886 && year <= DateTime.UtcNow.Year + 1;
        }
       
    }

}
