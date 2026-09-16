using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/preceptores")]
[Authorize]
public sealed class PreceptoresController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Preceptores.AsNoTracking()
            .Include(p => p.Persona)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Legajo.Contains(texto) || 
                x.Persona.Nombre.Contains(texto) || 
                x.Persona.Apellido.Contains(texto) ||
                x.Persona.Dni.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Persona.Apellido)
            .ThenBy(x => x.Persona.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { preceptores = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var preceptor = await db.Preceptores.AsNoTracking()
            .Include(p => p.Persona)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (preceptor is null) return NotFound(new { codigo = "preceptor_no_encontrado", mensaje = "El preceptor no existe." });
        return Ok(new { preceptor });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarPreceptorRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var personaExiste = await db.Personas.AnyAsync(x => x.Id == request.PersonaId, cancellationToken);
        if (!personaExiste) return BadRequest(new { codigo = "persona_no_existe", mensaje = "La persona seleccionada no existe." });

        var legajoExiste = await db.Preceptores.AnyAsync(x => x.Legajo.ToLower() == request.Legajo.Trim().ToLower(), cancellationToken);
        if (legajoExiste) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe un preceptor con el mismo legajo." });

        var preceptor = new Preceptore();
        Aplicar(preceptor, request);
        db.Preceptores.Add(preceptor);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(preceptor).Reference(p => p.Persona).LoadAsync(cancellationToken);

        return Ok(new { preceptor = Proyectar(preceptor) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarPreceptorRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var preceptor = await db.Preceptores
            .Include(p => p.Persona)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (preceptor is null) return NotFound(new { codigo = "preceptor_no_encontrado", mensaje = "El preceptor no existe." });

        var legajoExiste = await db.Preceptores.AnyAsync(x => x.Legajo.ToLower() == request.Legajo.Trim().ToLower() && x.Id != id, cancellationToken);
        if (legajoExiste) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe otro preceptor con el mismo legajo." });

        Aplicar(preceptor, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { preceptor = Proyectar(preceptor) });
    }

    [HttpDelete("{id:int}")]
public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
    var persona = await db.Personas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (persona is null) return NotFound(new { codigo = "persona_no_encontrada", mensaje = "La persona no existe." });

    db.Personas.Remove(persona);
    await db.SaveChangesAsync(cancellationToken);
    return Ok(new { eliminado = true });
}

    private static object? Validar(GuardarPreceptorRequest request) {
        if (request.PersonaId <= 0) return new { codigo = "persona_requerida", mensaje = "Debe seleccionar una persona válida." };
        if (string.IsNullOrWhiteSpace(request.Legajo)) return new { codigo = "legajo_requerido", mensaje = "El legajo es obligatorio." };
        return null;
    }

    private static void Aplicar(Preceptore preceptor, GuardarPreceptorRequest request) {
        preceptor.PersonaId = request.PersonaId;
        preceptor.Legajo = request.Legajo.Trim();
    }

    private static PreceptorResponse Proyectar(Preceptore x) =>
        new(
            x.Id,
            x.PersonaId,
            x.Legajo,
            x.Persona?.Nombre ?? string.Empty,
            x.Persona?.Apellido ?? string.Empty,
            x.Persona?.Dni ?? string.Empty,
            x.Persona?.Telefono,
            x.Persona?.Direccion,
            x.Persona?.FechaNacimiento
        );
}

public sealed record PreceptorResponse(
    int Id,
    int PersonaId,
    string Legajo,
    string Nombre,
    string Apellido,
    string Dni,
    string? Telefono,
    string? Direccion
,
    DateOnly? fechaNacimiento);

public sealed record GuardarPreceptorRequest(
    int PersonaId,
    string Legajo
);