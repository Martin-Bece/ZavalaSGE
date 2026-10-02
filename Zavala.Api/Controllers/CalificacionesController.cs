using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Zavala.Api.Auth;
using System.Globalization;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/calificaciones")]
[Authorize]
public sealed class CalificacionesController(ZavalaAppContext db) : ControllerBase
{

    [HttpGet]
    [Authorize(Roles = RolCodigos.Admin)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    )
    {
        var consulta = db.Calificaciones.AsNoTracking()
            .Include(c => c.Alumno).ThenInclude(a => a.Persona)
            .Include(c => c.Materia)
            .Include(c => c.Comision)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0)
        {
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
    [Authorize(Roles = RolCodigos.Admin)]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
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
    public async Task<IActionResult> Crear(GuardarCalificacionRequest request, CancellationToken cancellationToken)
    {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        // Solo el docente con esa materia en esa comisión (o el admin).
        if (!await db.PuedeCalificarAsync(User, request.ComisionId, request.MateriaId, cancellationToken))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No podés calificar esta materia en esta comisión." });

        var alumnoExiste = await db.Alumnos.AnyAsync(x => x.Id == request.AlumnoId, cancellationToken);
        if (!alumnoExiste) return BadRequest(new { codigo = "alumno_no_existe", mensaje = "El alumno seleccionado no existe." });

        var materiaExiste = await db.Materias.AnyAsync(x => x.Id == request.MateriaId, cancellationToken);
        if (!materiaExiste) return BadRequest(new { codigo = "materia_no_existe", mensaje = "La materia seleccionada no existe." });

        var comisionExiste = await db.Comisiones.AnyAsync(x => x.Id == request.ComisionId, cancellationToken);
        if (!comisionExiste) return BadRequest(new { codigo = "comision_no_existe", mensaje = "La comisión seleccionada no existe." });

        var inscripto = await db.Inscripciones.AnyAsync(
            i => i.AlumnoId == request.AlumnoId && i.ComisionId == request.ComisionId, cancellationToken);
        if (!inscripto)
            return BadRequest(new { codigo = "alumno_no_inscripto", mensaje = "El alumno no está inscripto en esta comisión." });

        var calificacion = new Calificacione();
        Aplicar(calificacion, request);
        db.Calificaciones.Add(calificacion);
        await NotificarNotaAsync(calificacion, true, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(calificacion).Reference(c => c.Alumno).Query().Include(a => a.Persona).LoadAsync(cancellationToken);
        await db.Entry(calificacion).Reference(c => c.Materia).LoadAsync(cancellationToken);
        await db.Entry(calificacion).Reference(c => c.Comision).LoadAsync(cancellationToken);

        return Ok(new { calificacion = Proyectar(calificacion) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarCalificacionRequest request, CancellationToken cancellationToken)
    {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var calificacion = await db.Calificaciones
            .Include(c => c.Alumno).ThenInclude(a => a.Persona)
            .Include(c => c.Materia)
            .Include(c => c.Comision)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (calificacion is null) return NotFound(new { codigo = "calificacion_no_encontrada", mensaje = "La calificación no existe." });

        // Permiso sobre la nota actual y sobre el destino: el request puede cambiar materia o comisión.
        var puede = await db.PuedeCalificarAsync(User, calificacion.ComisionId, calificacion.MateriaId, cancellationToken)
            && await db.PuedeCalificarAsync(User, request.ComisionId, request.MateriaId, cancellationToken);
        if (!puede)
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No podés modificar esta calificación." });

        var inscripto = await db.Inscripciones.AnyAsync(
            i => i.AlumnoId == request.AlumnoId && i.ComisionId == request.ComisionId, cancellationToken);
        if (!inscripto)
            return BadRequest(new { codigo = "alumno_no_inscripto", mensaje = "El alumno no está inscripto en esta comisión." });

        Aplicar(calificacion, request);
        await NotificarNotaAsync(calificacion, false, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { calificacion = Proyectar(calificacion) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var calificacion = await db.Calificaciones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (calificacion is null) return NotFound(new { codigo = "calificacion_no_encontrada", mensaje = "La calificación no existe." });

        if (!await db.PuedeCalificarAsync(User, calificacion.ComisionId, calificacion.MateriaId, cancellationToken))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No podés borrar esta calificación." });

        db.Calificaciones.Remove(calificacion);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarCalificacionRequest request)
    {
        if (request.AlumnoId <= 0) return new { codigo = "alumno_requerido", mensaje = "Debe seleccionar un alumno válido." };
        if (request.MateriaId <= 0) return new { codigo = "materia_requerida", mensaje = "Debe seleccionar una materia válida." };
        if (request.ComisionId <= 0) return new { codigo = "comision_requerida", mensaje = "Debe seleccionar una comisión válida." };
        if (request.Nota < 1 || request.Nota > 10) return new { codigo = "nota_invalida", mensaje = "La nota debe estar entre 1 y 10." };
        if (string.IsNullOrWhiteSpace(request.Instancia)) return new { codigo = "instancia_requerida", mensaje = "La instancia de evaluación es obligatoria." };
        return null;
    }

    private static void Aplicar(Calificacione calificacion, GuardarCalificacionRequest request)
    {
        calificacion.AlumnoId = request.AlumnoId;
        calificacion.MateriaId = request.MateriaId;
        calificacion.ComisionId = request.ComisionId;
        calificacion.Nota = request.Nota;
        calificacion.Instancia = request.Instancia.Trim();
        calificacion.Fecha = request.Fecha ?? calificacion.Fecha ?? DateOnly.FromDateTime(DateTime.Today);
    }

    // Avisa al preceptor de la comisión (el interesado), nunca al docente que cargó la nota.
    // No guarda: viaja en el mismo SaveChanges que la nota, así pasan las dos cosas o ninguna.
    private async Task NotificarNotaAsync(Calificacione nota, bool nueva, CancellationToken cancellationToken)
    {
        var actorId = (int)User.UsuarioId();

        var comision = await db.Comisiones.AsNoTracking()
            .Where(c => c.Id == nota.ComisionId)
            .Select(c => new { c.Nombre, c.PreceptorId })
            .FirstAsync(cancellationToken);

        var preceptorUsuarioId = await db.UsuarioIdDePreceptorAsync(comision.PreceptorId, cancellationToken);
        if (preceptorUsuarioId is null || preceptorUsuarioId == actorId) return;

        var actor = await db.NombreDeUsuarioAsync(actorId, cancellationToken);
        var alumno = await db.Alumnos.AsNoTracking()
            .Where(a => a.Id == nota.AlumnoId)
            .Select(a => a.Persona.Apellido + ", " + a.Persona.Nombre)
            .FirstAsync(cancellationToken);
        var materia = await db.Materias.AsNoTracking()
            .Where(m => m.Id == nota.MateriaId)
            .Select(m => m.Nombre)
            .FirstAsync(cancellationToken);

        var titulo = nueva ? "Nota cargada" : "Nota modificada";
        var verbo = nueva ? "cargó" : "modificó";
        var valor = nota.Nota.ToString("0.##", new CultureInfo("es-AR"));

        db.Notificar(preceptorUsuarioId.Value,
            $"{titulo} · {materia}",
            $"{actor} {verbo} {valor} ({nota.Instancia}) a {alumno} en {comision.Nombre}.");
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