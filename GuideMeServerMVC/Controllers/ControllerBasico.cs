using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Services;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace GuideMeServerMVC.Controllers
{
    public abstract class ControllerBasico
        <T> : Controller where T : BaseViewModel
    {
        protected GuidemeDbContext _context;
        protected ServiceBase<T> _service;

        public async virtual Task<IActionResult> Index()
        {
            try
            {
                return View("Index", await _service.GetAll());
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

        }



        public virtual void ProcessaErros(Dictionary<string, string> erros)
        {
            ModelState.Clear();

            if (erros != null && erros.Count > 0)
            {
                foreach (KeyValuePair<string, string> erro in erros)
                    ModelState.AddModelError(erro.Key, erro.Value);
            }
        }
    }
}
