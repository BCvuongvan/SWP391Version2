using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TingStoreClient.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult ErrorPage()
        {
            return View();
        }

        public IActionResult changeHome()
        {
            return RedirectToAction("Index", "Home");
        }
        
        
    }
}