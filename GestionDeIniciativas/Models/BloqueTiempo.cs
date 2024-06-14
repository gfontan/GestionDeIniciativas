using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestionDeIniciativas.Models;

public partial class BloqueTiempo
{
   
    public string TiempoId { get; set; } = null!;

    public string DiaMes { get; set; } = null!;

    public string DiaSemana { get; set; } = null!;

    public int? Progreso { get; set; } = null;

    public int? Año { get; set; } = null!;

    public string Mes {  get; set; } = null!;

    public string? TareaId { get; set; }

    public virtual Tarea? Tarea { get; set; }
}