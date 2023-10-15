using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GuideMeServerMVC.Controllers
{
    public class LugaresController : ControllerAutenticado
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public LugaresController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                return View("Index", _context.Lugares.AsNoTracking().ToList());
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public async  Task<IActionResult> Save(LugaresViewModel model, string Operacao,int Id, List<SelectListItem> Itens)
        {
            try
            {
                ValidaDados(model, Operacao);
                if (ModelState.IsValid == false)
                {
                    model.TagsDiponiveis = await HelperControllers.GetListaTags(HttpContext.Session,_context);
                    ViewBag.Operacao = Operacao;
                    return View("Form", model);
                }
                else
                {
                   
                    var tagSelecionada = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.TAG_id);
                    if (tagSelecionada!=null)
                    {
                        using var transaction = _context.Database.BeginTransaction();
                        try
                        {
                            if (Operacao == "I")
                            {
                                TagViewModel newTag = tagSelecionada;
                                newTag.tipoTag = (int)EnumTipoTag.lugar;

                                _context.Update(newTag);
                                await _context.SaveChangesAsync();

                                _context.Add(model);
                                await _context.SaveChangesAsync();

                                transaction.Commit();
                            }
                            else
                            {
                                var lugaOld = await _context.Lugares.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
                                if (lugaOld != null)
                                {
                                   var tagAntiga = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lugaOld.TAG_id);
                                    if (tagAntiga != null)
                                    {

                                        if (tagAntiga.Id != tagSelecionada.Id)
                                        {
                                            TagViewModel newTag = tagAntiga;
                                            newTag.tipoTag = (int)EnumTipoTag.NaoCadastrada;

                                            _context.Update(newTag);
                                            await _context.SaveChangesAsync();

                                            newTag = tagSelecionada;
                                            newTag.tipoTag = (int)EnumTipoTag.lugar;

                                            _context.Update(newTag);
                                            await _context.SaveChangesAsync();
                                        }
                                       
                                        _context.Update(model);
                                        await _context.SaveChangesAsync();

                                        transaction.Commit();

                                    }
                                }

                            }
                        }
                        catch (Exception err)
                        {
                            await transaction.RollbackAsync();
                        }
                        finally
                        {
                            transaction.Dispose();
                        }
                    }
                    
                  

                    return RedirectToAction("Index","Lugares");
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
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var lugar =await _context.Lugares.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (lugar != null)
                {
                    var tagCadastrada = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lugar.TAG_id);
                    if (tagCadastrada != null)
                    {
                        TagViewModel newTag = tagCadastrada;
                        newTag.tipoTag = (int)EnumTipoTag.NaoCadastrada;

                        _context.Update(newTag);
                        await _context.SaveChangesAsync();

                        await _context.Lugares.Where(x => x.Id == id).ExecuteDeleteAsync();


                        await transaction.CommitAsync();

                    }
                }
                              
                return RedirectToAction("Index", "Lugares");
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }


        public virtual async Task<IActionResult> Edit(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                var lugar = await _context.Lugares.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (lugar != null)
                {
                    var tagCadastrada = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lugar.TAG_id);
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

        

        protected virtual void ValidaDados(LugaresViewModel model, string operacao)
        {
            ModelState.Clear();

            if(model.TAG_id <= 0)
                ModelState.AddModelError("TagSelecionada", "Por favor selecione uma tag disponível!");

            if (string.IsNullOrEmpty(model.Nome) || string.IsNullOrEmpty(model.Nome.Trim()))
                ModelState.AddModelError("Nome", "Nome inválido!");

            if (string.IsNullOrEmpty(model.Descricao) || string.IsNullOrEmpty(model.Descricao.Trim()))
                ModelState.AddModelError("Descricao", "Descricao inválida!");

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
