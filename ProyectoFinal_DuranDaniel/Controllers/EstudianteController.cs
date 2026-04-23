using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly AppDbContext _db;

        public EstudianteController(AppDbContext db)
        {
            _db = db;
        }

        private int? ObtenerIdUsuario()
            => HttpContext.Session.GetInt32("UsuarioId");

        // GET: /Estudiante/Carreras — Elegir carrera
        public async Task<IActionResult> Carreras()
        {
            if (ObtenerIdUsuario() == null) return RedirectToAction("Login", "Auth");

            var carreras = await _db.Carreras.ToListAsync();
            return View(carreras);
        }

        // GET: /Estudiante/Cursos?carreraId=1 — Ver cursos de la carrera
        public async Task<IActionResult> Cursos(int carreraId)
        {
            var uid = ObtenerIdUsuario();
            if (uid == null) return RedirectToAction("Login", "Auth");

            var cursos = await _db.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .Where(c => c.CarreraId == carreraId)
                .ToListAsync();

            // IDs de cursos en los que ya está matriculado
            var matriculados = await _db.Matriculas
                .Where(m => m.EstudianteId == uid)
                .Select(m => m.CursoId)
                .ToListAsync();

            ViewBag.Matriculados = matriculados;
            ViewBag.CarreraId = carreraId;
            return View(cursos);
        }

        // POST: /Estudiante/Matricular — Matricularse en un curso
        [HttpPost]
        public async Task<IActionResult> Matricular(int cursoId, int carreraId)
        {
            var uid = ObtenerIdUsuario();
            if (uid == null) return RedirectToAction("Login", "Auth");

            // Verificar si ya está matriculado
            bool yaMatriculado = await _db.Matriculas
                .AnyAsync(m => m.EstudianteId == uid && m.CursoId == cursoId);

            if (!yaMatriculado)
            {
                _db.Matriculas.Add(new Matricula
                {
                    EstudianteId = uid.Value,
                    CursoId = cursoId,
                    FechaMatricula = DateTime.Now,
                    Estado = "Matriculado"
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Cursos", new { carreraId });
        }

        // GET: /Estudiante/MisCursos — Ver cursos en los que está matriculado
        public async Task<IActionResult> MisCursos()
        {
            var uid = ObtenerIdUsuario();
            if (uid == null) return RedirectToAction("Login", "Auth");

            var matriculas = await _db.Matriculas
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Carrera)
                .Include(m => m.Curso)
                    .ThenInclude(c => c.Docente)
                .Where(m => m.EstudianteId == uid)
                .ToListAsync();

            return View(matriculas);
        }
    }
}