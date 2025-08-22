using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practice01.Domain.Entities;

namespace Practice01.Infrastructure.Data;

public class Practice01StartupContext : IdentityDbContext<User, Role, Guid>
{
    public Practice01StartupContext(DbContextOptions<Practice01StartupContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        
        builder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Manager",
                NormalizedName = "MANAGER"
            },
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Member",
                NormalizedName = "MEMBER"
            }
        );
    }
}
