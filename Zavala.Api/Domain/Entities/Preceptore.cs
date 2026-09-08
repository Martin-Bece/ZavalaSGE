using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Preceptore
{
    public int Id { get; set; }

    public string Legajo { get; set; } = null!;

    public int PersonaId { get; set; }

    public virtual ICollection<Comisione> Comisiones { get; set; } = new List<Comisione>();

    public virtual Persona Persona { get; set; } = null!;
}
