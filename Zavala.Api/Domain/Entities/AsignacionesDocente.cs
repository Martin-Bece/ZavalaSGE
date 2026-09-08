using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class AsignacionesDocente
{
    public int Id { get; set; }

    public int DocenteId { get; set; }

    public int MateriaId { get; set; }

    public int ComisionId { get; set; }

    public virtual Comisione Comision { get; set; } = null!;

    public virtual Docente Docente { get; set; } = null!;

    public virtual Materia Materia { get; set; } = null!;
}
