using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaPrestamos.Controllers
{
    public class ActionUserController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (HttpContext.Session.GetString("UserLogged") == null)
            {
                context.Result = this.RedirectToAction("Login", "Security");
            }


            base.OnActionExecuting(context);
        }


    }
}