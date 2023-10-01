using Microsoft.AspNetCore.Mvc;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using GuideMeServerMVC.Data;
using GuideMeServerMVC.TO;
using System.Security.Claims;
using System.Diagnostics;
//using GuideMeServerMVC.Utils;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly GuidemeDbContext _context;
        public TagController(GuidemeDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet(Name = "GetWeatherForecast"), Authorize]
        //https://localhost:7048/api/Tag/tag?id=1
        /*[HttpGet("tag"), Authorize]
        public TagViewModel GetTag(string id)
        {
            return Ok("TESTE");
        }*/
        [Authorize(Roles = "app")]
        [HttpGet("GetTagData")]
        public ActionResult<object> GetData(string TagID)
        {
            try
            {
                var tagInfo = _context.Tags.FirstOrDefault(x => x.TagId.Equals(TagID));

                if (tagInfo == null)
                    return Ok("Not Found");

                List<TagViewModel> listaTags = 
                    _context.Tags.Where(x => x.EstabelecimentoId == tagInfo.EstabelecimentoId).ToList();

                List<LugaresViewModel> lista = new List<LugaresViewModel>();

                foreach (TagViewModel tag in listaTags)
                {
                    tag.TagsPai = _context.TagsPai.Where(x => x.Id_Tag == tag.Id).ToList();
                    var lugar = _context.Lugares.FirstOrDefault(x => x.TAG_id == tag.Id);
                    if (lugar != null)
                        lista.Add(lugar);
                }

                TagsDataTO data = new TagsDataTO();
                data.Tags = listaTags;
                data.Lugares = lista;
                    

                return Ok(data);
            }
            catch (Exception err)
            {
                return Ok("");
            }
        }
        [Authorize]
        [HttpPost("SalvarTag")]
        public ActionResult<object> SalvaTag(string TagID)
        {
            try
            {
                var tagInfo = _context.Tags.FirstOrDefault(x => x.TagId.Equals(TagID));

                if (tagInfo != null)
                    return BadRequest("Tag já existente");

               // int idUsuario = ClaimsHelper.GetIntClaim(HttpContext.User.Identity as ClaimsIdentity, "id");


                //if(idUsuario==-1)
                   // return NotFound("Usuário não encontrado");


                List<TagViewModel> listaTags =
                    _context.Tags.Where(x => x.EstabelecimentoId == tagInfo.EstabelecimentoId).ToList();

                List<LugaresViewModel> lista = new List<LugaresViewModel>();

                foreach (TagViewModel tag in listaTags)
                {
                    tag.TagsPai = _context.TagsPai.Where(x => x.Id_Tag == tag.Id).ToList();
                    var lugar = _context.Lugares.FirstOrDefault(x => x.TAG_id == tag.Id);
                    if (lugar != null)
                        lista.Add(lugar);
                }

                TagsDataTO data = new TagsDataTO();
                data.Tags = listaTags;
                data.Lugares = lista;


                return Ok(data);
            }
            catch (Exception err)
            {
                return Ok("");
            }
        }

        [HttpGet("ExibirTagsEstabelecimento")]
        public IActionResult ExibirTagsEstabelecimento()
        {
            Debug.WriteLine("Listando Tags");
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();


            return View("Menu", menuModel);
        }
    }
}
