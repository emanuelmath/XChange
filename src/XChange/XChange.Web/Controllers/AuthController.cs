using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XChange.Core.Constants;
using XChange.Core.Entities;
using XChange.Core.Interfaces;
using XChange.Web.Filters;
using XChange.Web.ViewModels;

namespace XChange.Web.Controllers
{
    
    public class AuthController(
            IUserRepository userRepo,
            IUser2faRepository faRepo,
            IMfaService mfaService,
            IPasswordHasher passwordHasher) : Controller
    {
        [AllowAnonymous]
        [RedirectIfAuthenticated]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [RedirectIfAuthenticated]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [RedirectIfAuthenticated]
        [RedirectIfNotLoginForMfa]
        public IActionResult VerifyMfa()
        {
            return View();
        }

        [AllowAnonymous]
        [RedirectIfAuthenticated]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [RedirectIfAuthenticated]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [RedirectIfAuthenticated]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RegisterUser(RegisterViewModel request)
        {
            if (!ModelState.IsValid)
                return Json(new { cod = 0, msg = "Datos inválidos." });

            if (request.Password != request.ConfirmPassword)
                return Json(new { cod = 0, msg = "Las contraseñas no coinciden." });

            try
            {
                var hash = passwordHasher.Hash(request.Password);

                var newUser = new User(
                    email: request.Email,
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    authProvider: AuthProvider.Local,
                    password: hash
                );

                var existingUser = await userRepo.GetByEmailAsync(request.Email);

                if (existingUser != null)
                {
                    return Json(new { cod = 0, msg = "El correo ya está registrado." });
                }
    
                var userId = await userRepo.CreateAsync(newUser);

                return Json(new { cod = 1, msg = "Usuario registrado correctamente.", id = userId });
            }
            catch (Exception ex)
            {
                return Json(new { cod = 99, msg = "Error crítico en el servidor.", detail = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LoginUser(LoginViewModel request)
        {
            if (!ModelState.IsValid)
                return Json(new { cod = 0, msg = "Datos inválidos." });

            var user = await userRepo.GetByEmailAsync(request.Email);

            if (user == null || user.Status != UserStatus.Active)
                return Json(new { cod = 0, msg = "Usuario no existente." });

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return Json(new { cod = 0, msg = "Esta cuenta está vinculada a Google. Por favor, usa el botón de 'Continuar con Google'." });
            }

            try
            {
                if (!passwordHasher.Verify(request.Password, user.PasswordHash))
                {
                    return Json(new { cod = 0, msg = "Contraseña incorrecta." });
                }
            }
            catch (Exception)
            {
                return Json(new { cod = 99, msg = "La cuenta tiene una contraseña obsoleta. Contacte a soporte." });
            }

            bool? has2fa = await faRepo.Is2faEnabledAsync(user.Id);

            if (has2fa != null && has2fa == true)
            {

                HttpContext.Session.SetInt32("PendingUserId", user.Id);
                HttpContext.Session.SetString("PendingUserEmail", user.Email);
                HttpContext.Session.SetString("PendingUserName", user.FirstName);

                return Json(new { cod = 2, msg = "Credenciales válidas, requiere MFA." });
            }
            else
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = request.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Json(new { cod = 1, msg = "Inicio de Sesión exitoso, redirigiendo a Home." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginOrRegisterWithGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback", "Auth") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "Usuario";
            var lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? "Apellido";

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var existingUser = await userRepo.GetByEmailAsync(email!);
            int finalUserId = 0;

            if (existingUser != null)
            {
                await userRepo.LinkGoogleIdToUser(googleId!, existingUser.Id);
                finalUserId = existingUser.Id; 
            }
            else
            {
                var newUser = new User(
                    email: email!,
                    firstName: firstName,
                    lastName: lastName,
                    authProvider: AuthProvider.Google,
                    googleId: googleId!);

                finalUserId = await userRepo.CreateAsync(newUser);
            }

            if (finalUserId <= 0)
                return RedirectToAction("Login", "Auth");

            bool? has2fa = await faRepo.Is2faEnabledAsync(finalUserId);

            if (has2fa != null && has2fa == true)
            {
                HttpContext.Session.SetInt32("PendingUserId", finalUserId);
                HttpContext.Session.SetString("PendingUserEmail", email!);
                HttpContext.Session.SetString("PendingUserName", firstName);

                return RedirectToAction("VerifyMfa");
            }
            else
            {
                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, finalUserId.ToString()),
                    new Claim(ClaimTypes.Email, email!),
                    new Claim(ClaimTypes.Name, firstName)
                };

                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Dashboard", "User");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> VerifyMfaUser(VerifyMfaViewModel verifyMfaViewModel)
        {
            if (!ModelState.IsValid)
                return Json(new { cod = 0, msg = "Código inválido." });


            int? idUser = HttpContext.Session.GetInt32("PendingUserId");
            string? email = HttpContext.Session.GetString("PendingUserEmail");
            string? firstName = HttpContext.Session.GetString("PendingUserName");

            if (idUser != null && email != null && firstName != null)
            {
                var idString = idUser.ToString();
                var key = await faRepo.GetSecretKeyAsync((int)idUser);
                if (key != null)
                {
                    var resp = mfaService.ValidatePin(key, verifyMfaViewModel.Code);
                    if(resp)
                    {
                        var userClaims = new List<Claim>
                         {
                            new Claim(ClaimTypes.NameIdentifier, idString),
                            new Claim(ClaimTypes.Email, email!),
                            new Claim(ClaimTypes.Name, firstName)
                         };

                        var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        HttpContext.Session.Remove("PendingUserId");
                        HttpContext.Session.Remove("PendingUserEmail");
                        HttpContext.Session.Remove("PendingUserName");

                        return Json(new { cod = 1, msg = "Verificación completada." });
                    }
                    else
                    {
                        return Json(new { cod = 0, msg = "Código incorrecto." });
                    }
                }
                else
                {
                    HttpContext.Session.Remove("PendingUserId");
                    HttpContext.Session.Remove("PendingUserEmail");
                    HttpContext.Session.Remove("PendingUserName");
                    return Json(new { cod = 99, msg = "No tienes doble verificación u error al obtenerla." });
                }
            }
            else
            {
                HttpContext.Session.Remove("PendingUserId");
                HttpContext.Session.Remove("PendingUserEmail");
                HttpContext.Session.Remove("PendingUserName");
                return Json(new { cod = 99, msg = "Sesión inválida." });
            }
        }
    }

}
