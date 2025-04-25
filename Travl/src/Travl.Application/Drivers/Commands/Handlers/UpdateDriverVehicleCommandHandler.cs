using AspNetCoreHero.Results;
using Azure.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class UpdateDriverVehicleCommandHandler : IRequestHandler<UpdateDriverVehicleCommand, IResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepositoryBase<Vehicle> _repository;
        private readonly IDriverRepository _driverRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateDriverVehicleCommandHandler(ICurrentUserService currentUserService, IRepositoryBase<Vehicle> repository, IDriverRepository driverRepository, ICloudinaryService cloudinaryService)
        {
            _currentUserService = currentUserService;
            _repository = repository;
            _driverRepository = driverRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IResult> Handle(UpdateDriverVehicleCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail("Unauthorized. User ID not found.");
            }

            var driverResult = await _driverRepository.GetDriverByAppUserId(userId);

            if (!driverResult.Succeeded)
                return Result.Fail("Driver not found.");

            var driver = driverResult.Data;
            var vehicle = driver?.Vehicles?.FirstOrDefault(v => v.Id == request.VehicleId);

            if ( vehicle == null)
            {
                return Result.Fail("Vehicle not found");
            }

            vehicle.Model = request.VehicleDto.Model ?? vehicle.Model;
            vehicle.LicensePlateNo = request.VehicleDto.LicensePlateNo ?? vehicle.LicensePlateNo;
            vehicle.Color = request.VehicleDto.Color ?? vehicle.Color;
            vehicle.EngineNumber = request.VehicleDto.EngineNumber ?? vehicle.EngineNumber;
            vehicle.UpdatedBy = driver?.AppUser?.Name;
            vehicle.Year = !string.IsNullOrEmpty(request.VehicleDto.Year) 
                ? DateOnly.Parse(request.VehicleDto.Year) : vehicle.Year; 


            // Upload vehicle images to cloudinary if any
            if (request.VehicleDto.VehicleDocumentUrl.Count > 0 || request.VehicleDto.VehicleDocumentUrl.Any())
            {
                var vehicleDocuments = new List<string>();

                for (int i = 0; i < request.VehicleDto.VehicleDocumentUrl.Count; i++)
                {
                    var uploadResult = await _cloudinaryService.AddPhotoAsync(request.VehicleDto.VehicleDocumentUrl[i]);

                    if (uploadResult == null || string.IsNullOrEmpty(uploadResult.Uri.ToString()))
                    {
                        return Result<string>.Fail("An error occured while trying to upload the Document image");
                    }

                    vehicleDocuments.Add(uploadResult.ToString());
                }

                // Delete previous vehicle images from cloudinary
                if (vehicle.VehicleDocumentUrl.Any())
                {
                    foreach (var document in vehicle.VehicleDocumentUrl)
                    {
                        var deletionResult = await _cloudinaryService.DeletePhotoAsync(document);
                    }

                }

                // Update vehicle images with the new ones
                vehicle.VehicleDocumentUrl = vehicleDocuments;
            }

            // update vehicle in the database
            var result = await _repository.UpdateAsync(vehicle);

            if (!result.Succeeded) 
                return Result.Fail("Failed to update vehicle");

            return Result.Success("Vehicle successfully updated");
        }
    }
}
