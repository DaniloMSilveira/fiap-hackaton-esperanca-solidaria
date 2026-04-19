using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Infraestructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace EsperancaSolidaria.Infraestructure.Persistence.Contexts;

public class EsperancaSolidariaDbContext : DbContext
{   
    public EsperancaSolidariaDbContext(DbContextOptions<EsperancaSolidariaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UsuarioMapping().Configure(modelBuilder.Entity<Usuario>());
        base.OnModelCreating(modelBuilder);
    }
}