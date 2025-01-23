using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Travl.Application.Interfaces;
using Travl.Domain.Commons;
using Travl.Infrastructure.Commons;
using Travl.Infrastructure.Implementations;

namespace Travl.Infrastructure
{
    public static class DependencyInjection
    {
        public static void ConfigureInfraStructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Register Interface Services

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserValidationService,  UserValidationService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
                
            #endregion

            #region Register External Services

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(c =>
            {
                c.AddPolicy(Policies.SuperAdmin, Policies.SuperAdminPolicy());
                c.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                c.AddPolicy(Policies.DriverManager, Policies.DriverManagerPolicy());
            });

            #endregion

            #region Register App Services

            //Hashing Settings 
            var hashingConfiguration = new HashingSettings()
            {
                SecretKey = configuration["HashingSettings:SecretKey"]
            };

            #endregion

        }
    }
}
