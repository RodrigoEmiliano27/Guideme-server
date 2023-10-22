using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuideMeServerMVC.Controllers
{
    public abstract class ControllerBasico
        <T> : Controller where T : BaseViewModel
    {
        protected GuidemeDbContext _context;
        protected ServiceBase<T> _service;
        

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
