using SistemaPrestamos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppStore.Controllers
{
    public class SecurityController : Controller
    {

        public IActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult LoginPost(LoginViewModel model)
        {

            if (model.User == "admin" && model.Password == "123")
            {

                HttpContext.Session.SetString("UserLogged", model.User);

                return RedirectToAction("Index", "Home");
            }
            return View("Login");
        }
    }
}