using System;
using System.Collections.Generic;

namespace Zavala.Api.Domain.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool? Estado { get; set; }

    public int PersonaId { get; set; }

    public virtual Persona Persona { get; set; } = null!;
}
