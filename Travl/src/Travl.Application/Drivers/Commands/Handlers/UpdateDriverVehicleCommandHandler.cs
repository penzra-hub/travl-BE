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

        public UpdateDriverVehicleCommandHandler(ICurrentUserService currentUserService, IRepositoryBase<Vehicle> repository, IDriverRepository driverRepository)
        {
            _currentUserService = currentUserService;
            _repository = repository;
            _driverRepository = driverRepository;
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

            vehicle.Model = request.Model ?? vehicle.Model;
            vehicle.LicensePlateNo = request.LicensePlateNo ?? vehicle.LicensePlateNo;
            vehicle.Color = request.Color ?? vehicle.Color;
            vehicle.EngineNumber = request.EngineNumber ?? vehicle.EngineNumber;
            vehicle.UpdatedBy = driver?.AppUser?.Name;
            vehicle.VehicleDocumentUrl = request.VehicleDocumentUrl ?? vehicle.VehicleDocumentUrl;
            //vehicle.Year = request.Year ?? vehicle.Year; 

            var result = await _repository.UpdateAsync(vehicle);

            if (!result.Succeeded) 
                return Result.Fail("Failed to update vehicle");

            return Result.Success("Vehicle successfully updated");
        }
    }
}
