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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Png()
        {
            HttpContext context = HttpContext;

            Bitmap image = new Bitmap(1000, 1000);
            image.SetPixel(500,500,Color.Red);
            MemoryStream ms = new MemoryStream();

            image.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            //ms.WriteTo(context.Response.);


            return new FileStreamResult(ms, "image/png");
        }
    }
}