using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Curso
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int CicloLectivo { get; set; }

    public virtual ICollection<Comisione> Comisiones { get; set; } = new List<Comisione>();
}
