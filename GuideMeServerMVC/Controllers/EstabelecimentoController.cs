using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.TO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

            return View("Login", new UsuarioEstabelecimentoModel());
            // return View("Login", new LoginRequestTO());
        }

        [HttpGet("CadastrarEstabelecimento")]
        public IActionResult CadastrarEstabelecimento()
        {
            Debug.WriteLine("Chamou a tela de Cadastro de Estabelecimento!");
            return View("CadastroEstabelecimento", new EstabelecimentoViewModel());
        }

        [HttpPost("Create")]
        public IActionResult Create([FromForm] EstabelecimentoViewModel estabelecimento)
        {
            Debug.WriteLine("CreateEstabelecimento");
            //Cadastrar o estabelecimento
            _context.Estabelecimento.Add(estabelecimento);
            _context.SaveChanges();
            Debug.WriteLine("Estabelecimento criado");

            //Pegar o id do estabelecimento inserido e relaciona-lo ao usuario
            Debug.WriteLine("Estabelecimento Id => " + estabelecimento.Id);
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            user.Id_Estabelecimento = estabelecimento.Id;
            _context.UsuariosEstabelecimento.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index", "UsuarioEstabelecimento");
        }
    }
}
