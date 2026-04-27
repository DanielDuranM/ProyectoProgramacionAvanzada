using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin() =>
            HttpContext.Session.GetString("UsuarioRol") == "Administrador";

        // GET: /Admin/Index
        public IActionResult Index()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // ─── DOCENTES ────────────────────────────────────────────────────────

        // GET: /Admin/Docentes
        public async Task<IActionResult> Docentes()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Auth");

            var docentes = await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.RolId == 2)
                .ToListAsync();

            return View(docentes);
        }

        // POST: /Admin/AgregarDocente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarDocente(string nombre, string apellido,
            string correoPersonal, string identificacion, string telefono, string contrasena)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Auth");

            if (await _context.Usuarios.AnyAsync(u => u.CorreoPersonal == correoPersonal))
            {
                TempData["Error"] = "Ese correo ya está registrado.";
                return RedirectToAction("Docentes");
            }

            _context.Usuarios.Add(new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                CorreoPersonal = correoPersonal,
                Identificacion = identificacion,
                Telefono = telefono,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasena),
                RolId = 2
            });
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Docente agregado correctamente.";
            return RedirectToAction("Docentes");
        }

        // POST: /Admin/EliminarDocente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDocente(int id)
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Auth");

            var docente = await _context.Usuarios.FindAsync(id);
            if (docente != null)
            {
                _context.Usuarios.Remove(docente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Docentes");
        }

        // ─── ESTUDIANTES ─────────────────────────────────────────────────────

        // GET: /Admin/Estudiantes
        public async Task<IActionResult> Estudiantes()
        {
            if (!EsAdmin()) return RedirectToAction("Login", "Auth");

            var estudiantes = await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.RolId == 3)
                .ToListAsync();

            return View(estudiantes);
        }
    }
}