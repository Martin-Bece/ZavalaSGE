using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/materias")]
[Authorize]
public sealed class MateriasController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Materias.AsNoTracking().AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Nombre.Contains(texto) || 
                (x.Codigo != null && x.Codigo.Contains(texto)));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { materias = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var materia = await db.Materias.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (materia is null) return NotFound(new { codigo = "materia_no_encontrada", mensaje = "La materia no existe." });
        return Ok(new { materia });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarMateriaRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var duplicado = await db.Materias.AnyAsync(x => x.Nombre.ToLower() == request.Nombre.Trim().ToLower(), cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "materia_duplicada", mensaje = "Ya existe una materia con ese nombre." });

        if (!string.IsNullOrWhiteSpace(request.Codigo)) {
            var codigoExiste = await db.Materias.AnyAsync(x => x.Codigo!.ToLower() == request.Codigo.Trim().ToLower(), cancellationToken);
            if (codigoExiste) return BadRequest(new { codigo = "codigo_duplicado", mensaje = "Ya existe una materia con ese código." });
        }

        var materia = new Materia();
        Aplicar(materia, request);
        db.Materias.Add(materia);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new { materia = Proyectar(materia) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarMateriaRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var materia = await db.Materias.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (materia is null) return NotFound(new { codigo = "materia_no_encontrada", mensaje = "La materia no existe." });

        var duplicado = await db.Materias.AnyAsync(x => x.Nombre.ToLower() == request.Nombre.Trim().ToLower() && x.Id != id, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "materia_duplicada", mensaje = "Ya existe otra materia con ese nombre." });

        if (!string.IsNullOrWhiteSpace(request.Codigo)) {
            var codigoExiste = await db.Materias.AnyAsync(x => x.Codigo!.ToLower() == request.Codigo.Trim().ToLower() && x.Id != id, cancellationToken);
            if (codigoExiste) return BadRequest(new { codigo = "codigo_duplicado", mensaje = "Ya existe otra materia con ese código." });
        }

        Aplicar(materia, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { materia = Proyectar(materia) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var materia = await db.Materias.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (materia is null) return NotFound(new { codigo = "materia_no_encontrada", mensaje = "La materia no existe." });

        var tieneAsignaciones = await db.AsignacionesDocentes.AnyAsync(x => x.MateriaId == id, cancellationToken);
        var tieneCalificaciones = await db.Calificaciones.AnyAsync(x => x.MateriaId == id, cancellationToken);

        if (tieneAsignaciones || tieneCalificaciones) {
            return BadRequest(new { codigo = "materia_con_registros", mensaje = "No se puede eliminar la materia porque tiene registros asociados." });
        }

        db.Materias.Remove(materia);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarMateriaRequest request) {
        if (string.IsNullOrWhiteSpace(request.Nombre)) return new { codigo = "nombre_requerido", mensaje = "El nombre de la materia es obligatorio." };
        return null;
    }

    private static void Aplicar(Materia materia, GuardarMateriaRequest request) {
        materia.Nombre = request.Nombre.Trim();
        materia.Codigo = string.IsNullOrWhiteSpace(request.Codigo) ? null : request.Codigo.Trim();
    }

    private static MateriaResponse Proyectar(Materia x) =>
        new(
            x.Id,
            x.Nombre,
            x.Codigo
        );
}

public sealed record MateriaResponse(
    int Id,
    string Nombre,
    string? Codigo
);

public sealed record GuardarMateriaRequest(
    string Nombre,
    string? Codigo
);