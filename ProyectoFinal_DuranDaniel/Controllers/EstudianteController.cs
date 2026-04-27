using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly AppDbContext _context;

        public EstudianteController(AppDbContext context)
        {
            _context = context;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UsuarioId");
        private bool EsEstudiante() => HttpContext.Session.GetString("UsuarioRol") == "Estudiante";

        // GET: /Estudiante/Carreras
        public async Task<IActionResult> Carreras()
        {
            if (!EsEstudiante()) return RedirectToAction("Login", "Auth");
            var carreras = await _context.Carreras.ToListAsync();
            return View("~/Views/Estudiante/Carreras.cshtml", carreras);
        }

        // GET: /Estudiante/Cursos?carreraId=1
        public async Task<IActionResult> Cursos(int carreraId)
        {
            if (!EsEstudiante()) return RedirectToAction("Login", "Auth");

            var uid = GetUserId();
            var carrera = await _context.Carreras.FindAsync(carreraId);
            if (carrera == null) return NotFound();

            var cursos = await _context.Cursos
                .Include(c => c.Docente)
                .Where(c => c.CarreraId == carreraId)
                .ToListAsync();

            var matriculados = await _context.Matriculas
                .Where(m => m.EstudianteId == uid)
                .Select(m => m.CursoId)
                .ToListAsync();

            ViewBag.Carrera = carrera;
            ViewBag.Matriculados = matriculados;
            return View("~/Views/Estudiante/Cursos.cshtml", cursos);
        }

        // POST: /Estudiante/Matricular — vía AJAX
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Matricular([FromBody] MatricularRequest request)
        {
            var uid = GetUserId();
            if (uid == null) return Json(new { ok = false, msg = "Sesión expirada" });

            bool yaMatriculado = await _context.Matriculas
                .AnyAsync(m => m.EstudianteId == uid && m.CursoId == request.CursoId);

            if (yaMatriculado)
                return Json(new { ok = false, msg = "Ya estás matriculado en este curso" });

            _context.Matriculas.Add(new Matricula
            {
                EstudianteId = uid.Value,
                CursoId = request.CursoId,
                FechaMatricula = DateTime.Now,
                Estado = "Matriculado"
            });
            await _context.SaveChangesAsync();
            return Json(new { ok = true, msg = "Matriculado exitosamente" });
        }

        // GET: /Estudiante/MisCursos
        public async Task<IActionResult> MisCursos()
        {
            if (!EsEstudiante()) return RedirectToAction("Login", "Auth");

            var uid = GetUserId();
            var matriculas = await _context.Matriculas
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Carrera)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Docente)
                .Where(m => m.EstudianteId == uid)
                .ToListAsync();

            return View(matriculas);
        }
    }

    // Clase auxiliar para recibir el JSON del AJAX
    public class MatricularRequest
    {
        public int CursoId { get; set; }
    }
}