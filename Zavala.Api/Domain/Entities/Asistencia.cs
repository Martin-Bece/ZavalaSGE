using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Asistencia
{
    public int Id { get; set; }

    public int AlumnoId { get; set; }

    public int ComisionId { get; set; }

    public DateOnly Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Alumno Alumno { get; set; } = null!;

    public virtual Comisione Comision { get; set; } = null!;
}
