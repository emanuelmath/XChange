using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XChange.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Wallet()
        {
            return View();
        }

        public IActionResult Transactions()
        {
            return View();
        }

    }
}
