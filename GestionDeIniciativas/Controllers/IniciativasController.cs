using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionDeIniciativas.Models;

namespace GestionDeIniciativas.Controllers
{
    public class IniciativasController : Controller
    {
        private readonly ListaIniciativasContext _context;

        public IniciativasController(ListaIniciativasContext context)
        {
            _context = context;
        }

        // GET: Iniciativas
        public async Task<IActionResult> Index()
        {
            var listaIniciativasContext = _context.Iniciativas.Include(i => i.Usuario);
            return View(await listaIniciativasContext.ToListAsync());
        }

        // GET: Iniciativas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iniciativa = await _context.Iniciativas
                .Include(i => i.Tareas)
                .FirstOrDefaultAsync(m => m.IniciativaId == id);

            if (iniciativa == null)
            {
                return NotFound();
            }


            ViewData["IniciativaId"] = new SelectList(_context.Iniciativas, "IniciativaId", "Nombre", id);
            return View(iniciativa);
        }

        // POST: Iniciativas/CreateFromIniciativa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromIniciativa(string iniciativaId, [Bind("Nombre,Categoría,Estado")] Tarea tarea)
        {
            if (string.IsNullOrEmpty(iniciativaId))
            {
                return NotFound();
            }

            var iniciativa = await _context.Iniciativas.FindAsync(iniciativaId);
            if (iniciativa == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                tarea.IniciativaId = iniciativaId;
                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = iniciativaId });
            }

            ViewBag.IniciativaId = iniciativaId;
            return View(tarea);
        }


        // GET: Iniciativas/Create
        public IActionResult Create()
        {
            var usuarios = _context.Usuarios.ToList();
            ViewBag.HayUsuarios = usuarios.Any();
            ViewData["UsuarioId"] = new SelectList(usuarios, "UsuarioId", "UsuarioId");
            return View();
        }

        // POST: Iniciativas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IniciativaId,Nombre,Tipo,Prioridad,UsuarioId")] Iniciativa iniciativa)
        {
            if (_context.Iniciativas.Any(i => i.IniciativaId == iniciativa.IniciativaId))
            {
                ModelState.AddModelError("IniciativaId", "Ya existe una iniciativa con este ID.");
                var usuarios = _context.Usuarios.ToList();
                ViewBag.HayUsuarios = usuarios.Any();
                ViewData["UsuarioId"] = new SelectList(usuarios, "UsuarioId", "UsuarioId", iniciativa.UsuarioId);
                return View(iniciativa);
            }

            if (ModelState.IsValid)
            {
                _context.Add(iniciativa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", iniciativa.UsuarioId);
            return View(iniciativa);
        }

        // GET: Iniciativas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iniciativa = await _context.Iniciativas.FindAsync(id);
            if (iniciativa == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", iniciativa.UsuarioId);
            return View(iniciativa);
        }

        // POST: Iniciativas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IniciativaId,Nombre,Tipo,Prioridad,UsuarioId")] Iniciativa iniciativa)
        {
            if (id != iniciativa.IniciativaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(iniciativa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IniciativaExists(iniciativa.IniciativaId))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "UsuarioId", iniciativa.UsuarioId);
            return View(iniciativa);
        }

        // GET: Iniciativas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iniciativa = await _context.Iniciativas
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(m => m.IniciativaId == id);
            if (iniciativa == null)
            {
                return NotFound();
            }

            // Verificar si la iniciativa tiene tareas asociadas
            var tieneTareas = await _context.Tareas.AnyAsync(t => t.IniciativaId == id);
            ViewBag.TieneTareas = tieneTareas;

            return View(iniciativa);
        }

        // POST: Iniciativas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var iniciativa = await _context.Iniciativas
                .Include(i => i.Tareas)
                .ThenInclude(t => t.BloqueTiempos)
                .FirstOrDefaultAsync(m => m.IniciativaId == id);

            if (iniciativa != null)
            {
                // Eliminar los bloques de tiempo asociados a cada tarea
                foreach (var tarea in iniciativa.Tareas)
                {
                    _context.BloqueTiempos.RemoveRange(tarea.BloqueTiempos);
                }

                // Eliminar las tareas asociadas
                _context.Tareas.RemoveRange(iniciativa.Tareas);

                // Eliminar la iniciativa
                _context.Iniciativas.Remove(iniciativa);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IniciativaExists(string id)
        {
            return _context.Iniciativas.Any(e => e.IniciativaId == id);
        }
    }
}
