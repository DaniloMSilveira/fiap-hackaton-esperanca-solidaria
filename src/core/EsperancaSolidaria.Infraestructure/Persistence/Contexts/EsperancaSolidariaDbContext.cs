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
    public DbSet<Campanha> Campanhas { get; set; }
    public DbSet<Doacao> Doacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UsuarioMapping().Configure(modelBuilder.Entity<Usuario>());
        new CampanhaMapping().Configure(modelBuilder.Entity<Campanha>());
        new DoacaoMapping().Configure(modelBuilder.Entity<Doacao>());
        base.OnModelCreating(modelBuilder);
    }
}