using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using XChange.Web.Models;

namespace XChange.Web.Controllers
{
    //[Authorize] Lo dejo comentado ya que estas son rutas públicas. 
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

        public IActionResult Landing()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact() 
        {
            return View();
        }

        public IActionResult Terms()
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
