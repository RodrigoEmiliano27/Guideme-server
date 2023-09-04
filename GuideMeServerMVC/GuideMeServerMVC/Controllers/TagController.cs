using Microsoft.AspNetCore.Mvc;
using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace GuideMeServerMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet(Name = "GetWeatherForecast"), Authorize]
        //https://localhost:7048/api/Tag/tag?id=1
        [HttpGet("tag"), Authorize]
        public Tag GetTag(string id)
        {
            return new Tag
            {
                Id = id,
                Informacao = "Lojas Americanas"
            };
        }

        [HttpPost("tag"), Authorize]
        public Tag PostTag(Tag model)
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
