using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class DocenteController : Controller
    {
        private readonly AppDbContext _context;

        public DocenteController(AppDbContext context)
        {
            _context = context;
        }

        private int?   GetUserId()  => HttpContext.Session.GetInt32("UsuarioId");
        private bool   EsDocente()  => HttpContext.Session.GetString("UsuarioRol") == "Docente";

        // GET: /Docente/MisCursos
        public async Task<IActionResult> MisCursos()
        {
            if (!EsDocente()) return RedirectToAction("Login", "Auth");

            var uid    = GetUserId();
            var cursos = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Matriculas)
                .Where(c => c.DocenteId == uid)
                .ToListAsync();

            return View(cursos);
        }

        // GET: /Docente/DetalleCurso/5
        public async Task<IActionResult> DetalleCurso(int id)
        {
            if (!EsDocente()) return RedirectToAction("Login", "Auth");

            var uid   = GetUserId();
            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Matriculas)
                    .ThenInclude(m => m.Estudiante)
                .FirstOrDefaultAsync(c => c.Id == id && c.DocenteId == uid);

            if (curso == null) return NotFound();
            return View(curso);
        }
    }
}
