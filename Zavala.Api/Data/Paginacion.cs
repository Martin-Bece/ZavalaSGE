using Microsoft.EntityFrameworkCore;

namespace Zavala.Api.Data;

public sealed record PaginaConsulta {
    public const int TamanoPorDefecto = 20;
    public const int TamanoMaximo = 100;

    private PaginaConsulta(int pagina, int tamano) {
        Pagina = pagina;
        Tamano = tamano;
    }

    public int Pagina { get; }
    public int Tamano { get; }
    public int Saltear => (Pagina - 1) * Tamano;

    public static PaginaConsulta Desde(int? pagina, int? tamano) {
        var paginaSegura = pagina.HasValue && pagina.Value > 0 ? pagina.Value : 1;
        var tamanoSeguro = tamano.HasValue && tamano.Value > 0 ? Math.Min(tamano.Value, TamanoMaximo) : TamanoPorDefecto;
        return new PaginaConsulta(paginaSegura, tamanoSeguro);
    }
}

public sealed record PaginaResponse(int Pagina, int Tamano, int Total, bool HayMas);

public static class FiltroEnum {
    public static List<TEnum> Parsear<TEnum>(string? valores) where TEnum : struct, Enum {
        if(string.IsNullOrWhiteSpace(valores)) return [];
        var resultado = new List<TEnum>();
        foreach(var texto in valores.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) {
            var normalizado = texto.Replace("_", string.Empty);
            foreach(var candidato in Enum.GetValues<TEnum>()) {
                if(!string.Equals(candidato.ToString(), normalizado, StringComparison.OrdinalIgnoreCase)) continue;
                if(!resultado.Contains(candidato)) resultado.Add(candidato);
                break;
            }
        }
        return resultado;
    }
}

public static class PaginacionExtensions {
    public static async Task<(List<T> Items, PaginaResponse Pagina)> PaginarAsync<T>(this IQueryable<T> consulta, PaginaConsulta pagina, CancellationToken cancellationToken) {
        var total = await consulta.CountAsync(cancellationToken);
        var items = await consulta.Skip(pagina.Saltear).Take(pagina.Tamano).ToListAsync(cancellationToken);
        return (items, new PaginaResponse(pagina.Pagina, pagina.Tamano, total, pagina.Saltear + items.Count < total));
    }
}