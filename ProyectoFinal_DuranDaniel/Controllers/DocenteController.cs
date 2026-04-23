using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class DocenteController : Controller
    {
        private readonly AppDbContext _db;

        public DocenteController(AppDbContext db)
        {
            _db = db;
        }

        private int? ObtenerIdUsuario()
            => HttpContext.Session.GetInt32("UsuarioId");

        // GET: /Docente/MisCursos
        public async Task<IActionResult> MisCursos()
        {
            var uid = ObtenerIdUsuario();
            if (uid == null) return RedirectToAction("Login", "Auth");

            var cursos = await _db.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Matriculas)
                .Where(c => c.DocenteId == uid)
                .ToListAsync();

            return View(cursos);
        }

        // GET: /Docente/DetalleCurso/5
        public async Task<IActionResult> DetalleCurso(int id)
        {
            var uid = ObtenerIdUsuario();
            if (uid == null) return RedirectToAction("Login", "Auth");

            var curso = await _db.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Matriculas)
                    .ThenInclude(m => m.Estudiante)
                .FirstOrDefaultAsync(c => c.Id == id && c.DocenteId == uid);

            if (curso == null) return NotFound();
            return View(curso);
        }
    }
}