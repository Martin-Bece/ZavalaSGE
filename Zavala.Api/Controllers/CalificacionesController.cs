using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/calificaciones")]
public sealed class CalificacionesController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Calificaciones.AsNoTracking()
            .Include(c => c.Alumno).ThenInclude(a => a.Persona)
            .Include(c => c.Materia)
            .Include(c => c.Comision)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Alumno.Persona.Nombre.Contains(texto) || 
                x.Alumno.Persona.Apellido.Contains(texto) || 
                x.Alumno.Legajo.Contains(texto) || 
                x.Materia.Nombre.Contains(texto) || 
                x.Comision.Nombre.Contains(texto) ||
                x.Instancia.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderByDescending(x => x.Fecha)
            .ThenBy(x => x.Alumno.Persona.Apellido)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { calificaciones = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var calificacion = await db.Calificaciones.AsNoTracking()
            .Include(c => c.Alumno).ThenInclude(a => a.Persona)
            .Include(c => c.Materia)
            .Include(c => c.Comision)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (calificacion is null) return NotFound(new { codigo = "calificacion_no_encontrada", mensaje = "La calificación no existe." });
        return Ok(new { calificacion });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarCalificacionRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var alumnoExiste = await db.Alumnos.AnyAsync(x => x.Id == request.AlumnoId, cancellationToken);
        if (!alumnoExiste) return BadRequest(new { codigo = "alumno_no_existe", mensaje = "El alumno seleccionado no existe." });

        var materiaExiste = await db.Materias.AnyAsync(x => x.Id == request.MateriaId, cancellationToken);
        if (!materiaExiste) return BadRequest(new { codigo = "materia_no_existe", mensaje = "La materia seleccionada no existe." });

        var comisionExiste = await db.Comisiones.AnyAsync(x => x.Id == request.ComisionId, cancellationToken);
        if (!comisionExiste) return BadRequest(new { codigo = "comision_no_existe", mensaje = "La comisión seleccionada no existe." });

        var calificacion = new Calificacione();
        Aplicar(calificacion, request);
        db.Calificaciones.Add(calificacion);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(calificacion).Reference(c => c.Alumno).Query().Include(a => a.Persona).LoadAsync(cancellationToken);
        await db.Entry(calificacion).Reference(c => c.Materia).LoadAsync(cancellationToken);
        await db.Entry(calificacion).Reference(c => c.Comision).LoadAsync(cancellationToken);

        return Ok(new { calificacion = Proyectar(calificacion) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarCalificacionRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var calificacion = await db.Calificaciones
            .Include(c => c.Alumno).ThenInclude(a => a.Persona)
            .Include(c => c.Materia)
            .Include(c => c.Comision)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (calificacion is null) return NotFound(new { codigo = "calificacion_no_encontrada", mensaje = "La calificación no existe." });

        Aplicar(calificacion, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { calificacion = Proyectar(calificacion) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var calificacion = await db.Calificaciones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (calificacion is null) return NotFound(new { codigo = "calificacion_no_encontrada", mensaje = "La calificación no existe." });

        db.Calificaciones.Remove(calificacion);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarCalificacionRequest request) {
        if (request.AlumnoId <= 0) return new { codigo = "alumno_requerido", mensaje = "Debe seleccionar un alumno válido." };
        if (request.MateriaId <= 0) return new { codigo = "materia_requerida", mensaje = "Debe seleccionar una materia válida." };
        if (request.ComisionId <= 0) return new { codigo = "comision_requerida", mensaje = "Debe seleccionar una comisión válida." };
        if (request.Nota < 1 || request.Nota > 10) return new { codigo = "nota_invalida", mensaje = "La nota debe estar entre 1 y 10." };
        if (string.IsNullOrWhiteSpace(request.Instancia)) return new { codigo = "instancia_requerida", mensaje = "La instancia de evaluación es obligatoria." };
        return null;
    }

    private static void Aplicar(Calificacione calificacion, GuardarCalificacionRequest request) {
        calificacion.AlumnoId = request.AlumnoId;
        calificacion.MateriaId = request.MateriaId;
        calificacion.ComisionId = request.ComisionId;
        calificacion.Nota = request.Nota;
        calificacion.Instancia = request.Instancia.Trim();
        calificacion.Fecha = request.Fecha;
    }

    private static CalificacionResponse Proyectar(Calificacione x) =>
        new(
            x.Id,
            x.AlumnoId,
            x.MateriaId,
            x.ComisionId,
            x.Nota,
            x.Instancia,
            x.Fecha,
            x.Alumno?.Persona != null ? $"{x.Alumno.Persona.Apellido}, {x.Alumno.Persona.Nombre}" : string.Empty,
            x.Alumno?.Legajo ?? string.Empty,
            x.Materia?.Nombre ?? string.Empty,
            x.Comision?.Nombre ?? string.Empty
        );
}

public sealed record CalificacionResponse(
    int Id,
    int AlumnoId,
    int MateriaId,
    int ComisionId,
    decimal Nota,
    string Instancia,
    DateOnly? Fecha,
    string AlumnoNombre,
    string AlumnoLegajo,
    string MateriaNombre,
    string ComisionNombre
);

public sealed record GuardarCalificacionRequest(
    int AlumnoId,
    int MateriaId,
    int ComisionId,
    decimal Nota,
    string Instancia,
    DateOnly? Fecha
);