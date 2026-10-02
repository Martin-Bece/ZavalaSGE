using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Auth;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/preceptor/comisiones")]
[Authorize(Roles = RolCodigos.Preceptor)]
public sealed class PreceptorComisionesController(ZavalaAppContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var preceptorId = await db.PreceptorIdActualAsync(User, cancellationToken);
        if (preceptorId is null)
            return NotFound(new { codigo = "preceptor_no_encontrado", mensaje = "Tu usuario no está vinculado a un preceptor." });

        var comisiones = await db.Comisiones.AsNoTracking()
            .Where(c => c.PreceptorId == preceptorId)
            .OrderBy(c => c.Curso.Nombre)
            .ThenBy(c => c.Nombre)
            .Select(c => new ComisionPreceptorResponse(
                c.Id,
                c.Nombre,
                c.CursoId,
                c.Curso.Nombre,
                c.Curso.CicloLectivo,
                db.Inscripciones.Count(i => i.ComisionId == c.Id && i.Alumno.Estado != EstadoAlumno.Baja)))
            .ToListAsync(cancellationToken);

        return Ok(new { comisiones });
    }
}

public sealed record ComisionPreceptorResponse(
    int Id, string Nombre, int CursoId, string CursoNombre, int CicloLectivo, int CantidadAlumnos);