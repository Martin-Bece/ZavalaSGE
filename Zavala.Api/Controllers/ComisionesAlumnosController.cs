using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Auth;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/comisiones/{comisionId:int}/alumnos")]
[Authorize]
public sealed class ComisionesAlumnosController(ZavalaAppContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(int comisionId, CancellationToken cancellationToken)
    {
        var comision = await db.Comisiones.AsNoTracking()
            .Where(c => c.Id == comisionId)
            .Select(c => new ComisionResumenResponse(
                c.Id, c.Nombre, c.Curso.Nombre, c.Curso.CicloLectivo))
            .FirstOrDefaultAsync(cancellationToken);

        if (comision is null)
            return NotFound(new { codigo = "comision_no_encontrada", mensaje = "La comisión no existe." });

        // El admin ve todo. El preceptor, solo sus comisiones. El docente, solo donde tiene una asignación.
        var rol = User.RolCodigo();
        var permitido = false;
        if (rol == RolCodigos.Admin)
        {
            permitido = true;
        }
        else if (rol == RolCodigos.Preceptor)
        {
            var preceptorId = await db.PreceptorIdActualAsync(User, cancellationToken);
            permitido = await db.Comisiones.AnyAsync(
                c => c.Id == comisionId && c.PreceptorId == preceptorId, cancellationToken);
        }
        else if (rol == RolCodigos.Docente)
        {
            var docenteId = await db.DocenteIdActualAsync(User, cancellationToken);
            permitido = await db.AsignacionesDocentes.AnyAsync(
                a => a.ComisionId == comisionId && a.DocenteId == docenteId, cancellationToken);
        }

        if (!permitido)
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No tenés acceso a esta comisión." });

        var alumnos = await db.Inscripciones.AsNoTracking()
            .Where(i => i.ComisionId == comisionId && i.Alumno.Estado != EstadoAlumno.Baja)
            .OrderBy(i => i.Alumno.Persona.Apellido)
            .ThenBy(i => i.Alumno.Persona.Nombre)
            .Select(i => new AlumnoComisionResponse(
                i.AlumnoId,
                i.Alumno.Legajo,
                i.Alumno.Estado,
                i.Alumno.Persona.Nombre,
                i.Alumno.Persona.Apellido,
                i.Alumno.Persona.Dni))
            .ToListAsync(cancellationToken);

        return Ok(new { comision, alumnos });
    }
}

public sealed record ComisionResumenResponse(int Id, string Nombre, string CursoNombre, int CicloLectivo);

public sealed record AlumnoComisionResponse(
    int Id, string Legajo, string Estado, string Nombre, string Apellido, string Dni);