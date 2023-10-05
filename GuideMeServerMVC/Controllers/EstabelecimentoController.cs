using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstabelecimentoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public EstabelecimentoController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [Authorize(Roles ="estabelecimento")]
        [HttpGet("v1/EstabInfo")]
        public async Task<IActionResult> GetEstabInfo()
        {
            EstabelecimentoViewModel retorno = null;
            try
            {
                var customClaimValue = User.FindFirst("IdEstabelecimento")?.Value;

                if (customClaimValue == null)
                    return StatusCode(StatusCodes.Status401Unauthorized, JsonConvert.SerializeObject(retorno));

                int idPrincipal = Convert.ToInt32(customClaimValue);

                if (idPrincipal <= 0)
                    return BadRequest(JsonConvert.SerializeObject(retorno));


                retorno = _context.Estabelecimento.FirstOrDefault(o => o.Id == idPrincipal);

                if (retorno == null)
                    return StatusCode(StatusCodes.Status404NotFound, JsonConvert.SerializeObject(retorno));

                return Ok(JsonConvert.SerializeObject(retorno));
            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, JsonConvert.SerializeObject(retorno));
            }
        }

        [HttpGet("CadastrarEstabelecimento")]   //Chama a tela de cadastrar
        public IActionResult CadastrarEstabelecimento()
        {
            Debug.WriteLine("Chamou a tela de Cadastro de Estabelecimento!");
            return View("CadastroEstabelecimento", new EstabelecimentoViewModel());
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public IActionResult Create([FromForm] EstabelecimentoViewModel estabelecimento) //Create e Update
        {
            try
            {
                _context.Database.BeginTransaction();
                Debug.WriteLine("estabelecimento -> " + estabelecimento.Id);
                if (estabelecimento.Id == 0) //Create
                {
                    Debug.WriteLine("CreateEstabelecimento");
                    //Cadastrar o estabelecimento
                    _context.Estabelecimento.Add(estabelecimento);
                    _context.SaveChanges();
                    Debug.WriteLine("Estabelecimento criado");

                    //Pegar o id do estabelecimento inserido e relaciona-lo ao usuario
                    Debug.WriteLine("Estabelecimento Id => " + estabelecimento.Id);
                    Debug.WriteLine("HttpContext.Session.GetInt32(UserId) => " + HttpContext.Session.GetInt32("UserId"));
                    var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
                    Debug.WriteLine("user => " + (user != null));
                    if (user != null)
                    {
                        user.Id_Estabelecimento = estabelecimento.Id;
                        _context.UsuariosEstabelecimento.Add(user);
                        _context.SaveChanges();
                        _context.Database.CommitTransaction();
                        Debug.WriteLine("funcionou?");
                        return RedirectToAction("Index", "UsuarioEstabelecimento");
                    }
                    else
                    {
                        return View(new ErrorViewModel { RequestId = "Usuário não encontrado" });
                    }
                }
                else  //Update
                {
                    Debug.WriteLine("Editar Estabelecimento");
                    _context.Estabelecimento.Update(estabelecimento);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();
                    Debug.WriteLine("Estabelecimento Atualizado");
                }
                return RedirectToAction("Index", "UsuarioEstabelecimento");
            }
            catch (Exception ex)
            {
                // Em caso de erro, desfaz todas as alterações no banco de dados
                _context.Database.RollbackTransaction();

                Debug.WriteLine("Erro durante a operação: " + ex.Message);
                return View(new ErrorViewModel { RequestId = ex.Message });
            }
            
        }

        [HttpGet("EditarEstabelecimento")]
        [AllowAnonymous]
        public IActionResult EditarEstabelecimento()  //Chama a tela de editar
        {
            Debug.WriteLine("Chamou a tela de Editar Estabelecimento!");
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            if(user != null)
            {
                var estab = _context.Estabelecimento.FirstOrDefault(o => o.Id == user.Id_Estabelecimento);
                if(estab != null)
                {
                    EstabelecimentoViewModel estabelecimento = new EstabelecimentoViewModel();
                    estabelecimento.Id = user.Id_Estabelecimento;
                    estabelecimento.Nome = estab.Nome;

                    return View("CadastroEstabelecimento", estabelecimento);
                }
                else{
                    return View(new ErrorViewModel { RequestId = "Estabelecimento não encontrado" });
                }
            }
            else
            {
                Debug.WriteLine("Usuário não encontrado");
                Debug.WriteLine("user -> " + user);
                return View(new ErrorViewModel { RequestId = "Usuário não encontrado" });
            }
        }
    }
}
