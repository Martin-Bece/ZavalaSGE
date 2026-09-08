using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/comisiones")]
public sealed class ComisionesController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Comisiones.AsNoTracking()
            .Include(c => c.Curso)
            .Include(c => c.Preceptor).ThenInclude(p => p.Persona)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Nombre.Contains(texto) || 
                x.Curso.Nombre.Contains(texto) || 
                x.Preceptor.Persona.Nombre.Contains(texto) || 
                x.Preceptor.Persona.Apellido.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { comisiones = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var comision = await db.Comisiones.AsNoTracking()
            .Include(c => c.Curso)
            .Include(c => c.Preceptor).ThenInclude(p => p.Persona)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (comision is null) return NotFound(new { codigo = "comision_no_encontrada", mensaje = "La comisión no existe." });
        return Ok(new { comision });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarComisionRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var cursoExiste = await db.Cursos.AnyAsync(x => x.Id == request.CursoId, cancellationToken);
        if (!cursoExiste) return BadRequest(new { codigo = "curso_no_existe", mensaje = "El curso seleccionado no existe." });

        var preceptorExiste = await db.Preceptores.AnyAsync(x => x.Id == request.PreceptorId, cancellationToken);
        if (!preceptorExiste) return BadRequest(new { codigo = "preceptor_no_existe", mensaje = "El preceptor seleccionado no existe." });

        var comision = new Comisione();
        Aplicar(comision, request);
        db.Comisiones.Add(comision);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(comision).Reference(c => c.Curso).LoadAsync(cancellationToken);
        await db.Entry(comision).Reference(c => c.Preceptor).Query().Include(p => p.Persona).LoadAsync(cancellationToken);

        return Ok(new { comision = Proyectar(comision) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarComisionRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var comision = await db.Comisiones
            .Include(c => c.Curso)
            .Include(c => c.Preceptor).ThenInclude(p => p.Persona)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (comision is null) return NotFound(new { codigo = "comision_no_encontrada", mensaje = "La comisión no existe." });

        Aplicar(comision, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { comision = Proyectar(comision) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var comision = await db.Comisiones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (comision is null) return NotFound(new { codigo = "comision_no_encontrada", mensaje = "La comisión no existe." });

        var tieneAsignaciones = await db.AsignacionesDocentes.AnyAsync(x => x.ComisionId == id, cancellationToken);
        var tieneAsistencias = await db.Asistencias.AnyAsync(x => x.ComisionId == id, cancellationToken);
        var tieneCalificaciones = await db.Calificaciones.AnyAsync(x => x.ComisionId == id, cancellationToken);
        var tieneInscripciones = await db.Inscripciones.AnyAsync(x => x.ComisionId == id, cancellationToken);

        if (tieneAsignaciones || tieneAsistencias || tieneCalificaciones || tieneInscripciones) {
            return BadRequest(new { codigo = "comision_con_registros", mensaje = "No se puede eliminar la comisión porque tiene registros asociados." });
        }

        db.Comisiones.Remove(comision);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarComisionRequest request) {
        if (string.IsNullOrWhiteSpace(request.Nombre)) return new { codigo = "nombre_requerido", mensaje = "El nombre de la comisión es obligatorio." };
        if (request.CursoId <= 0) return new { codigo = "curso_requerido", mensaje = "Debe seleccionar un curso válido." };
        if (request.PreceptorId <= 0) return new { codigo = "preceptor_requerido", mensaje = "Debe seleccionar un preceptor válido." };
        return null;
    }

    private static void Aplicar(Comisione comision, GuardarComisionRequest request) {
        comision.Nombre = request.Nombre.Trim();
        comision.CursoId = request.CursoId;
        comision.PreceptorId = request.PreceptorId;
    }

    private static ComisionResponse Proyectar(Comisione x) =>
        new(
            x.Id,
            x.Nombre,
            x.CursoId,
            x.PreceptorId,
            x.Curso?.Nombre ?? string.Empty,
            x.Preceptor?.Persona != null ? $"{x.Preceptor.Persona.Apellido}, {x.Preceptor.Persona.Nombre}" : string.Empty
        );
}

public sealed record ComisionResponse(
    int Id,
    string Nombre,
    int CursoId,
    int PreceptorId,
    string CursoNombre,
    string PreceptorNombre
);

public sealed record GuardarComisionRequest(
    string Nombre,
    int CursoId,
    int PreceptorId
);