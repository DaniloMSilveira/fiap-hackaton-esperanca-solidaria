using EsperancaSolidaria.Domain.Entities;
using EsperancaSolidaria.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EsperancaSolidaria.Infraestructure.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.OwnsOne(o => o.Email, opt =>
        {
            opt.Property(p => p.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.OwnsOne(o => o.Cpf, opt =>
        {
            opt.Property(p => p.Value)
                .HasColumnName("Cpf")
                .IsRequired()
                .HasMaxLength(11);
        });

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.SenhaCriptografada)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Perfil)
            .IsRequired();

        builder.Property(u => u.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.DataCriacao)
            .HasColumnType("datetime")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.DataAtualizacao)
            .HasColumnType("datetime")
            .IsRequired(false);

        builder.ToTable("Usuario");
    }
}