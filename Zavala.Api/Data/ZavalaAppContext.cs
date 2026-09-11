using Microsoft.EntityFrameworkCore;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Data;

public partial class ZavalaAppContext(DbContextOptions<ZavalaAppContext> options) : DbContext(options)
{
    public DbSet<Alumno> Alumnos => Set<Alumno>();
    public DbSet<AsignacionesDocente> AsignacionesDocentes => Set<AsignacionesDocente>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();
    public DbSet<Calificacione> Calificaciones => Set<Calificacione>();
    public DbSet<Comisione> Comisiones => Set<Comisione>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Inscripcione> Inscripciones => Set<Inscripcione>();
    public DbSet<Materia> Materias => Set<Materia>();
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Preceptore> Preceptores => Set<Preceptore>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZavalaAppContext).Assembly);
    }
}