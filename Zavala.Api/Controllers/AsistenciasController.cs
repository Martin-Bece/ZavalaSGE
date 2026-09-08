using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/asistencias")]
public sealed class AsistenciasController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Asistencias.AsNoTracking()
            .Include(a => a.Alumno).ThenInclude(al => al.Persona)
            .Include(a => a.Comision)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Alumno.Persona.Nombre.Contains(texto) || 
                x.Alumno.Persona.Apellido.Contains(texto) || 
                x.Alumno.Legajo.Contains(texto) || 
                x.Comision.Nombre.Contains(texto) ||
                x.Estado.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderByDescending(x => x.Fecha)
            .ThenBy(x => x.Alumno.Persona.Apellido)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { asistencias = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var asistencia = await db.Asistencias.AsNoTracking()
            .Include(a => a.Alumno).ThenInclude(al => al.Persona)
            .Include(a => a.Comision)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (asistencia is null) return NotFound(new { codigo = "asistencia_no_encontrada", mensaje = "El registro de asistencia no existe." });
        return Ok(new { asistencia });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarAsistenciaRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var alumnoExiste = await db.Alumnos.AnyAsync(x => x.Id == request.AlumnoId, cancellationToken);
        if (!alumnoExiste) return BadRequest(new { codigo = "alumno_no_existe", mensaje = "El alumno seleccionado no existe." });

        var comisionExiste = await db.Comisiones.AnyAsync(x => x.Id == request.ComisionId, cancellationToken);
        if (!comisionExiste) return BadRequest(new { codigo = "comision_no_existe", mensaje = "La comisión seleccionada no existe." });

        var duplicado = await db.Asistencias.AnyAsync(x => 
            x.AlumnoId == request.AlumnoId && 
            x.ComisionId == request.ComisionId && 
            x.Fecha == request.Fecha, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "asistencia_duplicada", mensaje = "Ya se registró la asistencia de este alumno en la misma fecha y comisión." });

        var asistencia = new Asistencia();
        Aplicar(asistencia, request);
        db.Asistencias.Add(asistencia);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(asistencia).Reference(a => a.Alumno).Query().Include(al => al.Persona).LoadAsync(cancellationToken);
        await db.Entry(asistencia).Reference(a => a.Comision).LoadAsync(cancellationToken);

        return Ok(new { asistencia = Proyectar(asistencia) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarAsistenciaRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var asistencia = await db.Asistencias
            .Include(a => a.Alumno).ThenInclude(al => al.Persona)
            .Include(a => a.Comision)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (asistencia is null) return NotFound(new { codigo = "asistencia_no_encontrada", mensaje = "El registro de asistencia no existe." });

        var duplicado = await db.Asistencias.AnyAsync(x => 
            x.AlumnoId == request.AlumnoId && 
            x.ComisionId == request.ComisionId && 
            x.Fecha == request.Fecha && 
            x.Id != id, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "asistencia_duplicada", mensaje = "Ya existe otra asistencia registrada para el alumno en la misma fecha y comisión." });

        Aplicar(asistencia, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { asistencia = Proyectar(asistencia) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var asistencia = await db.Asistencias.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (asistencia is null) return NotFound(new { codigo = "asistencia_no_encontrada", mensaje = "El registro de asistencia no existe." });

        db.Asistencias.Remove(asistencia);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarAsistenciaRequest request) {
        if (request.AlumnoId <= 0) return new { codigo = "alumno_requerido", mensaje = "Debe seleccionar un alumno válido." };
        if (request.ComisionId <= 0) return new { codigo = "comision_requerida", mensaje = "Debe seleccionar una comisión válida." };
        if (string.IsNullOrWhiteSpace(request.Estado)) return new { codigo = "estado_requerido", mensaje = "El estado de asistencia es obligatorio." };
        return null;
    }

    private static void Aplicar(Asistencia asistencia, GuardarAsistenciaRequest request) {
        asistencia.AlumnoId = request.AlumnoId;
        asistencia.ComisionId = request.ComisionId;
        asistencia.Fecha = request.Fecha;
        asistencia.Estado = request.Estado.Trim();
    }

    private static AsistenciaResponse Proyectar(Asistencia x) =>
        new(
            x.Id,
            x.AlumnoId,
            x.ComisionId,
            x.Fecha,
            x.Estado,
            x.Alumno?.Persona != null ? $"{x.Alumno.Persona.Apellido}, {x.Alumno.Persona.Nombre}" : string.Empty,
            x.Alumno?.Legajo ?? string.Empty,
            x.Comision?.Nombre ?? string.Empty
        );
}

public sealed record AsistenciaResponse(
    int Id,
    int AlumnoId,
    int ComisionId,
    DateOnly Fecha,
    string Estado,
    string AlumnoNombre,
    string AlumnoLegajo,
    string ComisionNombre
);

public sealed record GuardarAsistenciaRequest(
    int AlumnoId,
    int ComisionId,
    DateOnly Fecha,
    string Estado
);