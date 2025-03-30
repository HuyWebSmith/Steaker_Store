using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;

namespace Steaker_Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = new IdentityUser { UserName = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email), Email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email) };
            var createUserResult = await _signInManager.UserManager.CreateAsync(user);
            if (createUserResult.Succeeded)
            {
                await _signInManager.UserManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
