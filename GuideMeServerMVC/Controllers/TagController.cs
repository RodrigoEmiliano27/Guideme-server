using Microsoft.AspNetCore.Mvc;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;
using GuideMeServerMVC.Data;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        public TagController(GuidemeDbContext context)
        { 
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

        [HttpPost("tag"), Authorize]
        public TagViewModel PostTag(TagViewModel model)
        {
            //DAO.Insert(model)
            //Tag tag = new Tag();
            //tag.Id = model.Id;
            //tag.Informacao = model.Informacao;
            // service.Insert(usuario);
            //await AzureStorageHelper.CreateContainerAsync($"plant-{model.id.ToString()}");
            return model;

        }
    }
}
