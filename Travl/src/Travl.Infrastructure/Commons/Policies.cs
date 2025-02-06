using Microsoft.AspNetCore.Authorization;

namespace Travl.Infrastructure.Commons
{
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string DriverManager = "DriverManager";
        public const string Passenger = "Passenger";
        public const string Driver = "Driver";

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }

        public static AuthorizationPolicy SuperAdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(SuperAdmin).Build();
        }

        public static AuthorizationPolicy DriverManagerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(DriverManager).Build();
        }

        public static AuthorizationPolicy PassengerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Passenger).Build();
        }

        public static AuthorizationPolicy DriverPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Driver).Build();
        }
    }
}
