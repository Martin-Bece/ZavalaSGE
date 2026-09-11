using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/docentes")]
public sealed class DocentesController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Docentes.AsNoTracking()
            .Include(d => d.Persona)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Legajo.Contains(texto) || 
                (x.Titulo != null && x.Titulo.Contains(texto)) ||
                x.Persona.Nombre.Contains(texto) || 
                x.Persona.Apellido.Contains(texto) ||
                x.Persona.Dni.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Persona.Apellido)
            .ThenBy(x => x.Persona.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { docentes = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var docente = await db.Docentes.AsNoTracking()
            .Include(d => d.Persona)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (docente is null) return NotFound(new { codigo = "docente_no_encontrado", mensaje = "El docente no existe." });
        return Ok(new { docente });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarDocenteRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var personaExiste = await db.Personas.AnyAsync(x => x.Id == request.PersonaId, cancellationToken);
        if (!personaExiste) return BadRequest(new { codigo = "persona_no_existe", mensaje = "La persona seleccionada no existe." });

        var legajoExiste = await db.Docentes.AnyAsync(x => x.Legajo.ToLower() == request.Legajo.Trim().ToLower(), cancellationToken);
        if (legajoExiste) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe un docente con el mismo legajo." });

        var docente = new Docente();
        Aplicar(docente, request);
        db.Docentes.Add(docente);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(docente).Reference(d => d.Persona).LoadAsync(cancellationToken);

        return Ok(new { docente = Proyectar(docente) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarDocenteRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var docente = await db.Docentes
            .Include(d => d.Persona)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (docente is null) return NotFound(new { codigo = "docente_no_encontrado", mensaje = "El docente no existe." });

        var legajoExiste = await db.Docentes.AnyAsync(x => x.Legajo.ToLower() == request.Legajo.Trim().ToLower() && x.Id != id, cancellationToken);
        if (legajoExiste) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe otro docente con el mismo legajo." });

        Aplicar(docente, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { docente = Proyectar(docente) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var docente = await db.Docentes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (docente is null) return NotFound(new { codigo = "docente_no_encontrado", mensaje = "El docente no existe." });

        var tieneAsignaciones = await db.AsignacionesDocentes.AnyAsync(x => x.DocenteId == id, cancellationToken);
        if (tieneAsignaciones) {
            return BadRequest(new { codigo = "docente_con_asignaciones", mensaje = "No se puede eliminar el docente porque tiene asignaciones a materias." });
        }

        db.Docentes.Remove(docente);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarDocenteRequest request) {
        if (request.PersonaId <= 0) return new { codigo = "persona_requerida", mensaje = "Debe seleccionar una persona válida." };
        if (string.IsNullOrWhiteSpace(request.Legajo)) return new { codigo = "legajo_requerido", mensaje = "El legajo es obligatorio." };
        return null;
    }

    private static void Aplicar(Docente docente, GuardarDocenteRequest request) {
        docente.PersonaId = request.PersonaId;
        docente.Legajo = request.Legajo.Trim();
        docente.Titulo = string.IsNullOrWhiteSpace(request.Titulo) ? null : request.Titulo.Trim();
    }

    private static DocenteResponse Proyectar(Docente x) =>
        new(
            x.Id,
            x.PersonaId,
            x.Legajo,
            x.Titulo,
            x.Persona?.Nombre ?? string.Empty,
            x.Persona?.Apellido ?? string.Empty,
            x.Persona?.Dni ?? string.Empty,
            x.Persona?.Telefono,
            x.Persona?.Direccion,
            x.Persona?.FechaNacimiento
        );
}

public sealed record DocenteResponse(
    int Id,
    int PersonaId,
    string Legajo,
    string? Titulo,
    string Nombre,
    string Apellido,
    string Dni,
    string? Telefono,
    string? Direccion
,
    DateOnly? fechaNacimiento);

public sealed record GuardarDocenteRequest(
    int PersonaId,
    string Legajo,
    string? Titulo
);