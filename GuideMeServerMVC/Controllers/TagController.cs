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
using System.Runtime.Intrinsics.Arm;
using Azure;
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
                    tagmodel.EstabelecimentoId = tagTO.IdEstabelecimento;

                    await _context.AddAsync(tagmodel);
                    await _context.SaveChangesAsync();

                    return Ok(JsonConvert.SerializeObject(tagmodel));
                }

            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, err.ToString());
            }

            return BadRequest();
        }

        [HttpGet("TelaEditar")]
        //[AllowAnonymous]
        //public IActionResult TelaEditar([FromForm] int Id)
        public virtual async Task<IActionResult> TelaEditar(int Id)
        {
            Debug.WriteLine("TelaEditar");
            var tag = _context.Tags.FirstOrDefault(o => o.Id == Id);
            if(tag != null){
                return View("EditarTag", tag);
            } else {
                return NotFound();
            }
        }

        [HttpGet("ExibirTagsEstabelecimento")]
        public IActionResult ExibirTagsEstabelecimento()
        {
            Debug.WriteLine("Listando Tags");
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();


            return View("TagsEstabelecimento", tags);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateTag([FromForm] TagViewModel model)
        {
            _context.Update(model);
            _context.SaveChanges();

            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();


            return View("TagsEstabelecimento", tags);
        }


        //public async Task<IActionResult> Delete([FromBody] DeleteTagModel model)
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            Debug.WriteLine("Chamou o DeleteTag");
            Debug.WriteLine("Id => " + id);
            var tag = _context.Tags.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if(tag != null)
            {
                tag.TagsPai = null;
                tag.EstabelecimentoId = 0;
                tag.tipoTag = (int)EnumTipoTag.NaoCadastrada;
                _context.Update(tag);
                await _context.SaveChangesAsync();

                _context.Tags.Remove(tag);

                await transaction.CommitAsync();
                //_context.SaveChanges();
                return RedirectToAction("ExibirTagsEstabelecimento", "Tag");;
            }
            else{
                return NotFound("tag não encontrada");
            }
        }

        /*public async Task<IActionResult> Delete(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var tag =await _context.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (tag != null)
                {
                        await _context.Lugares.Where(x => x.Id == id).ExecuteDeleteAsync();

                        await transaction.CommitAsync();

                    }
                }
                              
                return RedirectToAction("Index", "Lugares");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }*/
    }
    public class DeleteTagModel
    {
        public int Id { get; set; }
    }

    public class EditTagModel
    {
        public int Id { get; set; }
        public String TagId { get; set; }
        public String Nome { get; set; }
        public int tipoTag { get; set; }
    }

}
