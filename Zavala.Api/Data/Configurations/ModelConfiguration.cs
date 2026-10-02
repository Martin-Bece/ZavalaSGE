using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Data.Configurations;

public sealed class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("personas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Dni).HasColumnName("dni").HasMaxLength(15).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellido).HasColumnName("apellido").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(30);
        builder.Property(x => x.Direccion).HasColumnName("direccion");
        builder.Property(x => x.FechaNacimiento).HasColumnName("fecha_nacimiento");

        builder.HasIndex(x => x.Dni).IsUnique();
    }
}

public sealed class AlumnoConfiguration : IEntityTypeConfiguration<Alumno>
{
    public void Configure(EntityTypeBuilder<Alumno> builder)
    {
        builder.ToTable("alumnos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Legajo).HasColumnName("legajo").HasMaxLength(50).IsRequired();
        builder.Property(x => x.PersonaId).HasColumnName("persona_id").IsRequired();

        builder.HasIndex(x => x.Legajo).IsUnique();
        builder.HasIndex(x => x.PersonaId).IsUnique();
    }
}

public sealed class AsistenciaConfiguration : IEntityTypeConfiguration<Asistencia>
{
    public void Configure(EntityTypeBuilder<Asistencia> builder)
    {
        builder.ToTable("asistencias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.AlumnoId).HasColumnName("alumno_id").IsRequired();
        builder.Property(x => x.ComisionId).HasColumnName("comision_id").IsRequired();
        builder.Property(x => x.Fecha).HasColumnName("fecha").IsRequired();
        builder.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(20).IsRequired();

        builder.HasOne(x => x.Alumno)
            .WithMany(a => a.Asistencia)
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Comision)
            .WithMany(c => c.Asistencia)
            .HasForeignKey(x => x.ComisionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class CalificacionConfiguration : IEntityTypeConfiguration<Calificacione>
{
    public void Configure(EntityTypeBuilder<Calificacione> builder)
    {
        builder.ToTable("calificaciones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.AlumnoId).HasColumnName("alumno_id").IsRequired();
        builder.Property(x => x.MateriaId).HasColumnName("materia_id").IsRequired();
        builder.Property(x => x.ComisionId).HasColumnName("comision_id").IsRequired();
        builder.Property(x => x.Nota).HasColumnName("nota").HasPrecision(4, 2).IsRequired();
        builder.Property(x => x.Instancia).HasColumnName("instancia").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Fecha).HasColumnName("fecha");

        builder.HasOne(x => x.Alumno)
            .WithMany(a => a.Calificaciones)
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Materia)
            .WithMany(m => m.Calificaciones)
            .HasForeignKey(x => x.MateriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Comision)
            .WithMany(c => c.Calificaciones)
            .HasForeignKey(x => x.ComisionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ComisionConfiguration : IEntityTypeConfiguration<Comisione>
{
    public void Configure(EntityTypeBuilder<Comisione> builder)
    {
        builder.ToTable("comisiones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
        builder.Property(x => x.CursoId).HasColumnName("curso_id").IsRequired();
        builder.Property(x => x.PreceptorId).HasColumnName("preceptor_id").IsRequired();

        builder.HasOne(x => x.Curso)
            .WithMany(c => c.Comisiones)
            .HasForeignKey(x => x.CursoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Preceptor)
            .WithMany(c => c.Comisiones)
            .HasForeignKey(x => x.PreceptorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("cursos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
        builder.Property(x => x.CicloLectivo).HasColumnName("ciclo_lectivo").IsRequired();
    }
}

public sealed class DocenteConfiguration : IEntityTypeConfiguration<Docente>
{
    public void Configure(EntityTypeBuilder<Docente> builder)
    {
        builder.ToTable("docentes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Legajo).HasColumnName("legajo").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(100);
        builder.Property(x => x.PersonaId).HasColumnName("persona_id").IsRequired();

        builder.HasIndex(x => x.Legajo).IsUnique();
        builder.HasIndex(x => x.PersonaId).IsUnique();
    }
}

public sealed class InscripcionConfiguration : IEntityTypeConfiguration<Inscripcione>
{
    public void Configure(EntityTypeBuilder<Inscripcione> builder)
    {
        builder.ToTable("inscripciones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.AlumnoId).HasColumnName("alumno_id").IsRequired();
        builder.Property(x => x.ComisionId).HasColumnName("comision_id").IsRequired();
        builder.Property(x => x.FechaInscripcion).HasColumnName("fecha_inscripcion");

        builder.HasOne(x => x.Alumno)
            .WithMany(a => a.Inscripciones)
            .HasForeignKey(x => x.AlumnoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Comision)
            .WithMany(c => c.Inscripciones)
            .HasForeignKey(x => x.ComisionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MateriaConfiguration : IEntityTypeConfiguration<Materia>
{
    public void Configure(EntityTypeBuilder<Materia> builder)
    {
        builder.ToTable("materias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20);

        builder.HasIndex(x => x.Codigo).IsUnique();
    }
}

public sealed class PreceptorConfiguration : IEntityTypeConfiguration<Preceptore>
{
    public void Configure(EntityTypeBuilder<Preceptore> builder)
    {
        builder.ToTable("preceptores");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Legajo).HasColumnName("legajo").HasMaxLength(50).IsRequired();
        builder.Property(x => x.PersonaId).HasColumnName("persona_id").IsRequired();

        builder.HasIndex(x => x.Legajo).IsUnique();
        builder.HasIndex(x => x.PersonaId).IsUnique();
    }
}

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Rol).HasColumnName("rol").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Estado).HasColumnName("estado");
        builder.Property(x => x.PersonaId).HasColumnName("persona_id").IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.PersonaId).IsUnique();
    }
}

public sealed class AsignacionesDocenteConfiguration : IEntityTypeConfiguration<AsignacionesDocente>
{
    public void Configure(EntityTypeBuilder<AsignacionesDocente> builder)
    {
        builder.ToTable("asignaciones_docente");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.DocenteId).HasColumnName("docente_id").IsRequired();
        builder.Property(x => x.MateriaId).HasColumnName("materia_id").IsRequired();
        builder.Property(x => x.ComisionId).HasColumnName("comision_id").IsRequired();

        builder.HasOne(x => x.Docente)
            .WithMany(d => d.AsignacionesDocentes)
            .HasForeignKey(x => x.DocenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Materia)
            .WithMany(m => m.AsignacionesDocentes)
            .HasForeignKey(x => x.MateriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Comision)
            .WithMany(c => c.AsignacionesDocentes)
            .HasForeignKey(x => x.ComisionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("notificaciones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Mensaje).HasColumnName("mensaje").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Creada).HasColumnName("creada").IsRequired();
        builder.Property(x => x.Leida).HasColumnName("leida").IsRequired();

        builder.HasIndex(x => new { x.UsuarioId, x.Leida });

        // WithMany() vacío está bien acá: Usuario no tiene una colección de notificaciones.
        // El problema de antes era al revés: había colección y no se la indicaba.
        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}