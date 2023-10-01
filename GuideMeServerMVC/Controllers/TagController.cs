using Microsoft.AspNetCore.Mvc;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using GuideMeServerMVC.Data;
using GuideMeServerMVC.TO;
using System.Security.Claims;
using System.Diagnostics;
using Newtonsoft.Json;
using GuideMeServerMVC.Enum;
using Microsoft.EntityFrameworkCore;
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
        [Authorize(Roles = "estabelecimento")]
        [HttpPost("v1/SalvarTag")]
        public async Task<IActionResult> SalvaTag([FromBody] TagTO tagTO)
        {
            try
            {
                TagViewModel tagmodel = new TagViewModel();
                var tagInfo = _context.Tags.AsNoTracking().FirstOrDefault(x => x.TagId.Equals(tagTO.TagID));

                if (tagInfo != null)
                {
                    if (tagInfo.EstabelecimentoId != tagTO.IdEstabelecimento)
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    tagmodel.Id = tagInfo.Id;
                    tagmodel.Nome = tagTO.TagName;
                    tagmodel.tipoTag = tagInfo.tipoTag;
                    tagmodel.TagId = tagInfo.TagId;

                    _context.Update(tagmodel);
                    await _context.SaveChangesAsync();

                    return Ok(JsonConvert.SerializeObject(tagmodel));
                }
                else
                {
                    tagmodel.Nome = tagTO.TagName;
                    tagmodel.tipoTag = (int)EnumTipoTag.NaoCadastrada;
                    tagmodel.TagId = tagTO.TagID;
                    tagmodel.EstabelecimentoId=tagTO.IdEstabelecimento;

                    await _context.AddAsync(tagmodel);
                    await _context.SaveChangesAsync();

                    return Ok(JsonConvert.SerializeObject(tagmodel));
                }
             
            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,err.ToString());
            }

            return BadRequest();
        }

        
    }
}
