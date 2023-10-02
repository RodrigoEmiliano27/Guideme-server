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

        [HttpGet("ExibirTagsEstabelecimento")]
        public IActionResult ExibirTagsEstabelecimento()
        {
            Debug.WriteLine("Listando Tags");
            var user = _context.UsuariosEstabelecimento.FirstOrDefault(o => o.Id == HttpContext.Session.GetInt32("UserId"));
            var tags = _context.Tags.Where(o => o.EstabelecimentoId == user.Id_Estabelecimento).ToList();


            return View("TagsEstabelecimento", tags);
        }

        /*[HttpPost]
        public IActionResult Editar(int id, string novoNome)
        {
            var tag = _context.Tags.FirstOrDefault(o => o.Id == id);
            tag.Nome = novoNome;
            _context.Tags.Update(tag);
            _context.SaveChanges();

            return ExibirTagsEstabelecimento();
            // Lógica para atualizar a tag com o novo nome
            // Use o id para identificar a tag a ser atualizada
            // Atualize o registro no banco de dados ou em outra fonte de dados

            // Retorne um JSON ou uma resposta apropriada para indicar o sucesso ou falha da atualização
        }*/

        [HttpPost]
        public IActionResult Editar(String tag)
        {
            Debug.WriteLine("tag => " + tag);
            /*var tag = _context.Tags.FirstOrDefault(o => o.Id == id);
            tag.Nome = novoNome;
            _context.Tags.Update(tag);
            _context.SaveChanges();*/
            
            return ExibirTagsEstabelecimento();

        }

        [HttpPost("DeleteTag")]
        public ActionResult<object> DeleteTag([FromBody] DeleteTagModel model)
        {
            Debug.WriteLine("Chamou o DeleteTag");
            Debug.WriteLine("Id => " + model.Id);
            var tag = _context.Tags.AsNoTracking().FirstOrDefault(o => o.Id == model.Id);
            if(tag != null)
            {
                _context.Tags.Remove(tag);
                _context.SaveChanges();
                return Ok("tag deletada");
            }
            else{
                return NotFound("tag não encontrada");
            }
        }
    }
    public class DeleteTagModel
    {
        public int Id { get; set; }
    }

}
