using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;

namespace Zavala.Api.Auth;

public static class UsuarioActualExtensions
{
    // Id del preceptor vinculado al usuario logueado (usuario -> persona -> preceptor), o null.
    public static Task<int?> PreceptorIdActualAsync(
        this ZavalaAppContext db, ClaimsPrincipal usuario, CancellationToken cancellationToken)
    {
        var usuarioId = (int)usuario.UsuarioId();
        return db.Preceptores.AsNoTracking()
            .Where(p => db.Usuarios.Any(u => u.Id == usuarioId && u.PersonaId == p.PersonaId))
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Lo mismo para el docente.
    public static Task<int?> DocenteIdActualAsync(
        this ZavalaAppContext db, ClaimsPrincipal usuario, CancellationToken cancellationToken)
    {
        var usuarioId = (int)usuario.UsuarioId();
        return db.Docentes.AsNoTracking()
            .Where(d => db.Usuarios.Any(u => u.Id == usuarioId && u.PersonaId == d.PersonaId))
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}