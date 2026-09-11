using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/personas")]
public sealed class PersonasController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Personas.AsNoTracking().AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Nombre.Contains(texto) || 
                x.Apellido.Contains(texto) || 
                x.Dni.Contains(texto) ||
                (x.Telefono != null && x.Telefono.Contains(texto)));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Apellido)
            .ThenBy(x => x.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { personas = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var persona = await db.Personas.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (persona is null) return NotFound(new { codigo = "persona_no_encontrada", mensaje = "La persona no existe." });
        return Ok(new { persona });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarPersonaRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var dniExiste = await db.Personas.AnyAsync(x => x.Dni.Trim() == request.Dni.Trim(), cancellationToken);
        if (dniExiste) return BadRequest(new { codigo = "dni_duplicado", mensaje = "Ya existe una persona registrada con el mismo DNI." });

        var persona = new Persona();
        Aplicar(persona, request);
        db.Personas.Add(persona);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new { persona = Proyectar(persona) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarPersonaRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var persona = await db.Personas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (persona is null) return NotFound(new { codigo = "persona_no_encontrada", mensaje = "La persona no existe." });

        var dniExiste = await db.Personas.AnyAsync(x => x.Dni.Trim() == request.Dni.Trim() && x.Id != id, cancellationToken);
        if (dniExiste) return BadRequest(new { codigo = "dni_duplicado", mensaje = "Ya existe otra persona registrada con el mismo DNI." });

        Aplicar(persona, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { persona = Proyectar(persona) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var persona = await db.Personas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (persona is null) return NotFound(new { codigo = "persona_no_encontrada", mensaje = "La persona no existe." });

        db.Personas.Remove(persona);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarPersonaRequest request) {
        if (string.IsNullOrWhiteSpace(request.Dni)) return new { codigo = "dni_requerido", mensaje = "El DNI es obligatorio." };
        if (string.IsNullOrWhiteSpace(request.Nombre)) return new { codigo = "nombre_requerido", mensaje = "El nombre es obligatorio." };
        if (string.IsNullOrWhiteSpace(request.Apellido)) return new { codigo = "apellido_requerido", mensaje = "El apellido es obligatorio." };
        return null;
    }

    private static void Aplicar(Persona persona, GuardarPersonaRequest request) {
        persona.Dni = request.Dni.Trim();
        persona.Nombre = request.Nombre.Trim();
        persona.Apellido = request.Apellido.Trim();
        persona.Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : request.Telefono.Trim();
        persona.Direccion = string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim();
        persona.FechaNacimiento = request.FechaNacimiento;
    }

    private static PersonaResponse Proyectar(Persona x) =>
        new(
            x.Id,
            x.Dni,
            x.Nombre,
            x.Apellido,
            x.Telefono,
            x.Direccion,
            x.FechaNacimiento
        );
}

public sealed record PersonaResponse(
    int Id,
    string Dni,
    string Nombre,
    string Apellido,
    string? Telefono,
    string? Direccion,
    DateOnly? FechaNacimiento
);

public sealed record GuardarPersonaRequest(
    string Dni,
    string Nombre,
    string Apellido,
    string? Telefono,
    string? Direccion,
    DateOnly? FechaNacimiento
);