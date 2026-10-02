using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zavala.Api.Data;
using Zavala.Api.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Zavala.Api.Auth;

namespace Zavala.Api.Controllers;

[ApiController]
[Route("api/alumnos")]
[Authorize]
public sealed class AlumnosController(ZavalaAppContext db) : ControllerBase
{

    [HttpGet]
    [Authorize(Roles = RolCodigos.Admin)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] int? pagina,
        [FromQuery] int? tamano,
        [FromQuery] string? estado,
        CancellationToken cancellationToken
    )
    {
        var consulta = db.Alumnos.AsNoTracking().Include(a => a.Persona).AsQueryable();
        var texto = (busqueda ?? string.Empty).Trim();

        if (!string.IsNullOrWhiteSpace(estado))
            consulta = consulta.Where(x => x.Estado == estado);

        if (texto.Length > 0)
        {
            consulta = consulta.Where(x =>
                x.Legajo.Contains(texto) ||
                x.Persona.Nombre.Contains(texto) ||
                x.Persona.Apellido.Contains(texto) ||
                x.Persona.Dni.Contains(texto));
        }

        var (items, paginaResponse) = await consulta
            .OrderBy(x => x.Persona.Apellido)
            .ThenBy(x => x.Persona.Nombre)
            .Select(x => Proyectar(x))
            .PaginarAsync(PaginaConsulta.Desde(pagina, tamano), cancellationToken);

        return Ok(new { alumnos = items, pagina = paginaResponse });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        var alumno = await db.Alumnos.AsNoTracking()
            .Include(a => a.Persona)
            .Where(x => x.Id == id)
            .Select(x => Proyectar(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (alumno is null) return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });
        if (!await db.PuedeVerAlumnoAsync(User, id, cancellationToken))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No tenés acceso a este alumno." });
        return Ok(new { alumno });
    }

    [HttpGet("{id:int}/constancia")]
    [Authorize(Roles = $"{RolCodigos.Admin},{RolCodigos.Preceptor}")]
    public async Task<IActionResult> Constancia(int id, CancellationToken cancellationToken)
    {
        var alumno = await db.Alumnos.AsNoTracking()
            .Include(a => a.Persona)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (alumno is null)
            return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });

        if (!await db.PuedeVerAlumnoAsync(User, id, cancellationToken))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No tenés acceso a este alumno." });

        // 409 y no 400: el pedido es válido, lo que choca es el estado del alumno.
        if (alumno.Estado != EstadoAlumno.Regular)
            return Conflict(new { codigo = "alumno_no_regular", mensaje = "Solo se emite la constancia a alumnos regulares." });

        var codigo = CodigoAlumno(alumno.Id);
        return File(GenerarConstanciaPdf(alumno, codigo), "application/pdf", $"constancia-{codigo}.pdf");
    }

    [HttpPost]
    [Authorize(Roles = RolCodigos.Admin)]
    public async Task<IActionResult> Crear(GuardarAlumnoRequest request, CancellationToken cancellationToken)
    {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var legajo = request.Legajo.Trim();
        var duplicadoLegajo = await db.Alumnos.AnyAsync(x => x.Legajo == legajo, cancellationToken);
        if (duplicadoLegajo) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe un alumno con ese legajo." });

        var personaExiste = await db.Personas.AnyAsync(x => x.Id == request.PersonaId, cancellationToken);
        if (!personaExiste) return BadRequest(new { codigo = "persona_no_existe", mensaje = "La persona seleccionada no existe." });

        var duplicadoPersona = await db.Alumnos.AnyAsync(x => x.PersonaId == request.PersonaId, cancellationToken);
        if (duplicadoPersona) return BadRequest(new { codigo = "persona_ya_es_alumno", mensaje = "Esta persona ya está registrada como alumno." });

        var alumno = new Alumno();
        Aplicar(alumno, request);
        db.Alumnos.Add(alumno);
        await db.SaveChangesAsync(cancellationToken);

        // Recargamos navegación para devolver la proyección completa
        await db.Entry(alumno).Reference(a => a.Persona).LoadAsync(cancellationToken);
        return Ok(new { alumno = Proyectar(alumno) });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RolCodigos.Admin)]
    public async Task<IActionResult> Actualizar(int id, GuardarAlumnoRequest request, CancellationToken cancellationToken)
    {
        var error = Validar(request);
        if (error is not null) return BadRequest(error);

        var alumno = await db.Alumnos.Include(a => a.Persona).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (alumno is null) return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });

        var legajo = request.Legajo.Trim();
        var duplicadoLegajo = await db.Alumnos.AnyAsync(x => x.Legajo == legajo && x.Id != id, cancellationToken);
        if (duplicadoLegajo) return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe otro alumno con ese legajo." });

        Aplicar(alumno, request);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { alumno = Proyectar(alumno) });
    }


    [Authorize(Roles = $"{RolCodigos.Admin},{RolCodigos.Preceptor}")]
    [HttpPut("{id:int}/datos")]
    public async Task<IActionResult> ActualizarDatos(
        int id, ActualizarDatosAlumnoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Legajo))
            return BadRequest(new { codigo = "legajo_requerido", mensaje = "El legajo es obligatorio." });

        var estado = (request.Estado ?? string.Empty).Trim().ToLowerInvariant();
        if (!EstadoAlumno.Todos.Contains(estado))
            return BadRequest(new { codigo = "estado_invalido", mensaje = "El estado debe ser regular, libre o baja." });

        var alumno = await db.Alumnos.Include(a => a.Persona).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (alumno is null)
            return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });

        if (!await db.PuedeVerAlumnoAsync(User, id, cancellationToken))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { codigo = "sin_permiso", mensaje = "No tenés acceso a este alumno." });

        var legajo = request.Legajo.Trim();
        var duplicado = await db.Alumnos.AnyAsync(x => x.Legajo == legajo && x.Id != id, cancellationToken);
        if (duplicado)
            return BadRequest(new { codigo = "legajo_duplicado", mensaje = "Ya existe otro alumno con ese legajo." });

        var estadoAnterior = alumno.Estado;
        alumno.Legajo = legajo;
        alumno.Estado = estado;

        // Si cambió el estado, se avisa a los docentes del alumno (nunca al que lo cambió).
        // Va en el mismo SaveChanges que el cambio: pasan las dos cosas o ninguna.
        if (estado != estadoAnterior)
        {
            var actorId = (int)User.UsuarioId();
            var actor = await db.NombreDeUsuarioAsync(actorId, cancellationToken);
            var docentes = await db.UsuarioIdsDeDocentesDelAlumnoAsync(id, cancellationToken);
            foreach (var docenteUsuarioId in docentes.Where(d => d != actorId))
            {
                db.Notificar(docenteUsuarioId,
                    $"Cambio de estado · {alumno.Persona.Apellido}, {alumno.Persona.Nombre}",
                    $"{actor} cambió el estado de {alumno.Persona.Apellido}, {alumno.Persona.Nombre} de {estadoAnterior} a {estado}.");
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        return Ok(new { alumno = Proyectar(alumno) });
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = RolCodigos.Admin)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var alumno = await db.Alumnos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (alumno is null) return NotFound(new { codigo = "alumno_no_encontrado", mensaje = "El alumno no existe." });

        db.Alumnos.Remove(alumno);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(new { eliminado = true });
    }

    private static object? Validar(GuardarAlumnoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Legajo)) return new { codigo = "legajo_requerido", mensaje = "El legajo es obligatorio." };
        if (request.PersonaId <= 0) return new { codigo = "persona_requerida", mensaje = "Debe seleccionar una persona válida." };
        if (request.Estado is not null && !EstadoAlumno.Todos.Contains(request.Estado.Trim().ToLowerInvariant()))
            return new { codigo = "estado_invalido", mensaje = "El estado debe ser regular, libre o baja." };
        return null;
    }

    private static void Aplicar(Alumno alumno, GuardarAlumnoRequest request)
    {
        alumno.Legajo = request.Legajo.Trim();
        alumno.PersonaId = request.PersonaId;
        if (request.Estado is not null) alumno.Estado = request.Estado.Trim().ToLowerInvariant();
    }

    private static AlumnoResponse Proyectar(Alumno x) =>
        new(
            x.Id,
            x.PersonaId,
            x.Legajo,
            x.Estado,
            x.Persona?.Nombre ?? string.Empty,
            x.Persona?.Apellido ?? string.Empty,
            x.Persona?.Dni ?? string.Empty,
            x.Persona?.Telefono,
            x.Persona?.Direccion,
            x.Persona?.FechaNacimiento
        );

    private const string NombreEscuela = "Escuela Nocturna N° 4 - Juan Tulio Zavala";
    private const string Localidad = "San Luis";

    internal static string CodigoAlumno(int id) => $"ALU-{id:D4}";

    private static byte[] GenerarQr(string codigo)
    {
        using var generador = new QRCodeGenerator();
        using var datos = generador.CreateQrCode(codigo, QRCodeGenerator.ECCLevel.M);
        return new PngByteQRCode(datos).GetGraphic(8);
    }

    private static byte[] GenerarConstanciaPdf(Alumno alumno, string codigo)
    {
        var cultura = new CultureInfo("es-AR");
        var hoy = DateTime.Now;
        var persona = alumno.Persona;

        var texto =
            $"Se deja constancia que {persona.Apellido.ToUpper()} {persona.Nombre.ToUpper()} " +
            $"con DNI {persona.Dni} es alumno regular de {NombreEscuela}, legajo Nro: {alumno.Legajo}. " +
            $"Se extiende la presente constancia, para ser presentada ante QUIEN CORRESPONDA " +
            $"en {Localidad} a los {hoy.Day} días del mes de {hoy.ToString("MMMM", cultura)} de {hoy.Year}.";

        return Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(40);
                pagina.DefaultTextStyle(estilo => estilo.FontSize(11));

                pagina.Header().AlignCenter().Text(NombreEscuela).FontSize(18).Bold();

                pagina.Content().PaddingVertical(40).Column(columna =>
                {
                    columna.Item().AlignCenter().Text("Constancia de Alumno Regular")
                        .FontSize(14).Bold().Underline();
                    columna.Item().PaddingTop(30).Text(texto).Justify();
                });

                pagina.Footer().AlignCenter().Column(columna =>
                {
                    columna.Item().AlignCenter().Width(90).Image(GenerarQr(codigo));
                    columna.Item().AlignCenter().Text($"CÓDIGO DE VALIDACIÓN: {codigo}").FontSize(9);
                    columna.Item().PaddingTop(6).AlignCenter()
                        .Text("Esta constancia puede ser validada escaneando el código QR desde la app ZavalaSGE, por personal autorizado.")
                        .FontSize(8);
                });
            });
        }).GeneratePdf();
    }

}

public sealed record AlumnoResponse(
    int Id,
    int PersonaId,
    string Legajo,
    string Estado,
    string Nombre,
    string Apellido,
    string Dni,
    string? Telefono,
    string? Direccion,
    DateOnly? FechaNacimiento
);

public sealed record GuardarAlumnoRequest(
    string Legajo,
    int PersonaId,
    string? Estado = null
);

public sealed record ActualizarDatosAlumnoRequest(string Legajo, string Estado);