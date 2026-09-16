using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Zavala.Api.Auth;

public static class ClaimsExtensions {
    public static long UsuarioId(this ClaimsPrincipal usuario) {
        var valor = usuario.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? usuario.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(valor, out var id) ? id : 0;
    }

    public static string? RolCodigo(this ClaimsPrincipal usuario) {
        return usuario.FindFirstValue(TokenService.ClaimRolCodigo);
    }
}

public static class RolCodigos {
    public const string Admin = "ADMIN";
    public const string Preceptor = "PRECEPTOR";
    public const string Docente = "DOCENTE";

    public const string Operativos = $"{Admin},{Preceptor},{Docente}";
}