using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GuideMeServerMVC.Controllers
{
    public class ControllerAutenticado:Controller
    {
        protected int? _UsuarioLogado = null;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HelperControllers.VerificaUserLogado(HttpContext.Session))
                context.Result = RedirectToAction("Login", "Login");
            else
            {
                _UsuarioLogado = HelperControllers.GetUserLogadoID(HttpContext.Session);
                ViewBag.Logado = true;
                base.OnActionExecuting(context);
            }
        }
    }
}
