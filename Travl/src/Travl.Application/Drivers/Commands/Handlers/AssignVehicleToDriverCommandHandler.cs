﻿using AspNetCoreHero.Results;
using AutoMapper;
using Azure.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Domain.Enums;

namespace Travl.Application.Drivers.Commands.Handlers
{
    public class AssignVehicleToDriverCommandHandler : IRequestHandler<AssignVehicleToDriverCommand, IResult<string>>
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IRepositoryBase<Vehicle> _repository;          
        private readonly ICurrentUserService _currentUser;


        public AssignVehicleToDriverCommandHandler(IDriverRepository driverRepository, IRepositoryBase<Vehicle> repository, ICurrentUserService currentUser)
        {
            _driverRepository = driverRepository;
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<IResult<string>> Handle(AssignVehicleToDriverCommand request, CancellationToken cancellationToken)
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

            var vehicle = new Vehicle()
            {
                Id = Guid.NewGuid().ToString(),
                Status = Status.Active,
                DriverId = driver.Id,
                Model = request.Model,
                LicensePlateNo = request.LicensePlateNo,
                Color = request.Color,
                EngineNumber = request.EngineNumber,
                CreatedBy = driver?.AppUser?.Name,
                VehicleDocumentUrl = request.VehicleDocumentUrl
            };

            var result = await _repository.AddAsync(vehicle);

            if (!result.Succeeded) return Result<string>.Fail();

            return Result<string>.Success(vehicle.Id, "Vehicle successfully assigned");
        }
    }
}
