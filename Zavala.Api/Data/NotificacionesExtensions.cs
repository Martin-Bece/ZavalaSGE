using Zavala.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Zavala.Api.Data;

public static class NotificacionesExtensions
{
    // Agrega la notificación SIN guardar: se guarda con el SaveChanges de la acción que la
    // origina. Destinatario: el interesado, nunca el usuario que ejecuta la acción.
    public static void Notificar(this ZavalaAppContext db, int usuarioId, string titulo, string mensaje)
    {
        db.Notificaciones.Add(new Notificacion
        {
            UsuarioId = usuarioId,
            Titulo = titulo,
            Mensaje = mensaje,
            Creada = DateTime.UtcNow,
            Leida = false
        });
    }

    // Usuario (si tiene) de un preceptor: preceptor -> persona -> usuario.
    public static Task<int?> UsuarioIdDePreceptorAsync(
        this ZavalaAppContext db, int preceptorId, CancellationToken cancellationToken) =>
        db.Usuarios.AsNoTracking()
            .Where(u => db.Preceptores.Any(p => p.Id == preceptorId && p.PersonaId == u.PersonaId))
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

    // Usuarios de los docentes que tienen asignación en alguna comisión donde está inscripto el alumno.
    public static Task<List<int>> UsuarioIdsDeDocentesDelAlumnoAsync(
        this ZavalaAppContext db, int alumnoId, CancellationToken cancellationToken) =>
        db.Usuarios.AsNoTracking()
            .Where(u => db.AsignacionesDocentes.Any(a =>
                a.Docente.PersonaId == u.PersonaId
                && db.Inscripciones.Any(i => i.AlumnoId == alumnoId && i.ComisionId == a.ComisionId)))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

    // Nombre del que ejecuta la acción: el token no lo trae, se lee de la base.
    public static Task<string> NombreDeUsuarioAsync(
        this ZavalaAppContext db, int usuarioId, CancellationToken cancellationToken) =>
        db.Usuarios.AsNoTracking()
            .Where(u => u.Id == usuarioId)
            .Select(u => u.Persona.Nombre + " " + u.Persona.Apellido)
            .FirstAsync(cancellationToken);

}