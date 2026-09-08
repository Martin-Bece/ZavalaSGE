using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Docente
{
    public int Id { get; set; }

    public string Legajo { get; set; } = null!;

    public string? Titulo { get; set; }

    public int PersonaId { get; set; }

    public virtual ICollection<AsignacionesDocente> AsignacionesDocentes { get; set; } = new List<AsignacionesDocente>();

    public virtual Persona Persona { get; set; } = null!;
}
