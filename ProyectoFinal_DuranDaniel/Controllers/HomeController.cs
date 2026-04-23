using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal_DuranDaniel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(rol))
                return RedirectToAction("Login", "Auth");

            ViewBag.Nombre = HttpContext.Session.GetString("UsuarioNombre");
            ViewBag.Correo = HttpContext.Session.GetString("UsuarioCorreo");
            ViewBag.Rol = rol;

            return View();
        }
    }
}