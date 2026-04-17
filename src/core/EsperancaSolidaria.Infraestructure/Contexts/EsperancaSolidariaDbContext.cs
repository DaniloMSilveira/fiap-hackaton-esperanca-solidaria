using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Infraestructure.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EsperancaSolidaria.Infraestructure.Contexts;

public class EsperancaSolidariaDbContext : DbContext
{
    private readonly IHostEnvironment _environment;
    
    public EsperancaSolidariaDbContext(DbContextOptions<EsperancaSolidariaDbContext> options, IHostEnvironment environment)
        : base(options)
    {
        _environment = environment;
    }

    public DbSet<Usuario> Usuarios { get; set; }

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
        new UsuarioMapping().Configure(modelBuilder.Entity<Usuario>());
        base.OnModelCreating(modelBuilder);
    }
}