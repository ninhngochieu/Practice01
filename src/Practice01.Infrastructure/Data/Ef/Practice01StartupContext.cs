using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practice01.Domain.Entities.Roles;
using Practice01.Domain.Entities.Users;

namespace Practice01.Infrastructure.Data.Ef;

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

        var adminRole = Guid.Parse("dc8bcc55-8540-4bb3-b45c-719ea1bce0f2");
        var managerRole = Guid.Parse("ddad094e-f7b4-446c-9639-9f7a695a4db8");
        var memberRole = Guid.Parse("0517ce8f-9d05-40ae-8c42-d93c8b5da363");
        builder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRole,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new Role
            {
                Id = managerRole,
                Name = "Manager",
                NormalizedName = "MANAGER"
            },
            new Role
            {
                Id = memberRole,
                Name = "Member",
                NormalizedName = "MEMBER"
            }
        );

        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Id = Guid.Parse("ba638760-5686-4e92-b4d5-59850381bd8b"),
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "pA5eF@example.com",
            NormalizedEmail = "pA5eF@example.com",
            EmailConfirmed = true,
            SecurityStamp = string.Empty,
            ConcurrencyStamp = string.Empty,
            PhoneNumber = "+7(999)999-99-99",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
        
        var manager = new User
        {
            Id = Guid.Parse("b745d2be-dc7c-46ec-b4b3-da2c83099fd9"),
            UserName = "manager",
            NormalizedUserName = "MANAGER",
            Email = "9D2L6@example.com",
            NormalizedEmail = "9D2L6@example.com",
            EmailConfirmed = true,
            SecurityStamp = string.Empty,
            ConcurrencyStamp = string.Empty,
            PhoneNumber = "+7(999)999-99-99",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        manager.PasswordHash = hasher.HashPassword(manager, "Manager123!");
        
        var member = new User
        {
            Id = Guid.Parse("dbd9b6f3-12d6-4755-824c-2933ecce4c4a"),
            UserName = "member",
            NormalizedUserName = "MEMBER",
            Email = "6tMf6@example.com",
            NormalizedEmail = "6tMf6@example.com",
            EmailConfirmed = true,
            SecurityStamp = string.Empty,
            ConcurrencyStamp = string.Empty,
            PhoneNumber = "+7(999)999-99-99",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        member.PasswordHash = hasher.HashPassword(member, "Member123!");
        
        builder.Entity<User>().HasData(new List<User> { admin, manager, member });
        
        builder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid> { UserId = admin.Id, RoleId = adminRole },
            new IdentityUserRole<Guid> { UserId = admin.Id, RoleId = managerRole },
            new IdentityUserRole<Guid> { UserId = admin.Id, RoleId = memberRole },
            new IdentityUserRole<Guid> { UserId = manager.Id, RoleId = managerRole },
            new IdentityUserRole<Guid> { UserId = manager.Id, RoleId = memberRole },
            new IdentityUserRole<Guid> { UserId = member.Id, RoleId = memberRole }
        );
    }
}
