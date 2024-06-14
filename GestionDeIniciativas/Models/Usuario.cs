using System;
using System.Collections.Generic;

namespace GestionDeIniciativas.Models;

public partial class Usuario
{
    public string UsuarioId { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public virtual ICollection<Iniciativa> Iniciativas { get; set; } = new List<Iniciativa>();
}
