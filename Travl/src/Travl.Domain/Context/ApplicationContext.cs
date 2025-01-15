using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Travl.Domain.Commons;
using Travl.Domain.Entities;

namespace Travl.Domain.Context
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> getData) : base(getData)
        {
        }
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
                    default:
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
                (type) => !string.IsNullOrEmpty(type.Namespace) && !type.IsInterface &&
                          type is { IsAbstract: false, BaseType: not null } &&
                          typeof(IEntityTypeConfiguration<>).IsAssignableFrom(type));

            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            builder.Entity<AppUser>()
                .HasIndex(t => t.Email)
                .IsUnique();

            builder.Entity<AppUser>()
                .HasIndex(t => t.PhoneNumber)
                .IsUnique();

            base.OnModelCreating(builder);
        }
    }
}
