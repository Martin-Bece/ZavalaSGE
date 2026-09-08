using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Materia
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Codigo { get; set; }

    public virtual ICollection<AsignacionesDocente> AsignacionesDocentes { get; set; } = new List<AsignacionesDocente>();

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();
}
