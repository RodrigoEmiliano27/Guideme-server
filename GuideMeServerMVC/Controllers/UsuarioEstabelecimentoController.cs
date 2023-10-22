using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using GuideMeServerMVC.Data;
using GuideMeServerMVC.TO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using GuideMeServerMVC.Utils;
using System.Reflection;

namespace GuideMeServerMVC.Controllers
{
 
    public class UsuarioEstabelecimentoController : ControllerAutenticado<UsuarioEstabelecimentoModel>
    {
        private readonly GuidemeDbContext _context;

        public UsuarioEstabelecimentoController(GuidemeDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {

            try
            {
                Debug.WriteLine("Chamou a tela de Index!");
                var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                EstabelecimentoViewModel estabelecimento = new EstabelecimentoViewModel();
                try
                {
                    estabelecimento = _context.Estabelecimento.FirstOrDefault(o => o.Id == user.Id_Estabelecimento);
                }
                catch (Exception ex) { estabelecimento = null; }

                MenuViewModel menuModel = new MenuViewModel();
                menuModel.UsuarioEstabelecimento = user;
                menuModel.Estabelecimento = estabelecimento;

                return View("Menu", menuModel);
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Cadastro()
        {
            try
            {
                Debug.WriteLine("Chamou a tela de Cadastro!");
                return View("Cadastro", new UsuarioEstabelecimentoModel());
                // return View("Login", new LoginRequestTO());
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

        }

        public async Task<IActionResult> CreateUsuario(UsuarioEstabelecimentoModel usuario)
        {
            try
            {
                _context.UsuariosEstabelecimento.Add(usuario);
                _context.SaveChanges();
                return Ok("Teste Post");
            }
            catch (Exception err)
            {
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err);
                return View("Error", new ErrorViewModel(err.ToString()));
            }

           
        }

       
    }
}
