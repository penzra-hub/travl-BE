using AspNetCoreHero.Results;
using Azure.Core;
using CloudinaryDotNet.Actions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Commons;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class SubmitDriverVerificationCommandHandler : IRequestHandler<SubmitDriverVerificationCommand, IResult>
    {
        private readonly IRepositoryBase<UserVerification> _repository;
        private readonly IDriverRepository _driverRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IEmailService _emailService;

        public SubmitDriverVerificationCommandHandler(IRepositoryBase<UserVerification> repository, IRepositoryBase<Driver> driverRepository, ICurrentUserService currentUserService, ICloudinaryService cloudinaryService, IEmailService emailService)
        {
            _repository = repository;
            _driverRepository = driverRepository;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
            _emailService = emailService;
        }

        public async Task<IResult> Handle(SubmitDriverVerificationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                return Result.Fail("Unathorized. User Id cannot be found");

            
            var driverResult = await _driverRepository.GetDriverByAppUserId(userId);

            if (!driverResult.Succeeded)
                return Result<string>.Fail("Driver not found.");

            var driver = driverResult.Data;

            if (driver.VerificationStatus != VerificationStatus.Pending)
                return Result.Fail("Driver is not verified yet");

            var licenceUpload = await _cloudinaryService.AddPhotoAsync(request.LicensePhoto);

            if (licenceUpload == null)
            {
                return Result.Fail("An error occured while trying to upload the licence photo");
            }

            // Create the new verification request
            var licenceVerification = new UserVerification
            {
                Id = Guid.NewGuid().ToString(),
                AppUserId = userId,
                LicenseNumber = request.LicenseNumber,
                LicenseUrl = licenceUpload.Uri.ToString(),
                ExpiryDate = request.ExpiryDate,
                Status = Status.Pending,
                CreatedBy = _currentUserService.FullName
            };

            var result = await _repository.AddAsync(licenceVerification);

            if (!result.Succeeded) 
                return Result.Fail($"Driver verification request failed: {result.Message}");


            var email = new EmailVm
            {
                ToEmail = "travltester@gmail.com", // Replace with your admin email or fetch dynamically
                Subject = "New Driver Activation Request",
                Body = $@"
                <p>Hello Admin,</p>
                <p>A new driver activation request has been submitted by <strong>{driver.AppUser?.Name}</strong>.</p>
                <p><strong>Licence:</strong> {licenceVerification.LicenseNumber} - {licenceVerification.ExpiryDate}</p>
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

                return Result<string>.Success(licenceVerification.Id, "Driver activation request submitted, but admin notification failed.");
            }

            return Result<string>.Success(licenceVerification.Id, "Driver activation request successfully assigned and admin notified.");
        }
    }
}
