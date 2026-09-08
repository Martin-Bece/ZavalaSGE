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
        
        // Mapeos explícitos de columnas a la BD (snake_case)
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Dni).HasColumnName("dni").HasMaxLength(15).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellido).HasColumnName("apellido").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(30);
        builder.Property(x => x.Direccion).HasColumnName("direccion");
        
        // ACÁ ESTABA EL ERROR: mapea FechaNacimiento a la columna fecha_nacimiento
        builder.Property(x => x.FechaNacimiento).HasColumnName("fecha_nacimiento");

        builder.HasIndex(x => x.Dni).IsUnique();
    }
}