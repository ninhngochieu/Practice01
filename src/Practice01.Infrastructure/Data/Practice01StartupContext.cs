using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practice01.Startup.Areas.Identity.Data;

namespace Practice01.Startup.Data;

public class Practice01StartupContext : IdentityDbContext<Practice01StartupUser>
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
    }
}
