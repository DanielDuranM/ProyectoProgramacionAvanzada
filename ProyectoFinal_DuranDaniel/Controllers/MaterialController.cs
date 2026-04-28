using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;
using ProyectoFinal_DuranDaniel.Services;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class MaterialController : Controller
    {
        private readonly AppDbContext _context;
        private readonly FirebaseStorageService _firebase;

        public MaterialController(AppDbContext context, FirebaseStorageService firebase)
        {
            _context = context;
            _firebase = firebase;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UsuarioId");
        private string GetRol() => HttpContext.Session.GetString("UsuarioRol") ?? "";
        private bool EsDocente() => GetRol() == "Docente";
        private bool EsAdmin() => GetRol() == "Administrador";
        private bool EsEstudiante() => GetRol() == "Estudiante";

        // GET /Material/Curso/{cursoId}
        public async Task<IActionResult> Curso(int cursoId)
        {
            if (string.IsNullOrEmpty(GetRol()))
                return RedirectToAction("Login", "Auth");

            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null) return NotFound();

            var uid = GetUserId();

            if (EsEstudiante())
            {
                bool matriculado = await _context.Matriculas
                    .AnyAsync(m => m.EstudianteId == uid && m.CursoId == cursoId);
                if (!matriculado)
                {
                    TempData["Error"] = "No estás matriculado en este curso.";
                    return RedirectToAction("MisCursos", "Estudiante");
                }
            }
            else if (EsDocente() && curso.DocenteId != uid)
            {
                return Forbid();
            }

            var materiales = await _context.MaterialesCurso
                .Include(m => m.SubidoPor)
                .Where(m => m.CursoId == cursoId)
                .OrderByDescending(m => m.FechaSubida)
                .ToListAsync();

            ViewBag.Curso = curso;
            ViewBag.EsDocente = EsDocente() || EsAdmin();
            return View(materiales);
        }

        // POST /Material/Subir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> Subir(int cursoId)
        {
            if (!EsDocente() && !EsAdmin())
                return RedirectToAction("Login", "Auth");

            // Leer el archivo directamente desde Request.Form.Files
            var archivo = Request.Form.Files.GetFile("archivo");

            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debes seleccionar un archivo.";
                return RedirectToAction("Curso", new { cursoId });
            }

            if (archivo.Length > 20 * 1024 * 1024)
            {
                TempData["Error"] = "El archivo no puede superar 20 MB.";
                return RedirectToAction("Curso", new { cursoId });
            }

            var uid = GetUserId()!.Value;

            try
            {
                var url = await _firebase.SubirArchivoAsync(archivo, $"materiales/curso-{cursoId}");

                _context.MaterialesCurso.Add(new MaterialCurso
                {
                    CursoId = cursoId,
                    SubidoPorId = uid,
                    NombreArchivo = Path.GetFileName(archivo.FileName),
                    UrlFirebase = url,
                    TipoArchivo = archivo.ContentType,
                    TamanioBytes = archivo.Length,
                    FechaSubida = DateTime.Now
                });
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = $"'{archivo.FileName}' subido correctamente a Firebase Storage.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al subir: {ex.Message}";
            }

            return RedirectToAction("Curso", new { cursoId });
        }

        // POST /Material/Eliminar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsDocente() && !EsAdmin())
                return RedirectToAction("Login", "Auth");

            var material = await _context.MaterialesCurso
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null) return NotFound();

            var uid = GetUserId();
            if (EsDocente() && material.Curso.DocenteId != uid)
                return Forbid();

            int cursoId = material.CursoId;

            try
            {
                await _firebase.EliminarArchivoAsync(material.UrlFirebase);
                _context.MaterialesCurso.Remove(material);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Material eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
            }

            return RedirectToAction("Curso", new { cursoId });
        }
    }
}