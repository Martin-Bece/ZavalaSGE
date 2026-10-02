using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Auth;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/docente/materias")]
[Authorize(Roles = RolCodigos.Docente)]
public sealed class DocenteMateriasController(ZavalaAppContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var docenteId = await db.DocenteIdActualAsync(User, cancellationToken);
        if (docenteId is null)
            return NotFound(new { codigo = "docente_no_encontrado", mensaje = "Tu usuario no está vinculado a un docente." });

        var asignaciones = await db.AsignacionesDocentes.AsNoTracking()
            .Where(a => a.DocenteId == docenteId)
            .OrderBy(a => a.Materia.Nombre)
            .ThenBy(a => a.Comision.Nombre)
            .Select(a => new AsignacionDocenteMateriasResponse(
                a.Id,
                a.MateriaId,
                a.Materia.Nombre,
                a.Materia.Codigo,
                a.ComisionId,
                a.Comision.Nombre,
                a.Comision.Curso.Nombre,
                a.Comision.Curso.CicloLectivo,
                db.Inscripciones.Count(i => i.ComisionId == a.ComisionId && i.Alumno.Estado != EstadoAlumno.Baja)))
            .ToListAsync(cancellationToken);

        return Ok(new { asignaciones });
    }
}

public sealed record AsignacionDocenteMateriasResponse(
    int Id, int MateriaId, string MateriaNombre, string? MateriaCodigo,
    int ComisionId, string ComisionNombre, string CursoNombre, int CicloLectivo, int CantidadAlumnos);