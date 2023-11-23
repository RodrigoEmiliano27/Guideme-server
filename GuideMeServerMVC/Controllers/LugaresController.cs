using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Services;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Controllers
{
    public class LugaresController : ControllerAutenticado<LugaresViewModel>
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public LugaresController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _service = new LugarService(_context);
        }


        public async  Task<IActionResult> Save(LugaresViewModel model, string Operacao)
        {
            try
            {
               
                Dictionary<string,string> erros = _service.ValidarDados(model);
                ProcessaErros(erros);

                
                if (ModelState.IsValid == false)
                {
                    model.TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session,_context);
                    ViewBag.Operacao = Operacao;
                    return View("Form", model);
                }
                else
                {

                    if (Operacao == "I")
                        await _service.SaveAsync(model);
                    else
                        await _service.UpdateAsync(model);


                    return RedirectToAction("Index", "Lugares");
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {            
            try
            {
                await _service.Delete(id, (int)_UsuarioLogado);
                              
                return RedirectToAction("Index", "Lugares");
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

        }


        public virtual async Task<IActionResult> Edit(int id)
        {
            try
            {
                TagService serviceTag = new TagService(_context);
                ViewBag.Operacao = "A";
                var lugar = await _service.GetById(id);
                if (lugar != null)
                {
                    var tagCadastrada = await serviceTag.GetById(lugar.TAG_id);
                    lugar.TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session, _context);
                    lugar.TagsDiponiveis.Insert(0, new SelectListItem(tagCadastrada.Nome, tagCadastrada.Id.ToString()));

                    return View("Form", lugar);
                }

                else
                    return View("Index", "Home");
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        


       

        public async virtual Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                int idUsuario = HelperControllers.GetUserLogadoID(HttpContext.Session);
                return View("Form", new LugaresViewModel() { TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session, _context) });
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }


       
    }
}
