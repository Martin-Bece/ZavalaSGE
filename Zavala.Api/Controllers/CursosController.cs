using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/cursos")]
public sealed class CursosController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Cursos.AsNoTracking().AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Nombre.Contains(texto) || 
                x.CicloLectivo.ToString().Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderByDescending(x => x.CicloLectivo)
            .ThenBy(x => x.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { cursos = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var curso = await db.Cursos.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (curso is null) return NotFound(new { codigo = "curso_no_encontrado", mensaje = "El curso no existe." });
        return Ok(new { curso });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarCursoRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var duplicado = await db.Cursos.AnyAsync(x => 
            x.Nombre.ToLower() == request.Nombre.Trim().ToLower() && 
            x.CicloLectivo == request.CicloLectivo, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "curso_duplicado", mensaje = "Ya existe un curso con el mismo nombre y ciclo lectivo." });

        var curso = new Curso();
        Aplicar(curso, request);
        db.Cursos.Add(curso);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new { curso = Proyectar(curso) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarCursoRequest request, CancellationToken cancellationToken) {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var curso = await db.Cursos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (curso is null) return NotFound(new { codigo = "curso_no_encontrado", mensaje = "El curso no existe." });

        var duplicado = await db.Cursos.AnyAsync(x => 
            x.Nombre.ToLower() == request.Nombre.Trim().ToLower() && 
            x.CicloLectivo == request.CicloLectivo && 
            x.Id != id, cancellationToken);
        if (duplicado) return BadRequest(new { codigo = "curso_duplicado", mensaje = "Ya existe otro curso con el mismo nombre y ciclo lectivo." });

        Aplicar(curso, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { curso = Proyectar(curso) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var curso = await db.Cursos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (curso is null) return NotFound(new { codigo = "curso_no_encontrado", mensaje = "El curso no existe." });

        var tieneComisiones = await db.Comisiones.AnyAsync(x => x.CursoId == id, cancellationToken);
        if (tieneComisiones) {
            return BadRequest(new { codigo = "curso_con_comisiones", mensaje = "No se puede eliminar el curso porque tiene comisiones asociadas." });
        }

        db.Cursos.Remove(curso);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarCursoRequest request) {
        if (string.IsNullOrWhiteSpace(request.Nombre)) return new { codigo = "nombre_requerido", mensaje = "El nombre del curso es obligatorio." };
        if (request.CicloLectivo < 1900 || request.CicloLectivo > 2100) return new { codigo = "ciclo_lectivo_invalido", mensaje = "El ciclo lectivo ingresado no es válido." };
        return null;
    }

    private static void Aplicar(Curso curso, GuardarCursoRequest request) {
        curso.Nombre = request.Nombre.Trim();
        curso.CicloLectivo = request.CicloLectivo;
    }

    private static CursoResponse Proyectar(Curso x) =>
        new(
            x.Id,
            x.Nombre,
            x.CicloLectivo
        );
}

public sealed record CursoResponse(
    int Id,
    string Nombre,
    int CicloLectivo
);

public sealed record GuardarCursoRequest(
    string Nombre,
    int CicloLectivo
);