using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/asignaciones-docentes")]
[Authorize]
public sealed class AsignacionesDocentesController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.AsignacionesDocentes.AsNoTracking()
            .Include(a => a.Docente).ThenInclude(d => d.Persona)
            .Include(a => a.Materia)
            .Include(a => a.Comision)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Docente.Persona.Nombre.Contains(texto) || 
                x.Docente.Persona.Apellido.Contains(texto) || 
                x.Materia.Nombre.Contains(texto) || 
                x.Comision.Nombre.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Materia.Nombre)
            .ThenBy(x => x.Docente.Persona.Apellido)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { asignaciones = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var asignacion = await db.AsignacionesDocentes.AsNoTracking()
            .Include(a => a.Docente).ThenInclude(d => d.Persona)
            .Include(a => a.Materia)
            .Include(a => a.Comision)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (asignacion is null) return NotFound(new { codigo = "asignacion_no_encontrada", mensaje = "La asignación no existe." });
        return Ok(new { asignacion });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarAsignacionDocenteRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var docenteExiste = await db.Docentes.AnyAsync(x => x.Id == request.DocenteId, cancellationToken);
        if (!docenteExiste) return BadRequest(new { codigo = "docente_no_existe", mensaje = "El docente seleccionado no existe." });

        var materiaExiste = await db.Materias.AnyAsync(x => x.Id == request.MateriaId, cancellationToken);
        if (!materiaExiste) return BadRequest(new { codigo = "materia_no_existe", mensaje = "La materia seleccionada no existe." });

        var comisionExiste = await db.Comisiones.AnyAsync(x => x.Id == request.ComisionId, cancellationToken);
        if (!comisionExiste) return BadRequest(new { codigo = "comision_no_existe", mensaje = "La comisión seleccionada no existe." });

        var duplicado = await db.AsignacionesDocentes.AnyAsync(x => 
            x.DocenteId == request.DocenteId && 
            x.MateriaId == request.MateriaId && 
            x.ComisionId == request.ComisionId, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "asignacion_duplicada", mensaje = "El docente ya está asignado a esta materia y comisión." });

        var asignacion = new AsignacionesDocente();
        Aplicar(asignacion, request);
        db.AsignacionesDocentes.Add(asignacion);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(asignacion).Reference(a => a.Docente).Query().Include(d => d.Persona).LoadAsync(cancellationToken);
        await db.Entry(asignacion).Reference(a => a.Materia).LoadAsync(cancellationToken);
        await db.Entry(asignacion).Reference(a => a.Comision).LoadAsync(cancellationToken);

        return Ok(new { asignacion = Proyectar(asignacion) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarAsignacionDocenteRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var asignacion = await db.AsignacionesDocentes
            .Include(a => a.Docente).ThenInclude(d => d.Persona)
            .Include(a => a.Materia)
            .Include(a => a.Comision)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (asignacion is null) return NotFound(new { codigo = "asignacion_no_encontrada", mensaje = "La asignación no existe." });

        var duplicado = await db.AsignacionesDocentes.AnyAsync(x => 
            x.DocenteId == request.DocenteId && 
            x.MateriaId == request.MateriaId && 
            x.ComisionId == request.ComisionId && 
            x.Id != id, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "asignacion_duplicada", mensaje = "El docente ya está asignado a esta materia y comisión." });

        Aplicar(asignacion, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { asignacion = Proyectar(asignacion) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var asignacion = await db.AsignacionesDocentes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (asignacion is null) return NotFound(new { codigo = "asignacion_no_encontrada", mensaje = "La asignación no existe." });

        db.AsignacionesDocentes.Remove(asignacion);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarAsignacionDocenteRequest request) {
        if (request.DocenteId <= 0) return new { codigo = "docente_requerido", mensaje = "Debe seleccionar un docente válido." };
        if (request.MateriaId <= 0) return new { codigo = "materia_requerida", mensaje = "Debe seleccionar una materia válida." };
        if (request.ComisionId <= 0) return new { codigo = "comision_requerida", mensaje = "Debe seleccionar una comisión válida." };
        return null;
    }

    private static void Aplicar(AsignacionesDocente asignacion, GuardarAsignacionDocenteRequest request) {
        asignacion.DocenteId = request.DocenteId;
        asignacion.MateriaId = request.MateriaId;
        asignacion.ComisionId = request.ComisionId;
    }

    private static AsignacionDocenteResponse Proyectar(AsignacionesDocente x) =>
        new(
            x.Id,
            x.DocenteId,
            x.MateriaId,
            x.ComisionId,
            x.Docente?.Persona != null ? $"{x.Docente.Persona.Apellido}, {x.Docente.Persona.Nombre}" : string.Empty,
            x.Materia?.Nombre ?? string.Empty,
            x.Comision?.Nombre ?? string.Empty
        );
}

public sealed record AsignacionDocenteResponse(
    int Id,
    int DocenteId,
    int MateriaId,
    int ComisionId,
    string DocenteNombre,
    string MateriaNombre,
    string ComisionNombre
);

public sealed record GuardarAsignacionDocenteRequest(
    int DocenteId,
    int MateriaId,
    int ComisionId
);