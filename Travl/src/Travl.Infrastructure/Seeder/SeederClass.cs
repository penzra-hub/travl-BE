using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Travl.Domain.Context;
using Travl.Domain.Entities;
using Travl.Domain.Enums;
using Travl.Infrastructure.Commons;

namespace Travl.Infrastructure.Seeder
{
    public static class SeederClass
    {
        public static async Task SeedData(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            await RunSeed(
                serviceScope.ServiceProvider.GetService<UserManager<AppUser>>(),
                serviceScope.ServiceProvider.GetService<ApplicationContext>(),
                serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>());
        }

        private static async Task RunSeed(UserManager<AppUser> userManager, ApplicationContext context,
            RoleManager<IdentityRole> roleManager)
        {
            try
            {
                if (context != null && userManager != null && roleManager != null)
                {
                    //await context.Database.EnsureCreatedAsync();
                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await context.Database.MigrateAsync();
                    }


                    if (!context.Users.Any())
                    {
                        await roleManager.CreateAsync(new IdentityRole { Name = Policies.Admin });
                        await roleManager.CreateAsync(new IdentityRole { Name = Policies.SuperAdmin });
                        await roleManager.CreateAsync(new IdentityRole { Name = Policies.DriverManager });

                        var controlUser = new AppUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            FirstName = "John",
                            LastName = "Doe",
                            UserName = "SuperAdmin",
                            Email = "travltester@gmail.com",
                            PhoneNumber = "07025783611",
                            PhoneNumberConfirmed = true,
                            Gender = Gender.Male,
                            IsActive = true,
                            PublicId = null,
                            Avatar = "http://placehold.it/32x32",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Status = Status.Active,
                            EmailConfirmed = true,
                            UserType = UserType.Control
                        };
                        await userManager.CreateAsync(controlUser, "Password@123");
                        await userManager.AddToRoleAsync(controlUser, Policies.SuperAdmin);


                        var driverUser = new AppUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            FirstName = "Mark",
                            LastName = "Piercen",
                            UserName = "TravlDriver",
                            Email = "travldriver@gmail.com",
                            PhoneNumber = "07025783612",
                            PhoneNumberConfirmed = true,
                            Gender = Gender.Male,
                            IsActive = true,
                            PublicId = null,
                            Avatar = "http://placehold.it/32x32",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Status = Status.Active,
                            EmailConfirmed = true,
                            UserType = UserType.Driver
                        };
                        await userManager.CreateAsync(driverUser, "Password@123");
                        await userManager.AddToRoleAsync(driverUser, Policies.Driver);

                        var driver = new Driver
                        {
                            Id = Guid.NewGuid().ToString(),
                            AppUserId = driverUser.Id,
                        };
                        await context.Drivers.AddAsync(driver);

                        var passengerUser = new AppUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            FirstName = "Mark",
                            LastName = "English",
                            UserName = "TravlPassenger",
                            Email = "travlpassenger@gmail.com",
                            PhoneNumber = "07025783613",
                            PhoneNumberConfirmed = true,
                            Gender = Gender.Male,
                            IsActive = true,
                            PublicId = null,
                            Avatar = "http://placehold.it/32x32",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Status = Status.Active,
                            EmailConfirmed = true,
                            UserType = UserType.Passenger
                        };
                        await userManager.CreateAsync(passengerUser, "Password@123");
                        await userManager.AddToRoleAsync(passengerUser, Policies.Passenger);

                        var passenger = new Passenger
                        {
                            Id = Guid.NewGuid().ToString(),
                            AppUserId = passengerUser.Id,
                        };
                        await context.Passengers.AddAsync(passenger);

                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
