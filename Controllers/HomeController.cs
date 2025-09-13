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
            public IActionResult SetTheme(string theme)
            {
                HttpContext.Session.SetString("Theme", theme);
                return RedirectToAction("Index");
            }

        }
    }

}
