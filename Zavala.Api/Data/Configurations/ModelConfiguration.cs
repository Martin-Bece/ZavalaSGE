using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Data.Configurations;

public sealed class MateriaConfiguration : IEntityTypeConfiguration<Materia>
{
    public void Configure(EntityTypeBuilder<Materia> builder)
    {
        builder.ToTable("materias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Codigo).IsUnique();
    }
}

public sealed class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("personas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellido).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Dni).HasMaxLength(15).IsRequired();
        builder.HasIndex(x => x.Dni).IsUnique();
    }
}