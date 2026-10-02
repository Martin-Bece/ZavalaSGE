using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Auth;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/comisiones/{comisionId:int}/calificaciones")]
[Authorize(Roles = RolCodigos.Operativos)]
public sealed class ComisionesCalificacionesController(ZavalaAppContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(
        int comisionId, [FromQuery] int? materiaId, CancellationToken cancellationToken)
    {
        var comision = await db.Comisiones.AsNoTracking()
            .Where(c => c.Id == comisionId)
            .Select(c => new ComisionResumenResponse(c.Id, c.Nombre, c.Curso.Nombre, c.Curso.CicloLectivo))
            .FirstOrDefaultAsync(cancellationToken);

        if (comision is null)
            return NotFound(new { codigo = "comision_no_encontrada", mensaje = "La comisión no existe." });

        if (!await db.PuedeVerComisionAsync(User, comisionId, cancellationToken))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No tenés acceso a esta comisión." });

        // El docente solo ve las notas de una materia que tiene asignada en esta comisión.
        if (User.RolCodigo() == RolCodigos.Docente)
        {
            if (materiaId is null)
                return BadRequest(new { codigo = "materia_requerida", mensaje = "Indicá la materia." });

            if (!await db.PuedeCalificarAsync(User, comisionId, materiaId.Value, cancellationToken))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { codigo = "sin_permiso", mensaje = "No tenés asignada esa materia en esta comisión." });
        }

        var alumnos = await db.Inscripciones.AsNoTracking()
            .Where(i => i.ComisionId == comisionId && i.Alumno.Estado != EstadoAlumno.Baja)
            .OrderBy(i => i.Alumno.Persona.Apellido)
            .ThenBy(i => i.Alumno.Persona.Nombre)
            .Select(i => new AlumnoComisionResponse(
                i.AlumnoId, i.Alumno.Legajo, i.Alumno.Estado,
                i.Alumno.Persona.Nombre, i.Alumno.Persona.Apellido, i.Alumno.Persona.Dni))
            .ToListAsync(cancellationToken);

        var notas = await db.Calificaciones.AsNoTracking()
            .Where(c => c.ComisionId == comisionId && (materiaId == null || c.MateriaId == materiaId))
            .OrderBy(c => c.Fecha).ThenBy(c => c.Id)
            .Select(c => new NotaAlumnoResponse(
                c.Id, c.AlumnoId, c.MateriaId, c.Materia.Nombre, c.Nota, c.Instancia, c.Fecha))
            .ToListAsync(cancellationToken);

        var notasPorAlumno = notas.ToLookup(n => n.AlumnoId);
        var resultado = alumnos
            .Select(a => new AlumnoConNotasResponse(
                a.Id, a.Legajo, a.Nombre, a.Apellido, notasPorAlumno[a.Id].ToList()))
            .ToList();

        return Ok(new { comision, alumnos = resultado });
    }
}

public sealed record NotaAlumnoResponse(
    int Id, int AlumnoId, int MateriaId, string MateriaNombre, decimal Nota, string Instancia, DateOnly? Fecha);

public sealed record AlumnoConNotasResponse(
    int Id, string Legajo, string Nombre, string Apellido, List<NotaAlumnoResponse> Calificaciones);