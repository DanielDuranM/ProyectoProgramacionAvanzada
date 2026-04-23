using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal_DuranDaniel.Data;
using ProyectoFinal_DuranDaniel.Models;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // Verifico si el usuario sea administrador
        private bool EsAdministrador()
            => HttpContext.Session.GetString("UsuarioRol") == "Administrador";

        //MENÚ 

        public IActionResult Index()
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // DOCENTES

        public async Task<IActionResult> Docentes()
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var docentes = await _db.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Docente")
                .ToListAsync();

            return View(docentes);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDocente(
            string nombre, string apellido, string correoPersonal,
            string identificacion, string telefono, string contrasena)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            _db.Usuarios.Add(new Usuario
            {
                Nombre         = nombre,
                Apellido       = apellido,
                CorreoPersonal = correoPersonal,
                Identificacion = identificacion,
                Telefono       = telefono,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasena),
                RolId          = 2  // Docente
            });
            await _db.SaveChangesAsync();
            return RedirectToAction("Docentes");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDocente(int id)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var docente = await _db.Usuarios.FindAsync(id);
            if (docente != null)
            {
                _db.Usuarios.Remove(docente);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Docentes");
        }

        //ESTUDIANTES 

        public async Task<IActionResult> Estudiantes()
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var estudiantes = await _db.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Estudiante")
                .ToListAsync();

            return View(estudiantes);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarEstudiante(int id)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var estudiante = await _db.Usuarios.FindAsync(id);
            if (estudiante != null)
            {
                _db.Usuarios.Remove(estudiante);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Estudiantes");
        }

        //CARRERAS

        public async Task<IActionResult> Carreras()
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");
            return View(await _db.Carreras.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCarrera(string nombre, string descripcion)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            _db.Carreras.Add(new Carrera { Nombre = nombre, Descripcion = descripcion });
            await _db.SaveChangesAsync();
            return RedirectToAction("Carreras");
        }

        [HttpPost]
        public async Task<IActionResult> EditarCarrera(int id, string nombre, string descripcion)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var carrera = await _db.Carreras.FindAsync(id);
            if (carrera != null)
            {
                carrera.Nombre      = nombre;
                carrera.Descripcion = descripcion;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Carreras");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCarrera(int id)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var carrera = await _db.Carreras.FindAsync(id);
            if (carrera != null)
            {
                _db.Carreras.Remove(carrera);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Carreras");
        }

        //CURSOS

        public async Task<IActionResult> Cursos()
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var cursos = await _db.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .ToListAsync();

            ViewBag.Carreras = await _db.Carreras.ToListAsync();
            ViewBag.Docentes = await _db.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Docente")
                .ToListAsync();

            return View(cursos);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCurso(
            string nombre, string descripcion, int carreraId, int docenteId)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            _db.Cursos.Add(new Curso
            {
                Nombre      = nombre,
                Descripcion = descripcion,
                CarreraId   = carreraId,
                DocenteId   = docenteId
            });
            await _db.SaveChangesAsync();
            return RedirectToAction("Cursos");
        }

        [HttpPost]
        public async Task<IActionResult> EditarCurso(
            int id, string nombre, string descripcion, int carreraId, int docenteId)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var curso = await _db.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Nombre      = nombre;
                curso.Descripcion = descripcion;
                curso.CarreraId   = carreraId;
                curso.DocenteId   = docenteId;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Cursos");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCurso(int id)
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");

            var curso = await _db.Cursos.FindAsync(id);
            if (curso != null)
            {
                _db.Cursos.Remove(curso);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Cursos");
        }

        //ROLES 

        public async Task<IActionResult> Roles()
        {
            if (!EsAdministrador()) return RedirectToAction("Login", "Auth");
            return View(await _db.Roles.ToListAsync());
        }
    }
}