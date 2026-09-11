using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/alumnos")]
public sealed class AlumnosController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Alumnos.AsNoTracking().Include(a => a.Persona).AsQueryable();
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

        return Ok(new { alumnos = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var alumno = await db.Alumnos.AsNoTracking()
            .Include(a => a.Persona)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (alumno is null) return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });
        return Ok(new { alumno });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarAlumnoRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var legajo = request.Legajo.Trim();
        var duplicadoLegajo = await db.Alumnos.AnyAsync(x => x.Legajo == legajo, cancellationToken);
        if (duplicadoLegajo) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe un alumno con ese legajo." });

        var personaExiste = await db.Personas.AnyAsync(x => x.Id == request.PersonaId, cancellationToken);
        if (!personaExiste) return BadRequest(new { codigo = "persona_no_existe", mensaje = "La persona seleccionada no existe." });

        var duplicadoPersona = await db.Alumnos.AnyAsync(x => x.PersonaId == request.PersonaId, cancellationToken);
        if (duplicadoPersona) return BadRequest(new { codigo = "persona_ya_es_alumno", mensaje = "Esta persona ya está registrada como alumno." });

        var alumno = new Alumno();
        Aplicar(alumno, request);
        db.Alumnos.Add(alumno);
        await db.SaveChangesAsync(cancellationToken);

        // Recargamos navegación para devolver la proyección completa
        await db.Entry(alumno).Reference(a => a.Persona).LoadAsync(cancellationToken);
        return Ok(new { alumno = Proyectar(alumno) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarAlumnoRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var alumno = await db.Alumnos.Include(a => a.Persona).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (alumno is null) return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });

        var legajo = request.Legajo.Trim();
        var duplicadoLegajo = await db.Alumnos.AnyAsync(x => x.Legajo == legajo && x.Id != id, cancellationToken);
        if (duplicadoLegajo) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe otro alumno con ese legajo." });

        Aplicar(alumno, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { alumno = Proyectar(alumno) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var alumno = await db.Alumnos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (alumno is null) return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });

        db.Alumnos.Remove(alumno);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarAlumnoRequest request) {
        if (string.IsNullOrWhiteSpace(request.Legajo)) return new { codigo = "legajo_requerido", mensaje = "El legajo es obligatorio." };
        if (request.PersonaId <= 0) return new { codigo = "persona_requerida", mensaje = "Debe seleccionar una persona válida." };
        return null;
    }

    private static void Aplicar(Alumno alumno, GuardarAlumnoRequest request) {
        alumno.Legajo = request.Legajo.Trim();
        alumno.PersonaId = request.PersonaId;
    }

    private static AlumnoResponse Proyectar(Alumno x) =>
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

public sealed record AlumnoResponse(
    int Id,
    int PersonaId,
    string Legajo,
    string Nombre,
    string Apellido,
    string Dni,
    string? Telefono,
    string? Direccion,
    DateOnly? FechaNacimiento
);

public sealed record GuardarAlumnoRequest(
    string Legajo,
    int PersonaId
);