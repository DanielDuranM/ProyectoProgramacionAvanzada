using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class CursoesController : Controller
    {
        private readonly AppDbContext _context;

        public CursoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Cursoes/Index
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(rol))
                return RedirectToAction("Login", "Auth");

            var cursos = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .ToListAsync();

            return View(cursos);
        }

        // GET: /Cursoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(rol))
                return RedirectToAction("Login", "Auth");

            if (id == null) return NotFound();

            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (curso == null) return NotFound();
            return View(curso);
        }

        // GET: /Cursoes/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "Nombre");
            ViewData["DocenteId"] = new SelectList(
                _context.Usuarios.Where(u => u.RolId == 2), "Id", "Nombre");
            return View();
        }

        // POST: /Cursoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,CarreraId,DocenteId")] Curso curso)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Curso agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "Nombre", curso.CarreraId);
            ViewData["DocenteId"] = new SelectList(
                _context.Usuarios.Where(u => u.RolId == 2), "Id", "Nombre", curso.DocenteId);
            return View(curso);
        }

        // GET: /Cursoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (id == null) return NotFound();
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "Nombre", curso.CarreraId);
            ViewData["DocenteId"] = new SelectList(
                _context.Usuarios.Where(u => u.RolId == 2), "Id", "Nombre", curso.DocenteId);
            return View(curso);
        }

        // POST: /Cursoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,CarreraId,DocenteId")] Curso curso)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Curso actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "Nombre", curso.CarreraId);
            ViewData["DocenteId"] = new SelectList(
                _context.Usuarios.Where(u => u.RolId == 2), "Id", "Nombre", curso.DocenteId);
            return View(curso);
        }

        // GET: /Cursoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (id == null) return NotFound();

            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Cursoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Curso eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
            return _context.Cursos.Any(e => e.Id == id);
        }
    }
}