using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionDeIniciativas.Models;
using System.Globalization;

namespace GestionDeIniciativas.Controllers
{
    public class TareasController : Controller
    {
        private readonly ListaIniciativasContext _context;

        public TareasController(ListaIniciativasContext context)
        {
            _context = context;
        }



        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            var listaIniciativasContext = _context.Tareas.Include(t => t.Iniciativa);
            return View(await listaIniciativasContext.ToListAsync());
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
                .Include(t => t.Iniciativa)
                .Include(t => t.BloqueTiempos)
                .FirstOrDefaultAsync(m => m.TareaId == id);
            if (tarea == null)
            {
                return NotFound();
            }

            // Obtener el ID de la iniciativa asociada a la tarea
            var iniciativaId = tarea.IniciativaId;

            // Pasar el ID de la iniciativa a la vista
            ViewBag.IniciativaId = iniciativaId;

            return View(tarea);
        }

        // GET: Tareas/Create
        public IActionResult Create()
        {
            var iniciativas = _context.Iniciativas.ToList();
            ViewBag.HayIniciativas = iniciativas.Any();
            ViewData["IniciativaId"] = new SelectList(iniciativas, "IniciativaId", "IniciativaId");
            return View();
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TareaId,Nombre,Estado,Categoría,IniciativaId,Horas")] Tarea tarea)
        {
            {
                if (_context.Tareas.Any(t => t.TareaId == tarea.TareaId))
                {
                    ModelState.AddModelError("TareaId", "Ya existe una tarea con este ID.");
                    // Cargar la lista de iniciativas nuevamente y pasarla a la vista
                    ViewBag.HayIniciativas = _context.Iniciativas.Any();
                    ViewData["IniciativaId"] = new SelectList(_context.Iniciativas, "IniciativaId", "IniciativaId", tarea.IniciativaId);
                    return View(tarea);
                }

                if (ModelState.IsValid)
                {
                    _context.Add(tarea);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Iniciativas", new { id = tarea.IniciativaId });
                }

                // Cargar la lista de iniciativas nuevamente y pasarla a la vista
                ViewBag.HayIniciativas = _context.Iniciativas.Any();
                ViewData["IniciativaId"] = new SelectList(_context.Iniciativas, "IniciativaId", "IniciativaId", tarea.IniciativaId);
                return View(tarea);
            }
        }

        // GET: Tareas/CreateFromIniciativa
        public IActionResult CreateFromIniciativa(string iniciativaId)
        {
            ViewData["IniciativaId"] = iniciativaId;
            return View();
        }


        // GET: Tareas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            ViewData["IniciativaId"] = new SelectList(_context.Iniciativas, "IniciativaId", "IniciativaId", tarea.IniciativaId);
            return View(tarea);
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TareaId,Nombre,Estado,Categoría,IniciativaId,Horas")] Tarea tarea)
        {
            if (id != tarea.TareaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.TareaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IniciativaId"] = new SelectList(_context.Iniciativas, "IniciativaId", "IniciativaId", tarea.IniciativaId);
            return View(tarea);
        }

        // GET: Tareas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
                .Include(t => t.Iniciativa)
                .FirstOrDefaultAsync(m => m.TareaId == id);
            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tarea = await _context.Tareas.Include(t => t.BloqueTiempos).FirstOrDefaultAsync(t => t.TareaId == id);
            if (tarea != null)
            {
                // Eliminar los bloques de tiempo asociados
                _context.BloqueTiempos.RemoveRange(tarea.BloqueTiempos);

                // Eliminar la tarea
                _context.Tareas.Remove(tarea);

                // Guardar cambios en la base de datos
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Método para marcar una tarea como completada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Estado = "Completada";
            tarea.HorasRestantes = 0;
            tarea.Progreso = 100;
            _context.Update(tarea);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        private bool TareaExists(string id)
        {
            return _context.Tareas.Any(e => e.TareaId == id);
        }
        
    }
}




