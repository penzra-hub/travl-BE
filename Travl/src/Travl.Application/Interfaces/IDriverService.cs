using AspNetCoreHero.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travl.Application.Dtos.DriverDto;
using Travl.Domain.Entities;

namespace Travl.Application.Interfaces
{
    public interface IDriverService 
    {
        Task<IResult> CreateDriverProfile(AppUser user);
        Task<GetDriverDto> GetDriverForPassengerAsync(string DriverId);
        Task<IResult> UpdateDriverAsync(string id, UpdateDriverDto updateDriverDto);
    }
}
