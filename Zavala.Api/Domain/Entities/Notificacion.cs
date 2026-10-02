namespace Zavala.Api.Domain.Entities;

public partial class Notificacion
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Titulo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public DateTime Creada { get; set; }

    public bool Leida { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}