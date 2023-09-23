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

        [HttpGet("Login")]
        public IActionResult Login()
        {
            System.Diagnostics.Debug.WriteLine("Chamou a tela de Login!");
            return View("Login", new UsuarioEstabelecimentoModel());
           // return View("Login", new LoginRequestTO());
        }
        [HttpGet("Index")]
        public IActionResult Index()
        {
            Debug.WriteLine("Chamou a tela de Index!");
            return View("Menu");
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

        [HttpPost("FazLogin")]
        public IActionResult FazLogin([FromForm] UsuarioEstabelecimentoModel usuario)
        {
            System.Diagnostics.Debug.WriteLine("Testei");
            //Valida usuario
            bool isUsernamePasswordValid = false;
            if (usuario != null)
            {
                System.Diagnostics.Debug.WriteLine("Entrou no login");
                var teste = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Login == usuario.Login && o.Senha == usuario.Senha);
                isUsernamePasswordValid = teste != null ? true : false;
            }
            if(isUsernamePasswordValid)
            {
                System.Diagnostics.Debug.WriteLine("Logou krai");
                return RedirectToAction("Index");
                //return Ok("Login realizado");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("N achou");
                return RedirectToAction("Error");
                //return NotFound();
            }
        }
    }
}
