using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Persona
{
    public int Id { get; set; }

    public string Dni { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public virtual Alumno? Alumno { get; set; }

    public virtual Docente? Docente { get; set; }

    public virtual Preceptore? Preceptore { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
