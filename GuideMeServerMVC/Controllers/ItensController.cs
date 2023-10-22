﻿using GuideMeServerMVC.Data;
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
        private readonly ItensService _service;

        public ItensController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _service = new ItensService(_context);

        }

        public async Task<IActionResult> Index()
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
