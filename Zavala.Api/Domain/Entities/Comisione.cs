using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Comisione
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int CursoId { get; set; }

    public int PreceptorId { get; set; }

    public virtual ICollection<AsignacionesDocente> AsignacionesDocentes { get; set; } = new List<AsignacionesDocente>();

    public virtual ICollection<Asistencia> Asistencia { get; set; } = new List<Asistencia>();

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();

    public virtual Curso Curso { get; set; } = null!;

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();

    public virtual Preceptore Preceptor { get; set; } = null!;
}
