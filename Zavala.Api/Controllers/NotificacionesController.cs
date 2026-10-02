using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Auth;
using Zavala.Api.Data;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/notificaciones")]
[Authorize]
public sealed class NotificacionesController(ZavalaAppContext db) : ControllerBase
{
    // La bandeja es la del que pide: sale del token, nunca de la URL.
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var usuarioId = (int)User.UsuarioId();

        var items = await db.Notificaciones.AsNoTracking()
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Creada).ThenByDescending(n => n.Id)
            .Take(50)
            .ToListAsync(cancellationToken);

        // La fecha se guarda en UTC; se marca como tal para que el JSON lleve la "Z"
        // y la app la muestre en la hora del teléfono.
        var notificaciones = items
            .Select(n => new NotificacionResponse(
                n.Id, n.Titulo, n.Mensaje, DateTime.SpecifyKind(n.Creada, DateTimeKind.Utc), n.Leida))
            .ToList();

        var noLeidas = await db.Notificaciones
            .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida, cancellationToken);

        return Ok(new { notificaciones, noLeidas });
    }

    // Solo el número, para el globo de la campana.
    [HttpGet("no-leidas")]
    public async Task<IActionResult> NoLeidas(CancellationToken cancellationToken)
    {
        var usuarioId = (int)User.UsuarioId();
        var noLeidas = await db.Notificaciones
            .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida, cancellationToken);
        return Ok(new { noLeidas });
    }

    [HttpPut("{id:int}/leida")]
    public async Task<IActionResult> MarcarLeida(int id, CancellationToken cancellationToken)
    {
        var usuarioId = (int)User.UsuarioId();

        // Se filtra también por el usuario del token: no se puede tocar la bandeja de otro.
        var filas = await db.Notificaciones
            .Where(n => n.Id == id && n.UsuarioId == usuarioId)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Leida, true), cancellationToken);

        if (filas == 0)
            return NotFound(new { codigo = "notificacion_no_encontrada", mensaje = "La notificación no existe." });

        return Ok(new { leida = true });
    }

    [HttpPut("leidas")]
    public async Task<IActionResult> MarcarTodasLeidas(CancellationToken cancellationToken)
    {
        var usuarioId = (int)User.UsuarioId();
        var filas = await db.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leida)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Leida, true), cancellationToken);

        return Ok(new { marcadas = filas });
    }
}

public sealed record NotificacionResponse(int Id, string Titulo, string Mensaje, DateTime Creada, bool Leida);