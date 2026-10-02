using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Alumno
{
    public int Id { get; set; }

    public string Legajo { get; set; } = null!;

    public string Estado { get; set; } = EstadoAlumno.Regular;

    public int PersonaId { get; set; }

    public virtual ICollection<Asistencia> Asistencia { get; set; } = new List<Asistencia>();

    public virtual ICollection<Calificacione> Calificaciones { get; set; } = new List<Calificacione>();

    public virtual ICollection<Inscripcione> Inscripciones { get; set; } = new List<Inscripcione>();

    public virtual Persona Persona { get; set; } = null!;
}

public static class EstadoAlumno
{
    public const string Regular = "regular";
    public const string Libre = "libre";
    public const string Baja = "baja";

    public static readonly string[] Todos = [Regular, Libre, Baja];
}
