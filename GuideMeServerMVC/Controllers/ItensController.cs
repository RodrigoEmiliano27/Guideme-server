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
    public class ItensController : ControllerAutenticado<ItensViewModel>
    {
        private readonly IConfiguration _configuration;

        public ItensController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;         
            _context = context;
            _service = new ItensService(_context);

        }


        public async Task<IActionResult> Index()
        {
            try
            {
                int idUsuario = HelperControllers.GetUserLogadoID(HttpContext.Session);

                if (idUsuario != 0)
                {
                    List<ItensViewModel> itens = new List<ItensViewModel>();
                    var _user = _context.UsuariosEstabelecimento.AsNoTracking().FirstOrDefault(x => x.Id == idUsuario);
                    if (_user != null)
                    {

                        var tagItem = _context.Tags.Where(o => o.EstabelecimentoId == _user.Id_Estabelecimento && o.tipoTag == (int)EnumTipoTag.lugar).ToList();
                        foreach (var tag in tagItem)
                        {
                            var item = _context.Itens.AsNoTracking().FirstOrDefault(x => x.TAG_id == tag.Id);
                            if (item != null)
                                itens.Add(item);

                        }
                    }
                    return View("Index", itens);
                }

                return RedirectToAction("Home", "index");

            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

        }




        public async  Task<IActionResult> Save(ItensViewModel model, string Operacao)
        {
            try
            {
               
                ModelState.Clear();
                Dictionary<string, string> erros = _service.ValidarDados(model);
                ProcessaErros(erros);

                if (ModelState.IsValid == false)
                {
                    model.TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session, _context);
                    ViewBag.Operacao = Operacao;
                    return View("Form", model);
                }
                else
                {

                    if (Operacao == "I")
                        await _service.SaveAsync(model);
                    else
                        await _service.UpdateAsync(model);


                    return RedirectToAction("Index","Itens");
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
                return RedirectToAction("Index", "Itens");
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
                ViewBag.Operacao = "A";

                var item = await _service.GetById(id);
                if (item != null)
                {
                    TagService _serviceTag = new TagService(_context);

                    var tagCadastrada = await _serviceTag.GetById(item.TAG_id);
                    item.TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session, _context);
                    item.TagsDiponiveis.Insert(0, new SelectListItem(tagCadastrada.Nome, tagCadastrada.Id.ToString()));

                    return View("Form", item);
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
                return View("Form", new ItensViewModel() { TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session, _context) });
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }


       
    }
}
