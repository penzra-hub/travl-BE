using AspNetCoreHero.Results;
using AutoMapper;
using Azure.Core;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Drivers.Commands.CommandValidators;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Commons;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class SubmitVehicleActivationCommandHandler : IRequestHandler<SubmitVehicleActivationCommand, IResult<string>>
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IRepositoryBase<Vehicle> _repository;          
        private readonly ICurrentUserService _currentUser;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IEmailService _emailService;
        private readonly IValidator<SubmitVehicleActivationCommand> _validator;



        public SubmitVehicleActivationCommandHandler(IDriverRepository driverRepository, IRepositoryBase<Vehicle> repository, ICurrentUserService currentUser, ICloudinaryService cloudinaryService, IEmailService emailService, IValidator<SubmitVehicleActivationCommand> validator)
        {
            _driverRepository = driverRepository;
            _repository = repository;
            _currentUser = currentUser;
            _cloudinaryService = cloudinaryService;
            _emailService = emailService;
            _validator = validator;
        }

        public async Task<IResult<string>> Handle(SubmitVehicleActivationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<string>.Fail("Unauthorized. User ID not found.");
            }

            var driverResult = await _driverRepository.GetDriverByAppUserId(userId);

            if (!driverResult.Succeeded) 
                return Result<string>.Fail("Driver not found.");

            var driver = driverResult.Data;

            if (driver.Status != Status.Active)
                return Result<string>.Fail("Driver is not activated yet");

            // Upload vehicle images to cloudinary
            var vehicleDocuments = new List<string>();

            foreach (var doc in request.VehicleDocuments)
            {
                var uploadResult = await _cloudinaryService.AddPhotoAsync(doc);

                if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Uri.ToString()))
                {
                    return Result<string>.Fail("An error occured while trying to upload the Document image");
                }

                vehicleDocuments.Add(uploadResult.ToString());
            }


            var vehicle = new Vehicle()
            {
                Id = Guid.NewGuid().ToString(),
                Status = Status.Pending,
                DriverId = driver.Id,
                Model = request.Model,
                LicensePlateNo = request.LicensePlateNo,
                Color = request.Color,
                Year = DateOnly.Parse(request.Year,  CultureInfo.InvariantCulture),
                EngineNumber = request.EngineNumber,
                CreatedBy = driver?.AppUser?.Name,
                VehicleDocumentUrl = vehicleDocuments
            };

            var result = await _repository.AddAsync(vehicle);

            if (!result.Succeeded) return Result<string>.Fail();

            var email = new EmailVm
            {
                ToEmail = "travltester@gmail.com", // Replace with admin email
                Subject = "New Vehicle Activation Request",
                Body = $@"
                <p>Hello Admin,</p>
                <p>A new vehicle activation request has been submitted by <strong>{driver.AppUser?.Name}</strong>.</p>
                <p><strong>Vehicle:</strong> {vehicle.Model} - {vehicle.LicensePlateNo}</p>
                <p><strong>Submitted On:</strong> {DateTime.UtcNow.ToString("f")} UTC</p>
                <p>Please log in to the admin dashboard to review and approve this request.</p>
                <br/>
                <p>Best,<br/>Travl Team</p>"
            };


            bool isSent = await _emailService.SendEmail(email);

            if (!isSent)
            {
                // Queue for retry 
                // BackgroundJob.Schedule<IEmailService>(service =>
                // service.SendEmail(emailVm, true), TimeSpan.FromMinutes(2));

                return Result<string>.Success(vehicle.Id, "Vehicle submitted, but admin notification failed.");
            }

            return Result<string>.Success(vehicle.Id, "Vehicle activation request successfully submitted and admin notified.");

        }
    }
}
