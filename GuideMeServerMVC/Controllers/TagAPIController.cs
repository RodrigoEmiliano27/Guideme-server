using GuideMeServerMVC.Data;
using GuideMeServerMVC.Enum;
using GuideMeServerMVC.Models;
using GuideMeServerMVC.Services;
using GuideMeServerMVC.TO;
using GuideMeServerMVC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/Tag")]
    [ApiController]
    public class TagAPIController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly GuidemeDbContext _context;

        public TagAPIController(IConfiguration configuration, GuidemeDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [Authorize(Roles = "app")]
        [HttpGet("v1/GetTagData")]
        public async Task<IActionResult> GetData(string TagID)
        {
            try
            {
                var lugarService = new LugarService(_context);
                var itenService = new ItensService(_context);
                var tagService = new TagService(_context);
                string nome = "";

                var tagInfo = _context.Tags.FirstOrDefault(x => x.TagId.Equals(TagID));

                var estab = _context.Tags.AsNoTracking().FirstOrDefault(x => x.TagId.Equals(TagID));
                if (estab != null)
                    nome = estab.Nome;

                if (tagInfo == null)
                    return NotFound();

                List<TagViewModel> listaTags =
                    _context.Tags.Where(x => x.EstabelecimentoId == tagInfo.EstabelecimentoId).ToList();

                List<TagViewModel> listaTagsNavegaveis = new List<TagViewModel>();

                List<LugaresViewModel> lista = new List<LugaresViewModel>();
                List<ItensViewModel> itens = new List<ItensViewModel>();

                foreach (TagViewModel tag in listaTags)
                {
                    bool navegavel = false;
                    tag.TagsPai = _context.TagsPai.Where(x => x.Id_Tag == tag.Id).ToList();
                    var tagsAux = _context.TagsPai.Where(x => x.Id_Tag_Pai == tag.Id).ToList();
                    foreach (var tagAux in tagsAux)
                    {
                        if(tag.TagsPai!=null)
                            tag.TagsPai.Add(tagAux);
                        else
                            tag.TagsPai = tagsAux;

                    }

                    if (tag.TagsPai != null && tag.TagsPai.Count > 0)
                    {
                        listaTagsNavegaveis.Add(tag);
                        navegavel = true;
                       
                    }
                    if (tag.tipoTag == (int)EnumTipoTag.lugar)
                    {
                        var lugarTag = await lugarService.GetLugarByTag(tag);
                        if (lugarTag != null)
                        {
                            lugarTag.Navegavel = navegavel;
                            lista.Add(lugarTag);
                        }
                            
                    }
                    else if (tag.tipoTag == (int)EnumTipoTag.itens)
                    {
                        var itenTag = await itenService.GetItemByTag(tag);
                        if (itenTag != null)
                        {
                            itenTag.Navegavel = navegavel;
                            itens.Add(itenTag);
                        }
                            
                    }
                }

                EstabelecimentoTagsTO data = new EstabelecimentoTagsTO();
                data.Lugares = lista;
                data.Itens = itens;
                data.Tags = listaTagsNavegaveis;
                data.NomeEstabelecimento = nome;


                return Ok(data);
            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, err.ToString());
            }
        }
        [Authorize(Roles = "estabelecimento")]
        [HttpPost("v1/SalvarTag")]
        public async Task<IActionResult> SalvaTag([FromBody] TagTO tagTO)
        {
            TagViewModel tagInfo = null;
            try
            {
                TagViewModel tagmodel = new TagViewModel();
                tagInfo = _context.Tags.AsNoTracking().FirstOrDefault(x => x.TagId.Equals(tagTO.TagID));

                if (tagInfo != null)
                {
                    if (tagInfo.EstabelecimentoId != tagTO.IdEstabelecimento)
                        return StatusCode(StatusCodes.Status401Unauthorized);

                    tagmodel.Id = tagInfo.Id;
                    tagmodel.Nome = tagTO.TagName;
                    tagmodel.tipoTag = tagInfo.tipoTag;
                    tagmodel.TagId = tagInfo.TagId;
                    tagmodel.EstabelecimentoId = tagTO.IdEstabelecimento;

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
                _ = HelperControllers.LoggerErro(HttpContext.Session, _context, this.GetType().Name, MethodBase.GetCurrentMethod().Name, err,
                    EstabelecimentoID: tagInfo!=null ? tagInfo.EstabelecimentoId:null, naoProcureEstabelecimento: true);
                return StatusCode(StatusCodes.Status500InternalServerError, err.ToString());
            }

            return BadRequest();
        }
    }
}
