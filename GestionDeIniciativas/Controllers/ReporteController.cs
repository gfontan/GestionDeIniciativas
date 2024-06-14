using Microsoft.AspNetCore.Mvc;
using GestionDeIniciativas.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class ReporteController : Controller
{
    private readonly ListaIniciativasContext _context;

    public ReporteController(ListaIniciativasContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ReporteIniciativas()
    {
        var iniciativas = await _context.Iniciativas
            .Include(i => i.Tareas)
            .Select(i => new ReporteIniciativaViewModel
            {
                IniciativaId = i.IniciativaId,
                NombreIniciativa = i.Nombre,
                TareasTotales = i.Tareas.Count(),
                TareasCompletadas = i.Tareas.Count(t => t.Estado == "Completada")
            })
            .ToListAsync();

        return View(iniciativas);
    }
}
