using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/materias")]
public sealed class MateriasController(ZavalaAppContext db) : ControllerBase 
{
    [HttpGet]
    public async Task<IActionResult> GetMaterias(CancellationToken ct)
    {
        var materias = await db.Materias
            .AsNoTracking()
            .Select(x => new {
                x.Id,
                x.Nombre,
                x.Codigo
            })
            .ToListAsync(ct);

        return Ok(new { materias });
    }
}