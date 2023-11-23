using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Services;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GuideMeServerMVC.Controllers
{
    public class ControllerAutenticado<T>: ControllerBasico<T> where T : BaseViewModel
    {
        protected int? _UsuarioLogado = null;

        protected void VerificaUserLogado()
        {
            if(_UsuarioLogado==null || _UsuarioLogado<=0)
                RedirectToAction("Login", "Login");

        }

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
