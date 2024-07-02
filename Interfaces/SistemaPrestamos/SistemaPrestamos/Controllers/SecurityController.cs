using SistemaPrestamos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SistemaPrestamos.Context;

namespace SistemaPrestamos.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SecurityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginPost(LoginViewModel model)
        {
            // Intenta convertir el valor ingresado a un entero
            if (int.TryParse(model.User, out int userId))
            {
                var user = _context.Usuario.SingleOrDefault(u => u.CodUsuario == userId && u.Contrasenia == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.CodUsuario.ToString()), // Usar CodUsuario como Claim
                new Claim(ClaimTypes.Role, user.Role)
            };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    HttpContext.Session.SetString("UserLogged", user.CodUsuario.ToString());

                    return RedirectToAction("Index", "Home");
                }
            }

            // Si la conversión falla o el usuario no se encuentra, muestra el formulario de inicio de sesión nuevamente
            return View("Login");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Security");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
