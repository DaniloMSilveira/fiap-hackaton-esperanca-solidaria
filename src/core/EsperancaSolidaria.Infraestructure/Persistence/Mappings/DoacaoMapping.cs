using EsperancaSolidaria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsperancaSolidaria.Infraestructure.Persistence.Mappings;

public class DoacaoMapping : IEntityTypeConfiguration<Doacao>
{
    public void Configure(EntityTypeBuilder<Doacao> builder)
    {
        builder.ToTable("Doacao");

        builder.HasKey(d => d.Id);

        builder.HasIndex(c => c.ReferenciaPagamento)
            .IsUnique();

        builder.Property(d => d.CampanhaId)
            .IsRequired();
        builder.HasOne(d => d.Campanha)
            .WithMany()
            .HasForeignKey(d => d.CampanhaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(d => d.DoadorId)
            .IsRequired();
        builder.HasOne(p => p.Doador)
            .WithMany()
            .HasForeignKey(d => d.DoadorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(d => d.Valor)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(d => d.DataDoacao)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(d => d.ReferenciaPagamento)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.DataCriacao)
            .HasColumnType("datetime")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
