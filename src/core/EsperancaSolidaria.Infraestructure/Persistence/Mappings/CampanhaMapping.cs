using EsperancaSolidaria.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsperancaSolidaria.Infraestructure.Persistence.Mappings;

public class CampanhaMapping : IEntityTypeConfiguration<Campanha>
{
    public void Configure(EntityTypeBuilder<Campanha> builder)
    {
        builder.ToTable("Campanha");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Titulo)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Descricao)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.DataInicio)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(c => c.DataFim)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(c => c.MetaFinanceira)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(c => c.ValorArrecadado)
            .HasColumnType("decimal(18, 2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.DataCriacao)
            .HasColumnType("datetime")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UsuarioCriacao)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.DataAtualizacao)
            .HasColumnType("datetime")
            .IsRequired(false);

        builder.Property(c => c.UsuarioAtualizacao)
            .IsRequired(false)
            .HasMaxLength(255);
    }
}
