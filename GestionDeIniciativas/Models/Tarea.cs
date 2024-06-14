namespace GestionDeIniciativas.Models
{
    public partial class Tarea
    {
        public string TareaId { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public int? Horas { get; set; } = null!;
        public int? HorasRestantes { get; set; } = null!;
        public int? Progreso { get; set; } = null!;
        public string Categoría { get; set; } = null!;
        public string? IniciativaId { get; set; }
        public virtual ICollection<BloqueTiempo> BloqueTiempos { get; set; } = new List<BloqueTiempo>();
        public virtual Iniciativa? Iniciativa { get; set; }

    }
    
}
