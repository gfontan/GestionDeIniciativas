namespace GestionDeIniciativas.Models
{
    public class ReporteIniciativaViewModel
    {
        public string IniciativaId { get; set; }
        public string NombreIniciativa { get; set; }
        public int TareasTotales { get; set; }
        public int TareasCompletadas { get; set; }
        public double PorcentajeCompletado
        {
            get
            {
                return TareasTotales == 0 ? 0 : (double)TareasCompletadas / TareasTotales * 100;
            }
        }
    }



}