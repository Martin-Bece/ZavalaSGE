using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonasController : ControllerBase
{
    private readonly ZavalaAppContext _context;

    public PersonasController(ZavalaAppContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonas()
    {
        var personas = await _context.Personas
            .Select(p => new
            {
                id = p.Id,
                dni = p.Dni,
                nombre = p.Nombre,
                apellido = p.Apellido,
                telefono = p.Telefono,
                direccion = p.Direccion
            })
            .ToListAsync();

        return Ok(new { personas });
    }
}