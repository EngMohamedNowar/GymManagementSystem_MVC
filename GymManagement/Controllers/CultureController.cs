using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GymManagement.Controllers
{
    public class CultureController : Controller
    {
        [HttpGet("/Culture/SetCulture")]
        public IActionResult SetCulture(string culture, string? redirectUri)
        {
            if (!string.IsNullOrWhiteSpace(culture))
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true,
                        Path = "/"
                    });
            }

            // Only allow local redirects to prevent open-redirect attacks.
            var destination = "/";
            if (!string.IsNullOrWhiteSpace(redirectUri)
                && Url.IsLocalUrl(redirectUri))
            {
                destination = redirectUri;
            }

            return LocalRedirect(destination);
        }
    }
}
