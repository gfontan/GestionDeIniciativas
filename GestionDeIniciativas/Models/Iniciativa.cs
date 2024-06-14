using GestionDeIniciativas.Models;
using System;
using System.Collections.Generic;

namespace GestionDeIniciativas.Models;

public partial class Iniciativa
{
    public string IniciativaId { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string Prioridad { get; set; } = null!;

    public string? UsuarioId { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual Usuario? Usuario { get; set; }
}
