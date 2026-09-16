using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public sealed class UsuariosController(ZavalaAppContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        CancellationToken cancellationToken
    ) {
        var consulta = db.Usuarios.AsNoTracking()
            .Include(u => u.Persona)
            .AsQueryable();

        var texto = (busqueda ?? string.Empty).Trim();
        if (texto.Length > 0) {
            consulta = consulta.Where(x => 
                x.Email.Contains(texto) || 
                x.Rol.Contains(texto) || 
                x.Persona.Nombre.Contains(texto) || 
                x.Persona.Apellido.Contains(texto) ||
                x.Persona.Dni.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Persona.Apellido)
            .ThenBy(x => x.Persona.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { usuarios = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken) {
        var usuario = await db.Usuarios.AsNoTracking()
            .Include(u => u.Persona)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (usuario is null) return NotFound(new { codigo = "usuario_no_encontrado", mensaje = "El usuario no existe." });
        return Ok(new { usuario });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(GuardarUsuarioRequest request, CancellationToken cancellationToken) {
        var error = Validar(request, esCreacion: true);
        if (error is not null) return BadRequest(error);

        var personaExiste = await db.Personas.AnyAsync(x => x.Id == request.PersonaId, cancellationToken);
        if (!personaExiste) return BadRequest(new { codigo = "persona_no_existe", mensaje = "La persona seleccionada no existe." });

        var personaConUsuario = await db.Usuarios.AnyAsync(x => x.PersonaId == request.PersonaId, cancellationToken);
        if (personaConUsuario) return BadRequest(new { codigo = "persona_tiene_usuario", mensaje = "La persona seleccionada ya posee una cuenta de usuario asignada." });

        var emailExiste = await db.Usuarios.AnyAsync(x => x.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (emailExiste) return BadRequest(new { codigo = "email_duplicado", mensaje = "El correo electrónico ya está en uso." });

        var usuario = new Usuario();
        Aplicar(usuario, request);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(usuario).Reference(u => u.Persona).LoadAsync(cancellationToken);

        return Ok(new { usuario = Proyectar(usuario) });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, GuardarUsuarioRequest request, CancellationToken cancellationToken) {
        var error = Validar(request, esCreacion: false);
        if (error is not null) return BadRequest(error);

        var usuario = await db.Usuarios
            .Include(u => u.Persona)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (usuario is null) return NotFound(new { codigo = "usuario_no_encontrado", mensaje = "El usuario no existe." });

        var emailExiste = await db.Usuarios.AnyAsync(x => x.Email.ToLower() == request.Email.Trim().ToLower() && x.Id != id, cancellationToken);
        if (emailExiste) return BadRequest(new { codigo = "email_duplicado", mensaje = "El correo electrónico ya está en uso por otro usuario." });

        Aplicar(usuario, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { usuario = Proyectar(usuario) });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken) {
        var usuario = await db.Usuarios.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (usuario is null) return NotFound(new { codigo = "usuario_no_encontrado", mensaje = "El usuario no existe." });

        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarUsuarioRequest request, bool esCreacion) {
        if (request.PersonaId <= 0) return new { codigo = "persona_requerida", mensaje = "Debe seleccionar una persona válida." };
        if (string.IsNullOrWhiteSpace(request.Email)) return new { codigo = "email_requerido", mensaje = "El correo electrónico es obligatorio." };
        if (esCreacion && string.IsNullOrWhiteSpace(request.Password)) return new { codigo = "password_requerido", mensaje = "La contraseña es obligatoria." };
        if (string.IsNullOrWhiteSpace(request.Rol)) return new { codigo = "rol_requerido", mensaje = "El rol es obligatorio." };
        return null;
    }

    private static void Aplicar(Usuario usuario, GuardarUsuarioRequest request) {
        usuario.PersonaId = request.PersonaId;
        usuario.Email = request.Email.Trim();
        usuario.Rol = request.Rol.Trim();
        usuario.Estado = request.Estado ?? true;

        if (!string.IsNullOrWhiteSpace(request.Password)) {
            usuario.Password = request.Password; // Se recomienda aplicar BCrypt/Argon2 aquí según la arquitectura
        }
    }

    private static UsuarioResponse Proyectar(Usuario x) =>
        new(
            x.Id,
            x.PersonaId,
            x.Email,
            x.Rol,
            x.Estado ?? true,
            x.Persona != null ? $"{x.Persona.Apellido}, {x.Persona.Nombre}" : string.Empty,
            x.Persona?.Dni ?? string.Empty
        );
}

public sealed record UsuarioResponse(
    int Id,
    int PersonaId,
    string Email,
    string Rol,
    bool Estado,
    string PersonaNombre,
    string PersonaDni
);

public sealed record GuardarUsuarioRequest(
    int PersonaId,
    string Email,
    string? Password,
    string Rol,
    bool? Estado
);