using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travl.Application.Drivers.Commands;
using Travl.Application.Drivers.Queries;
using Travl.Application.Dtos.DriverDto;
using Travl.Application.Interfaces;
using Travl.Infrastructure.Implementations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Travl.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DriversController : ApiController
    {    

        [HttpGet("{driverId}")]
        public async Task<IActionResult> Get(string driverId)
        {
            var query = new GetDriverForPassengerQuery()
            {
                driverId = driverId
            };
            return await Initiate(() => Mediator.Send(query));

        }


        [HttpPut("update-driver")]
        public async Task<IActionResult> UpdateDriver([FromForm] UpdateDriverBasicDetailsCommand command)
        {
            return await Initiate(() => Mediator.Send(command));
        }
    }
}
