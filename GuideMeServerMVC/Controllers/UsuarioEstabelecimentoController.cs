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

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioEstabelecimentoController : Controller
    {
        private readonly GuidemeDbContext _context;

        public UsuarioEstabelecimentoController(GuidemeDbContext context)
        {
            _context = context;
        }

       

        [HttpGet("Index")]
        public IActionResult Index()
        {
            Debug.WriteLine("Chamou a tela de Index!");
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            EstabelecimentoViewModel estabelecimento = new EstabelecimentoViewModel();
            try
            {
                estabelecimento = _context.Estabelecimento.FirstOrDefault(o => o.Id == user.Id_Estabelecimento);
            }catch(Exception ex) { estabelecimento = null; }

            MenuViewModel menuModel = new MenuViewModel();
            menuModel.UsuarioEstabelecimento = user;
            menuModel.Estabelecimento = estabelecimento;
            
            return View("Menu", menuModel);
        }

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("Cadastro")]
        public IActionResult Cadastro()
        {
            Debug.WriteLine("Chamou a tela de Cadastro!");
            return View("Cadastro", new UsuarioEstabelecimentoModel());
            // return View("Login", new LoginRequestTO());
        }

        [HttpPost("create")]
        public ActionResult<object> CreateUsuario(UsuarioEstabelecimentoModel usuario)
        {

            _context.UsuariosEstabelecimento.Add(usuario);
            _context.SaveChanges();
            return Ok("Teste Post");
        }

        

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HelperControllers.VerificaUserLogado(HttpContext.Session))
                context.Result = RedirectToAction("Login", "UsuarioEstabelecimento");
            else
            {
                ViewBag.Logado = true;
                base.OnActionExecuting(context);
            }
        }
    }
}
