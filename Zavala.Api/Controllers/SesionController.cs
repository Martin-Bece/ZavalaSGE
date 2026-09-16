using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Auth;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities; 

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/sesion")]
public sealed class SesionController(ZavalaAppContext db, TokenService tokens) : ControllerBase {
    private static readonly PasswordHasher<Usuario> Hasher = new();

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken) {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        var usuario = await db.Usuarios
            .Include(u => u.Persona)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (usuario is null || usuario.Estado != true)
            return Unauthorized(new { codigo = "credenciales_invalidas", mensaje = "Email o contraseña incorrectos." });

        var verificacion = Hasher.VerifyHashedPassword(usuario, usuario.Password, request.Password ?? string.Empty);
        if (verificacion == PasswordVerificationResult.Failed) 
            return Unauthorized(new { codigo = "credenciales_invalidas", mensaje = "Email o contraseña incorrectos." });

        return Ok(new { sesion = await EmitirSesionAsync(usuario, cancellationToken) });
    }

    [Authorize]
    [HttpGet("yo")]
    public async Task<IActionResult> Yo(CancellationToken cancellationToken) {
        var usuarioId = User.UsuarioId();
        var usuario = await db.Usuarios
            .Include(u => u.Persona)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == usuarioId, cancellationToken);

        if (usuario is null) 
            return NotFound(new { codigo = "usuario_no_encontrado", mensaje = "El usuario no existe." });

        return Ok(new { 
            usuario = new SesionUsuarioResponse(
                usuario.Id, 
                usuario.Persona?.Nombre ?? string.Empty, 
                usuario.Persona?.Apellido ?? string.Empty, 
                usuario.Email, 
                usuario.Persona?.Telefono, 
                usuario.Rol, 
                usuario.Rol
            ) 
        });
    }

    private async Task<SesionResponse> EmitirSesionAsync(Usuario usuario, CancellationToken cancellationToken) {
        var (token, expiraEn) = tokens.Emitir(usuario, usuario.Rol);
        var (refresh, refreshHash, refreshExpiraEn) = tokens.EmitirRefresh();

        // Si tenés la tabla RefreshToken mapeada la guardás acá
        // db.RefreshTokens.Add(new RefreshToken { UsuarioId = usuario.Id, TokenHash = refreshHash, ExpiraEn = refreshExpiraEn });
        // await db.SaveChangesAsync(cancellationToken);

        return new SesionResponse(
            token,
            expiraEn,
            refresh,
            refreshExpiraEn,
            new SesionUsuarioResponse(
                usuario.Id, 
                usuario.Persona?.Nombre ?? string.Empty, 
                usuario.Persona?.Apellido ?? string.Empty, 
                usuario.Email, 
                usuario.Persona?.Telefono, 
                usuario.Rol, 
                usuario.Rol
            )
        );
    }
}

public sealed record SesionResponse(string Token, DateTimeOffset ExpiraEn, string RefreshToken, DateTimeOffset RefreshExpiraEn, SesionUsuarioResponse Usuario);
public sealed record SesionUsuarioResponse(long Id, string Nombre, string Apellido, string Email, string? Telefono, string? RolId, string? RolCodigo);
public sealed record LoginRequest(string Email, string Password);