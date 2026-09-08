using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Calificacione
{
    public int Id { get; set; }

    public int AlumnoId { get; set; }

    public int MateriaId { get; set; }

    public int ComisionId { get; set; }

    public decimal Nota { get; set; }

    public string Instancia { get; set; } = null!;

    public DateOnly? Fecha { get; set; }

    public virtual Alumno Alumno { get; set; } = null!;

    public virtual Comisione Comision { get; set; } = null!;

    public virtual Materia Materia { get; set; } = null!;
}
