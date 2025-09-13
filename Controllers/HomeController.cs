using CarAPI.Data;
using CarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace CarAPI.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    namespace CarAPI.Controllers
    { 
        public class HomeController : Controller
        {
            public IActionResult Index()
            {
                return View();

            }
            [HttpPost]
            public IActionResult SetTheme(string theme, string returnUrl)
            {
                HttpContext.Session.SetString("Theme", theme);

                // Validate returnUrl to prevent open redirect attacks
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Fallback to home if returnUrl is invalid
                return RedirectToAction("Index", "Home");
            }

        }
    }

}
