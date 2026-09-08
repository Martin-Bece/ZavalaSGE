using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Inscripcione
{
    public int Id { get; set; }

    public int AlumnoId { get; set; }

    public int ComisionId { get; set; }

    public DateOnly? FechaInscripcion { get; set; }

    public virtual Alumno Alumno { get; set; } = null!;

    public virtual Comisione Comision { get; set; } = null!;
}
