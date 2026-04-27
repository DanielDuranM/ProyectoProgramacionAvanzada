using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Register
        public IActionResult Register() => View();

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string nombre, string apellido,
            string correoPersonal, string identificacion, string telefono, string contrasena)
        {
            if (await _context.Usuarios.AnyAsync(u => u.CorreoPersonal == correoPersonal))
            {
                ViewBag.Error = "Ese correo ya está registrado.";
                return View();
            }

            var usuario = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                CorreoPersonal = correoPersonal,
                Identificacion = identificacion,
                Telefono = telefono,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasena),
                RolId = 3
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Registro exitoso. Iniciá sesión.";
            return RedirectToAction("Login");
        }

        // GET: /Auth/Login
        public IActionResult Login() => View();

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.CorreoPersonal == correo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.ContrasenaHash))
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNombre", $"{usuario.Nombre} {usuario.Apellido}");
            HttpContext.Session.SetString("UsuarioCorreo", usuario.CorreoPersonal);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol.Nombre);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}