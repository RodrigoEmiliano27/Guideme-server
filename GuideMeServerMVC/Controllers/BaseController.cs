using GuideMeServerMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuideMeServerMVC.Controllers
{
    public class BaseController<T>:Controller  where T: PadraoViewModel
    {
    }
}
