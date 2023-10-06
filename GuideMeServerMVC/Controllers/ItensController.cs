using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GuideMeServerMVC.Controllers
{
    public class ItensController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public ItensController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                return View("Index", _context.Itens.AsNoTracking().ToList());
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }


            return View("Index");
        }

        public async  Task<IActionResult> Save(ItensViewModel model, string Operacao,int Id, List<SelectListItem> Itens)
        {
            try
            {
                ValidaDados(model, Operacao);
                if (ModelState.IsValid == false)
                {
                    model.TagsDiponiveis = await GetListaTags();
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
                                newTag.tipoTag = (int)EnumTipoTag.itens;

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
                                        TagViewModel newTag = tagAntiga;
                                        newTag.tipoTag = (int)EnumTipoTag.NaoCadastrada;

                                        _context.Update(newTag);
                                        await _context.SaveChangesAsync();

                                        newTag = tagSelecionada;
                                        newTag.tipoTag = (int)EnumTipoTag.itens;

                                        _context.Update(newTag);
                                        await _context.SaveChangesAsync();

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
                    
                  

                    return RedirectToAction("Index","Itens");
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
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
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
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
                    lugar.TagsDiponiveis = await GetListaTags();
                    lugar.TagsDiponiveis.Insert(0, new SelectListItem(tagCadastrada.Nome, tagCadastrada.Id.ToString()));

                    return View("Form", lugar);
                }

                else
                    return View("Index", "Home");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        

        protected virtual void ValidaDados(ItensViewModel model, string operacao)
        {
            ModelState.Clear();

            if(model.TAG_id <= 0)
                ModelState.AddModelError("TagSelecionada", "Por favor selecione uma tag disponível!");

            if (string.IsNullOrEmpty(model.Nome) || string.IsNullOrEmpty(model.Nome.Trim()))
                ModelState.AddModelError("Nome", "Nome inválido!");

            if (string.IsNullOrEmpty(model.Descricao) || string.IsNullOrEmpty(model.Descricao.Trim()))
                ModelState.AddModelError("Descricao", "Descricao inválida!");

        }

        private async Task<List<SelectListItem>> GetListaTags()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            try
            {
                int idUsuario = HelperControllers.GetUserLogadoID(HttpContext.Session);
                
                var usuario = await _context.UsuariosEstabelecimento.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idUsuario);
                if (usuario != null)
                {
                    var tagsDisponiveis = await _context.Tags.AsNoTracking().Where(x => x.EstabelecimentoId == usuario.Id_Estabelecimento &&
                    x.tipoTag == (int)EnumTipoTag.NaoCadastrada).ToListAsync();


                    foreach (TagViewModel tag in tagsDisponiveis)
                        lista.Add(new SelectListItem(tag.Nome, tag.Id.ToString()));

                }
            }
            catch (Exception err)
            { 

            }
            return lista;
        }

        public async virtual Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                int idUsuario = HelperControllers.GetUserLogadoID(HttpContext.Session);
                return View("Form", new ItensViewModel() { TagsDiponiveis = await GetListaTags() });
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HelperControllers.VerificaUserLogado(HttpContext.Session))
                context.Result = RedirectToAction("Login", "Login");
            else
            {
                ViewBag.Logado = true;
                base.OnActionExecuting(context);
            }
        }
    }
}
