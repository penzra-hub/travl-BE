using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;
using Travl.Domain.Commons;
using Travl.Domain.Entities;

namespace Travl.Domain.Context
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        // DbSet properties
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<RideStop> RideStops { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        item.Entity.Id = Guid.NewGuid().ToString();
                        item.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Decimal Precision
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            // String Properties
            foreach (var property in builder.Model.GetEntityTypes()
             .SelectMany(t => t.GetProperties())
             .Where(p => p.ClrType == typeof(string)))
            {
                property.SetMaxLength(255);
            }

            // Relationships and constraints

            // AppUser Constraints
            builder.Entity<AppUser>()
                .HasIndex(t => t.Email)
                .IsUnique();

            builder.Entity<AppUser>()
                .HasIndex(t => t.PhoneNumber)
                .IsUnique();

            // AppUser -> Driver
            builder.Entity<Driver>()
                .HasIndex(d => d.AppUserId)
                .IsUnique();

            // AppUser -> Passenger
            builder.Entity<Passenger>()
                .HasIndex(p => p.AppUserId)
                .IsUnique();

            // Driver -> Vehicle
            builder.Entity<Vehicle>()
                .HasOne(v => v.Driver)
                .WithMany(d => d.Vehicles)
                .HasForeignKey(v => v.DriverId);

            // Driver -> Ride
            builder.Entity<Ride>()
                .HasOne(r => r.Driver)
                .WithMany(d => d.Rides)
                .HasForeignKey(r => r.DriverId);

            // Ride -> Booking
            builder.Entity<Booking>()
                .HasOne(b => b.Ride)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RideId);

            // Passenger -> Booking
            builder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PassengerId);

            // Booking -> Payment
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId);

            // Wallet -> Transactions
            builder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId);

            // AppUser -> Wallet
            builder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.AppUserId);

            builder.Entity<AppUser>()
              .Property(u => u.Token)
              .HasMaxLength(2048);

            base.OnModelCreating(builder);
        }
    }
}
