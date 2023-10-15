using Microsoft.AspNetCore.Mvc;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using GuideMeServerMVC.Data;
using GuideMeServerMVC.TO;
using System.Security.Claims;
using System.Diagnostics;
using Newtonsoft.Json;
using GuideMeServerMVC.Enum;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using Azure;
using GuideMeServerMVC.Utils;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
//using GuideMeServerMVC.Utils;

namespace GuideMeServerMVC.Controllers
{
    public class TagController : ControllerAutenticado
    {
        private readonly GuidemeDbContext _context;
        public TagController(GuidemeDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            try
            {
                return View("TagsEstabelecimento", _context.Tags.AsNoTracking().ToList());
            }
             catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

      
        protected async Task<CadastrarAssociacaoTagsTO> PreparaDadosParaView()
        {
            var tagsDisponiveis = await HelperControllers.GetListaTags(HttpContext.Session, _context, EnumTipoTag.lugar);
            var direcoesDisponiveis = System.Enum.GetNames(typeof(EnumDirecao)).ToList();
            CadastrarAssociacaoTagsTO associacaoTagsTO = new CadastrarAssociacaoTagsTO();
            associacaoTagsTO.DirecoesDisponiveis = HelperControllers.GetListaDirecoes();
            associacaoTagsTO.TagsDiponiveis = tagsDisponiveis;
            return associacaoTagsTO;
        }

        protected virtual void ValidaDados(CadastrarAssociacaoTagsTO model,out TagViewModel _menorTag,out TagViewModel _maiorTag)
        {
            ModelState.Clear();
            int menorTag = 0;
            int maiorTag = 0;
            string menorTagNome = "";
            string maiorTagNome = "";
            _menorTag = null;
            _maiorTag = null;

            if (model.TagPrincipalSelecionada <= 0)
                ModelState.AddModelError("TagPrincipalSelecionada", "Por favor selecione uma tag disponível!");

            if (model.TagSecundariaSelecionada <= 0)
                ModelState.AddModelError("TagSecundariaSelecionada", "Por favor selecione uma tag disponível!");

            if (model.TagSecundariaSelecionada == model.TagPrincipalSelecionada)
            {
                ModelState.AddModelError("TagPrincipalSelecionada", "A tag principal e a secundaria não podem ser as mesmas!");
                ModelState.AddModelError("TagSecundariaSelecionada", "A tag principal e a secundaria não podem ser as mesmas!");
            }


            if (model.TagPrincipalSelecionada < model.TagSecundariaSelecionada)
            {
                menorTag = model.TagPrincipalSelecionada;
                menorTagNome = "TagPrincipalSelecionada";
                maiorTag = model.TagSecundariaSelecionada;
                maiorTagNome = "TagSecundariaSelecionada";

            }
            else
            {
                menorTag = model.TagSecundariaSelecionada;
                menorTagNome = "TagSecundariaSelecionada";
                maiorTag = model.TagPrincipalSelecionada;
                maiorTagNome = "TagPrincipalSelecionada";
            }


            if (model.TagPrincipalSelecionada > 0 && model.TagSecundariaSelecionada > 0 &&
                model.TagSecundariaSelecionada != model.TagPrincipalSelecionada)
            {
                _menorTag = _context.Tags.AsNoTracking().FirstAsync(x => x.Id == menorTag).Result;
                _maiorTag = _context.Tags.AsNoTracking().FirstAsync(x => x.Id == maiorTag).Result;

                if(_menorTag==null)
                    ModelState.AddModelError(menorTagNome, "Tag não foi encontrada!");
                if(_maiorTag==null)
                    ModelState.AddModelError(maiorTagNome, "Tag não foi encontrada!");

             

            }
            


        }

        public async Task<IActionResult> Save(CadastrarAssociacaoTagsTO model)
        {
            try
            {
 
                TagViewModel menorTag = null, maioTag=null;

                ValidaDados(model, out menorTag, out maioTag);

                if (ModelState.IsValid == false)
                    return View("AssociarTags", await PreparaDadosParaView());
                else
                {
                    var TagRelacionamento = await  _context.TagsPai.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Tag_Pai == menorTag.Id
                    && x.Id_Tag == maioTag.Id);

                    TagsPaiViewModel tagPai = new TagsPaiViewModel();
                    tagPai.Id_Tag_Pai = menorTag.Id;
                    tagPai.Id_Tag = maioTag.Id;
                    tagPai.Direcao = model.DirecaoSelecionada;


                    if (TagRelacionamento == null)
                    {                     
                        _context.Add(tagPai);
                        await _context.SaveChangesAsync();
                    }
                    else 
                    {
                        tagPai.Id=TagRelacionamento.Id; 
                        _context.Update(tagPai);
                        await _context.SaveChangesAsync();
                    }

                    if (HttpContext.Session.GetInt32("UserId") != null)
                    {
                        var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                        var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();
                        return View("TagsEstabelecimento", tags);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public async Task<IActionResult> VisualizarRelacionamentos(int Id)
        {
            try
            {
                TagViewModel model = await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
                if (model != null)
                {
                    List<AssociacaoTagTO> lista = new List<AssociacaoTagTO>();
                    var tagsRelacionamento = _context.TagsPai.AsNoTracking().Where(x => x.Id_Tag_Pai == model.Id || x.Id_Tag == model.Id).ToList();
                    foreach (var relacionamento in tagsRelacionamento)
                    {
                        var TagPrincipal = await _context.Tags.AsNoTracking().FirstAsync(x => x.Id == relacionamento.Id_Tag_Pai);
                        var TagSecundaria = await _context.Tags.AsNoTracking().FirstAsync(x => x.Id == relacionamento.Id_Tag);

                        if (TagPrincipal != null && TagSecundaria != null)
                            lista.Add(new AssociacaoTagTO() {TagPrincipal = TagPrincipal, TagSecundaria = TagSecundaria, Direcao = (EnumDirecao)relacionamento.Direcao });
                    }

                    return View("RelacionamentoTags", new ContainerAssociacaoTagsTO() { TagOriginal=model, Relacionamentos=lista});

                }
                else
                {
                    if (HttpContext.Session.GetInt32("UserId") != null)
                    {
                        var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                        var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();
                        return View("TagsEstabelecimento", tags);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }

                
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }


        public virtual async Task<IActionResult> TelaEditar(int Id)
        {

            try
            {
                Debug.WriteLine("TelaEditar");
                var tag = _context.Tags.FirstOrDefault(o => o.Id == Id);
                if (tag != null)
                {
                    return View("EditarTag", tag);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
            
        }

        public IActionResult ExibirTagsEstabelecimento()
        {

            try
            {
                Debug.WriteLine("Listando Tags");
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                    var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();
                    return View("TagsEstabelecimento", tags);
                }
                else
                {
                    return RedirectToAction("Login", "Login");
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

        }

        public virtual async Task<IActionResult> UpdateTag([FromForm] TagViewModel model)
        {


            try
            {
                _context.Update(model);
                _context.SaveChanges();

                var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();


                return View("TagsEstabelecimento", tags);
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
                using var transaction = _context.Database.BeginTransaction();
                Debug.WriteLine("Chamou o DeleteTag");
                Debug.WriteLine("Id => " + id);
                var tag = _context.Tags.AsNoTracking().FirstOrDefault(o => o.Id == id);
                if (tag != null)
                {
                    tag.TagsPai = null;
                    tag.EstabelecimentoId = 0;
                    tag.tipoTag = (int)EnumTipoTag.NaoCadastrada;
                    _context.Update(tag);
                    await _context.SaveChangesAsync();

                    _context.Tags.Remove(tag);

                    await transaction.CommitAsync();
                    //_context.SaveChanges();
                    return RedirectToAction("ExibirTagsEstabelecimento", "Tag"); ;
                }
                else
                {
                    return NotFound("tag não encontrada");
                }
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }


           
        }
        public async Task<IActionResult> DeleteRelacionamento(int idTagPrincipal, int idTagSecundaria)
        {

            try
            {
                var relacionamento = await _context.TagsPai.AsNoTracking().FirstOrDefaultAsync(x => x.Id_Tag_Pai == idTagPrincipal && x.Id_Tag == idTagSecundaria);
                if (relacionamento != null)
                {
                    _context.Remove(relacionamento);
                    await _context.SaveChangesAsync();
                }
               

            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

            return RedirectToAction("ExibirTagsEstabelecimento", "Tag");

        }
        public async Task<IActionResult> AssociarTags()
        {
            try
            {
                return View("AssociarTags", await PreparaDadosParaView());
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

     
    }
   

}
