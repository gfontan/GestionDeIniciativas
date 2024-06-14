using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GestionDeIniciativas.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace GestionDeIniciativas.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Login()
        {

            ClaimsPrincipal claimsUser = HttpContext.User;

            if (claimsUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            if (modelLogin.Email == "olalla.aguirre@aguifontmanager.com" &&
                modelLogin.Password == "123")
            {
                List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                new Claim("OtherProperties","Example Role")
                };

                ClaimsIdentity claimsidentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelLogin.KeepLoggedIn
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsidentity), properties);

                return RedirectToAction("Index", "Home");

            }

            ViewData["ValidateMessage"] = "No se ha encontrado al usuario";

            return View();
        }
    }
}