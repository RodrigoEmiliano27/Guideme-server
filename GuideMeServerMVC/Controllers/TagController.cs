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
using GuideMeServerMVC.Services;
//using GuideMeServerMVC.Utils;

namespace GuideMeServerMVC.Controllers
{
    public class TagController : ControllerAutenticado<LugaresViewModel>
    {
        private readonly GuidemeDbContext _context;
        private readonly TagService _service;
        private readonly TagsPaiService _serviceTagsPai;
        public TagController(GuidemeDbContext context)
        {
            _context = context;
            _service = new TagService(_context);
            _serviceTagsPai = new TagsPaiService(_context);
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                return View("TagsEstabelecimento", await _service.GetTagsUsuario((int)_UsuarioLogado));
            }
             catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

      


        protected virtual void ValidaDados(CadastrarAssociacaoTagsTO model,out TagViewModel _menorTag,out TagViewModel _maiorTag)
        {
            ModelState.Clear();
            _menorTag = null;
            _maiorTag = null;
            var erros =_service.ValidarDadosAssociacaoTags(model,out _menorTag,out _maiorTag);
            if (erros != null && erros.Count > 0)
            {
                foreach (KeyValuePair<string, string> erro in erros)
                    ModelState.AddModelError(erro.Key, erro.Value);

            }

        }

        public async Task<IActionResult> SaveAssociacaoTags(CadastrarAssociacaoTagsTO model)
        {
            try
            {

                VerificaUserLogado();
 
                TagViewModel menorTag = null, maiorTag=null;

                ValidaDados(model, out menorTag, out maiorTag);

                if (ModelState.IsValid == false)
                    return View("AssociarTags", await _service.GetDadosCadastroAssociacao(HelperControllers.GetUserLogadoID(HttpContext.Session)));
                else
                {
                    await _service.SalvarAssociacao(model, menorTag, maiorTag);

                    return View("TagsEstabelecimento", await _service.GetTagsUsuario((int)_UsuarioLogado));
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
                ContainerAssociacaoTagsTO container = await _service.GetAssociacoesTag(Id);

                if(container != null)
                    return View("RelacionamentoTags", container);

                return View("TagsEstabelecimento", await _service.GetTagsUsuario((int)_UsuarioLogado));
                
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

        public async Task<IActionResult> ExibirTagsEstabelecimento()
        {

            try
            {
                Debug.WriteLine("Listando Tags");
                return View("TagsEstabelecimento", await _service.GetTagsUsuario((int)_UsuarioLogado));
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
                await _service.UpdateAsync(model);

                return View("TagsEstabelecimento", await _service.GetTagsUsuario((int)_UsuarioLogado));
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
               await _service.Delete(id,(int)_UsuarioLogado);

                return RedirectToAction("ExibirTagsEstabelecimento", "Tag");
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
               await _serviceTagsPai.DeletarRelacionamentos(idTagPrincipal, idTagSecundaria);
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
                return View("AssociarTags", await  _service.GetDadosCadastroAssociacao(HelperControllers.GetUserLogadoID(HttpContext.Session)));
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

     
    }
   

}
