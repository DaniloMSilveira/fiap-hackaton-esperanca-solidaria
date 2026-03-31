using EsperancaSolidaria.Infra.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EsperancaSolidaria.Infra.Security.Contexts;

public class IdentityDataContext : IdentityDbContext<IdentityCustomUser>
{
    private readonly IHostEnvironment _environment;

    public IdentityDataContext(DbContextOptions<IdentityDataContext> options, IHostEnvironment environment) : base(options)
    {
        _environment = environment;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_environment.IsDevelopment())
            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<IdentityCustomUser>(b => { b.ToTable("Users"); });
        modelBuilder.Entity<IdentityRole>(b => { b.ToTable("Roles"); });
        modelBuilder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); });
        modelBuilder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaims"); });
        modelBuilder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogins"); });
        modelBuilder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaims"); });
        modelBuilder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserTokens"); });
    }
}