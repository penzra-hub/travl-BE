using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.Interfaces;
using Travl.Application.IRepositories;
using Travl.Domain.Context;

namespace Travl.Application.Drivers.Queries.Handlers
{
    public class GetDriverForPassengerQueryHandler : IRequestHandler<GetDriverForPassengerQuery, IResult<GetDriverDto>>
    {

        private readonly IDriverRepository _repository;   
        private readonly ICurrentUserService _currentUserService;



        public GetDriverForPassengerQueryHandler(IDriverRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;           
            _currentUserService = currentUserService;
        }


        public async Task<IResult<GetDriverDto>> Handle(GetDriverForPassengerQuery query, CancellationToken cancellationToken)
        {
            var userId = _currentUserService?.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Result<GetDriverDto>.Fail("User role not provided");
            }

            /*var userRole = _currentUserService?.UserRole;

            if (string.IsNullOrEmpty(userRole))
            {
                return Result<GetDriverDto>.Fail("User role not provided");
            }*/

            GetDriverDto? driver = null;

           /* Determine which method to call based on the user's role
            if (userRole.Equals("Admin"))
            {
                driver = await _repository.GetDriverForAdminAsync(query.driverId);
            }*/

            var result = await _repository.GetDriverForPassenger(query.driverId);
            if (result.Succeeded) driver = result.Data;           

            if (driver == null)
            {
                return Result<GetDriverDto>.Fail("Driver not found");
            }

            return await Result<GetDriverDto>.SuccessAsync(driver, "Driver successfully retrieved");

        }
    }
}
