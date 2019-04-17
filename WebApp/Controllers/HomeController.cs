using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BrysterAsp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace BrysterAsp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(bool error)
        {
            ViewBag.error = error;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult About()
        {
            return View();
            throw new NotImplementedException();
        }

        public IActionResult Statistics()
        {
            return View();
            throw new NotImplementedException();
        }

        public IActionResult Contact()
        {
            return View();
            throw new NotImplementedException();
        }
    }
}