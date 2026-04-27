using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class CarrerasController : Controller
    {
        private readonly AppDbContext _context;

        public CarrerasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Carreras/Index
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(rol))
                return RedirectToAction("Login", "Auth");

            return View(await _context.Carreras.ToListAsync());
        }

        // GET: /Carreras/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            return View();
        }

        // POST: /Carreras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] Carrera carrera)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                _context.Add(carrera);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Carrera agregada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(carrera);
        }

        // GET: /Carreras/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (id == null) return NotFound();
            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // POST: /Carreras/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Carrera carrera)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (id != carrera.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(carrera);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Carrera actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(carrera);
        }

        // GET: /Carreras/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            if (id == null) return NotFound();
            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrera == null) return NotFound();
            return View(carrera);
        }

        // POST: /Carreras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
                return RedirectToAction("Index");

            var carrera = await _context.Carreras.FindAsync(id);
            if (carrera != null)
            {
                _context.Carreras.Remove(carrera);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Carrera eliminada correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}