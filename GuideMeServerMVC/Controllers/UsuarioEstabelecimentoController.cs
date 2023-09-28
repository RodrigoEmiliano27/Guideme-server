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
using static System.Net.Mime.MediaTypeNames;

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

        //[HttpGet("CadastrarEstabelecimento")]
        public IActionResult CadastrarEstabelecimento()
        {
            EstabelecimentoController estabelecimentoController = new EstabelecimentoController(_context);
            Debug.WriteLine("Chamou a tela de Cadastro de estabelecimento!");
            return estabelecimentoController.CadastrarEstabelecimento();


            //return View("CadastroEstabelecimento", new EstabelecimentoViewModel());
           // return View("Cadastro", new EstabelecimentoViewModel());
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
            //Valida usuario
            bool isUsernamePasswordValid = false;
            if (usuario != null)
            {
                Debug.WriteLine("Entrou no login");
                //var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Login == usuario.Login && o.Senha == usuario.Senha);
                var users = _context.UsuariosEstabelecimento.ToList();
                Debug.WriteLine("users => " + users);
                var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Login == usuario.Login);

                isUsernamePasswordValid = user != null ? true : false;

                if (isUsernamePasswordValid)
                {
                    Debug.WriteLine("Logou krai");
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    // Recupere o ID do usuário da sessão
                    var userId = HttpContext.Session.GetInt32("UserId");
                    Debug.WriteLine("userId => " + userId);
                    return RedirectToAction("Index");
                    //return Ok("Login realizado");
                }
                else
                {
                    Debug.WriteLine("N achou");
                    return RedirectToAction("Error");
                    //return NotFound();
                }
            }
            else return RedirectToAction("Error");
        }
    }
}
