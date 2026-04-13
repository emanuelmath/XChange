using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using XChange.Web.Models;

namespace XChange.Web.Controllers
{
    [Authorize] //Si todos las rutas son privadas, se le pone a la clase.
    // Se puede liberar alguna que sea pública poniéndole al método el atributo [AllowAnonymous].
    // Y también solo se puede decir qué nivel de protección tiene cada método sin tener que ponerle al controller.
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
