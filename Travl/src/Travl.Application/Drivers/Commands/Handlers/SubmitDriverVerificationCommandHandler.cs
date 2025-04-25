using AspNetCoreHero.Results;
using Azure.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class SubmitDriverVerificationCommandHandler : IRequestHandler<SubmitDriverVerificationCommand, IResult>
    {
        private readonly IRepositoryBase<UserVerification> _repository;
        private readonly IRepositoryBase<AppUser> _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICloudinaryService _cloudinaryService;

        public SubmitDriverVerificationCommandHandler(IRepositoryBase<UserVerification> repository, IRepositoryBase<AppUser> userRepository, ICurrentUserService currentUserService, ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IResult> Handle(SubmitDriverVerificationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                return Result.Fail("User Id cannot be found");

            // Confirm that the user exists
            var user = _userRepository.FindByIdAsync(userId);

            if (user == null)
                return Result.Fail("driver cannot be found");

            // Upload document image to cloudinary
            var documentUpload = await _cloudinaryService.AddPhotoAsync(request.DocumentImage);

            if (documentUpload == null)
            {
                return Result.Fail("An error occured while trying to upload the Document image");
            }

            // Create the new verification request
            var verification = new UserVerification
            {
                Id = Guid.NewGuid().ToString(),
                AppUserId = userId,
                IdentificationType = request.IdentificationType,
                IdentificationUrl = documentUpload.Uri.ToString(),
                IdentificationNo = request.IdentificationNo,
                LicenseNumber = request.LicenseNumber,
                LicenseUrl = request.LicenseUrl,
                ExpiryDate = request.ExpiryDate ?? DateTime.UtcNow.AddYears(1),
                //VerificationStatus = VerificationStatus.Pending,
                CreatedBy = _currentUserService.FullName
            };

            var result = await _repository.AddAsync(verification);

            if (!result.Succeeded) 
                return Result.Fail($"Driver verification request failed: {result.Message}");
            
            return Result.Success("Driver Verification request submitted successfully");
        }
    }
}
