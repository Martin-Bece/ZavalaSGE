using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/inscripciones")]
[Authorize]
public sealed class InscripcionesController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Inscripciones.AsNoTracking()
            .Include(i => i.Alumno).ThenInclude(a => a.Persona)
            .Include(i => i.Comision)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Alumno.Persona.Nombre.Contains(texto) || 
                x.Alumno.Persona.Apellido.Contains(texto) || 
                x.Alumno.Legajo.Contains(texto) || 
                x.Comision.Nombre.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderByDescending(x => x.FechaInscripcion)
            .ThenBy(x => x.Alumno.Persona.Apellido)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { inscripciones = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var inscripcion = await db.Inscripciones.AsNoTracking()
            .Include(i => i.Alumno).ThenInclude(a => a.Persona)
            .Include(i => i.Comision)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (inscripcion is null) return NotFound(new { codigo = "inscripcion_no_encontrada", mensaje = "La inscripción no existe." });
        return Ok(new { inscripcion });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarInscripcionRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var alumnoExiste = await db.Alumnos.AnyAsync(x => x.Id == request.AlumnoId, cancellationToken);
        if (!alumnoExiste) return BadRequest(new { codigo = "alumno_no_existe", mensaje = "El alumno seleccionado no existe." });

        var comisionExiste = await db.Comisiones.AnyAsync(x => x.Id == request.ComisionId, cancellationToken);
        if (!comisionExiste) return BadRequest(new { codigo = "comision_no_existe", mensaje = "La comisión seleccionada no existe." });

        var duplicado = await db.Inscripciones.AnyAsync(x => x.AlumnoId == request.AlumnoId && x.ComisionId == request.ComisionId, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "inscripcion_duplicada", mensaje = "El alumno ya se encuentra inscripto en esta comisión." });

        var inscripcion = new Inscripcione();
        Aplicar(inscripcion, request);
        db.Inscripciones.Add(inscripcion);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(inscripcion).Reference(i => i.Alumno).Query().Include(a => a.Persona).LoadAsync(cancellationToken);
        await db.Entry(inscripcion).Reference(i => i.Comision).LoadAsync(cancellationToken);

        return Ok(new { inscripcion = Proyectar(inscripcion) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarInscripcionRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var inscripcion = await db.Inscripciones
            .Include(i => i.Alumno).ThenInclude(a => a.Persona)
            .Include(i => i.Comision)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (inscripcion is null) return NotFound(new { codigo = "inscripcion_no_encontrada", mensaje = "La inscripción no existe." });

        var duplicado = await db.Inscripciones.AnyAsync(x => x.AlumnoId == request.AlumnoId && x.ComisionId == request.ComisionId && x.Id != id, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "inscripcion_duplicada", mensaje = "El alumno ya se encuentra inscripto en esta comisión." });

        Aplicar(inscripcion, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { inscripcion = Proyectar(inscripcion) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var inscripcion = await db.Inscripciones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (inscripcion is null) return NotFound(new { codigo = "inscripcion_no_encontrada", mensaje = "La inscripción no existe." });

        db.Inscripciones.Remove(inscripcion);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarInscripcionRequest request) {
        if (request.AlumnoId <= 0) return new { codigo = "alumno_requerido", mensaje = "Debe seleccionar un alumno válido." };
        if (request.ComisionId <= 0) return new { codigo = "comision_requerida", mensaje = "Debe seleccionar una comisión válida." };
        return null;
    }

    private static void Aplicar(Inscripcione inscripcion, GuardarInscripcionRequest request) {
        inscripcion.AlumnoId = request.AlumnoId;
        inscripcion.ComisionId = request.ComisionId;
        inscripcion.FechaInscripcion = request.FechaInscripcion ?? DateOnly.FromDateTime(DateTime.Now);
    }

    private static InscripcionResponse Proyectar(Inscripcione x) =>
        new(
            x.Id,
            x.AlumnoId,
            x.ComisionId,
            x.FechaInscripcion,
            x.Alumno?.Persona != null ? $"{x.Alumno.Persona.Apellido}, {x.Alumno.Persona.Nombre}" : string.Empty,
            x.Alumno?.Legajo ?? string.Empty,
            x.Comision?.Nombre ?? string.Empty
        );
}

public sealed record InscripcionResponse(
    int Id,
    int AlumnoId,
    int ComisionId,
    DateOnly? FechaInscripcion,
    string AlumnoNombre,
    string AlumnoLegajo,
    string ComisionNombre
);

public sealed record GuardarInscripcionRequest(
    int AlumnoId,
    int ComisionId,
    DateOnly? FechaInscripcion
);