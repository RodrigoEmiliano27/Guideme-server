using GuideMeServerMVC.Data;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.TO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
        [Authorize]
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
    }
}
