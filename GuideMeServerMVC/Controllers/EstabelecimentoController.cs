using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GuideMeServerMVC.Models;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentoController : Controller
    {
        private readonly GuidemeDbContext _context;

        public EstabelecimentoController(GuidemeDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("CadastrarEstabelecimento")]
        public IActionResult CadastrarEstabelecimento()
        {
            Debug.WriteLine("Chamou a tela de Cadastro de estabelecimento!");
            return View("CadastroEstabelecimento", new EstabelecimentoViewModel());
        }

        /*[HttpPost("create")]
        public ActionResult<object> CreateUsuario(UsuarioEstabelecimentoModel usuario)
        {

            _context.UsuariosEstabelecimento.Add(usuario);
            _context.SaveChanges();
            return Ok("Teste Post");
        }*/
    }
}
