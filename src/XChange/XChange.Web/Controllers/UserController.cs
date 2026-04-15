using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XChange.Core.Entities;
using XChange.Core.Interfaces;
using XChange.Web.ViewModels;

namespace XChange.Web.Controllers
{
    [Authorize]
    public class UserController(IUser2faRepository user2faRepo, IMfaService mfaService) : Controller
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
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Auth");

            var setupData = mfaService.GenerateSetupInfo(email);

            ViewBag.BarcodeImageUrl = setupData.QrCodeSetupImageUrl;
            // ViewBag.ManualCode = setupData.ManualEntryKey; 

            HttpContext.Session.Set("TempMfaSecret", setupData.SecretKey);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ConfirmMfaActivation(VerifyMfaViewModel request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { cod = 99, msg = "No se pudo identificar tu sesión." });
            }

            byte[]? tempSecret = HttpContext.Session.Get("TempMfaSecret");
            if (tempSecret == null || tempSecret.Length == 0)
            {
                return Json(new { cod = 99, msg = "La sesión expiró. Cierra la ventana y vuelve a intentarlo." });
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Json(new { cod = 0, msg = "El código es requerido." });
            }

            bool isValid = mfaService.ValidatePin(tempSecret, request.Code);

            if (!isValid)
            {
                return Json(new { cod = 0, msg = "El código ingresado es incorrecto." });
            }

            try
            {
                var user2fa = new User2fa(userId, tempSecret);
                await user2faRepo.CreateAsync(user2fa);

                HttpContext.Session.Remove("TempMfaSecret");

                return Json(new { cod = 1, msg = "MFA activado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { cod = 99, msg = "Error al guardar en la base de datos." });
            }
        }

    }
}
