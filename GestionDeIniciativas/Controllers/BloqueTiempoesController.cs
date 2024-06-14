using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionDeIniciativas.Models;


namespace GestionDeIniciativas.Controllers
{
    public class BloqueTiempoesController : Controller
    {
        private readonly ListaIniciativasContext _context;

        public BloqueTiempoesController(ListaIniciativasContext context)
        {
            _context = context;
        }

        // GET: BloqueTiempoes
        public async Task<IActionResult> Index()
        {
            var ListaIniciativasContext = _context.BloqueTiempos.Include(b => b.Tarea);
            return View(await ListaIniciativasContext.ToListAsync());
        }

        // GET: BloqueTiempoes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloqueTiempo = await _context.BloqueTiempos
                .Include(b => b.Tarea)
                .FirstOrDefaultAsync(m => m.TiempoId == id);
            if (bloqueTiempo == null)
            {
                return NotFound();
            }

            return View(bloqueTiempo);
        }

        // GET: BloqueTiempoes/Create
        public IActionResult Create()
        {
            ViewData["TareaId"] = new SelectList(_context.Tareas, "TareaId", "TareaId");
            return View();
        }


        // POST: BloqueTiempoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TiempoId,DiaMes,DiaSemana,Progreso,Mes,Año,TareaId")] BloqueTiempo bloqueTiempo)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el ID ya existe
                var existingBlockTime = await _context.BloqueTiempos.FindAsync(bloqueTiempo.TiempoId);
                if (existingBlockTime != null)
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un bloque de tiempo con este ID.");
                    ViewData["TareaId"] = new SelectList(_context.Tareas, "TareaId", "TareaId", bloqueTiempo.TareaId);
                    return View(bloqueTiempo);
                }

                // Obtiene la tarea relacionada
                var tarea = await _context.Tareas
                    .Include(t => t.BloqueTiempos)
                    .FirstOrDefaultAsync(t => t.TareaId == bloqueTiempo.TareaId);

                if (tarea != null)
                {
                    // Calcula las horas restantes y el progreso
                    int horasTotales = tarea.Horas ?? 0;
                    int progresoTotal = (int)tarea.BloqueTiempos.Sum(bt => bt.Progreso);
                    int horasRestantes = horasTotales - progresoTotal;
                    horasRestantes = (horasRestantes < 0) ? 0 : horasRestantes; // Asegurarse de que las horas restantes no sean negativas

                    // Verificar si el progreso supera las horas restantes de la tarea
                    if (bloqueTiempo.Progreso > horasRestantes)
                    {
                        ModelState.AddModelError(string.Empty, "El progreso ingresado excede las horas restantes de la tarea.");
                        ViewData["TareaId"] = new SelectList(_context.Tareas, "TareaId", "TareaId", bloqueTiempo.TareaId);
                        return View(bloqueTiempo);
                    }

                    int progresoPorcentaje = (horasTotales > 0) ? (int)((double)(progresoTotal + bloqueTiempo.Progreso) / horasTotales * 100) : 0;

                    // Verificar si el progreso supera el 100%
                    progresoPorcentaje = (progresoPorcentaje > 100) ? 100 : progresoPorcentaje;

                    // Actualiza los valores en la tarea
                    tarea.HorasRestantes = horasRestantes - bloqueTiempo.Progreso;
                    tarea.Progreso = progresoPorcentaje;

                    // Marcar la tarea como completada si el progreso alcanza el 100%
                    if (progresoPorcentaje == 100)
                    {
                        tarea.Estado = "Completada";
                    }

                    // Guarda los cambios en la base de datos
                    _context.Update(tarea);
                    _context.Add(bloqueTiempo);
                    await _context.SaveChangesAsync();

                    // Redirige a los detalles de la tarea
                    return RedirectToAction("Details", "Tareas", new { id = bloqueTiempo.TareaId });
                }
            }

            // Si hay algún error en el modelo, volvemos a cargar la vista con los datos
            ViewData["TareaId"] = new SelectList(_context.Tareas, "TareaId", "TareaId", bloqueTiempo.TareaId);
            return View(bloqueTiempo);
        }


        // GET: BloqueTiempoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloqueTiempo = await _context.BloqueTiempos.FindAsync(id);
            if (bloqueTiempo == null)
            {
                return NotFound();
            }
            ViewData["TareaId"] = new SelectList(_context.Tareas, "TareaId", "TareaId", bloqueTiempo.TareaId);
            return View(bloqueTiempo);
        }

        // POST: BloqueTiempoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TiempoId,DiaMes,DiaSemana,Progreso,Mes,Año,TareaId")] BloqueTiempo bloqueTiempo)
        {
            if (id != bloqueTiempo.TiempoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bloqueTiempo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BloqueTiempoExists(bloqueTiempo.TiempoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Tareas", new { id = bloqueTiempo.TareaId });
            }
            ViewData["TareaId"] = new SelectList(_context.Tareas, "TareaId", "TareaId", bloqueTiempo.TareaId);
            return View(bloqueTiempo);
        }

        // GET: BloqueTiempoes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bloqueTiempo = await _context.BloqueTiempos
                .Include(b => b.Tarea)
                .FirstOrDefaultAsync(m => m.TiempoId == id);
            if (bloqueTiempo == null)
            {
                return NotFound();
            }

            return View(bloqueTiempo);
        }

        // POST: BloqueTiempoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bloqueTiempo = await _context.BloqueTiempos.FindAsync(id);
            if (bloqueTiempo != null)
            {
                _context.BloqueTiempos.Remove(bloqueTiempo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BloqueTiempoExists(string id)
        {
            return _context.BloqueTiempos.Any(e => e.TiempoId == id);
        }
    }
}
