using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;

namespace Zavala.Api.Auth;

public static class AccesoComisionExtensions
{
    // Ver una comisión: el admin todas, el preceptor la suya, el docente donde tenga alguna asignación.
    public static async Task<bool> PuedeVerComisionAsync(
        this ZavalaAppContext db, ClaimsPrincipal usuario, int comisionId, CancellationToken cancellationToken)
    {
        var rol = usuario.RolCodigo();
        if (rol == RolCodigos.Admin) return true;

        if (rol == RolCodigos.Preceptor)
        {
            var preceptorId = await db.PreceptorIdActualAsync(usuario, cancellationToken);
            return preceptorId is not null && await db.Comisiones.AnyAsync(
                c => c.Id == comisionId && c.PreceptorId == preceptorId, cancellationToken);
        }

        if (rol == RolCodigos.Docente)
        {
            var docenteId = await db.DocenteIdActualAsync(usuario, cancellationToken);
            return docenteId is not null && await db.AsignacionesDocentes.AnyAsync(
                a => a.ComisionId == comisionId && a.DocenteId == docenteId, cancellationToken);
        }

        return false;
    }

    // Calificar: el admin, o el docente que tiene asignada ESA materia en ESA comisión.
    public static async Task<bool> PuedeCalificarAsync(
        this ZavalaAppContext db, ClaimsPrincipal usuario, int comisionId, int materiaId, CancellationToken cancellationToken)
    {
        var rol = usuario.RolCodigo();
        if (rol == RolCodigos.Admin) return true;
        if (rol != RolCodigos.Docente) return false;

        var docenteId = await db.DocenteIdActualAsync(usuario, cancellationToken);
        return docenteId is not null && await db.AsignacionesDocentes.AnyAsync(
            a => a.DocenteId == docenteId && a.ComisionId == comisionId && a.MateriaId == materiaId, cancellationToken);
    }

    // Ver un alumno: el admin siempre; el preceptor o el docente, solo si el alumno
    // está inscripto en una comisión suya.
    public static async Task<bool> PuedeVerAlumnoAsync(
        this ZavalaAppContext db, ClaimsPrincipal usuario, int alumnoId, CancellationToken cancellationToken)
    {
        var rol = usuario.RolCodigo();
        if (rol == RolCodigos.Admin) return true;

        if (rol == RolCodigos.Preceptor)
        {
            var preceptorId = await db.PreceptorIdActualAsync(usuario, cancellationToken);
            return preceptorId is not null && await db.Inscripciones.AnyAsync(
                i => i.AlumnoId == alumnoId && i.Comision.PreceptorId == preceptorId, cancellationToken);
        }

        if (rol == RolCodigos.Docente)
        {
            var docenteId = await db.DocenteIdActualAsync(usuario, cancellationToken);
            return docenteId is not null && await db.Inscripciones.AnyAsync(
                i => i.AlumnoId == alumnoId
                     && db.AsignacionesDocentes.Any(a => a.DocenteId == docenteId && a.ComisionId == i.ComisionId),
                cancellationToken);
        }

        return false;
    }

}