using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.Xml;
using BrysterAsp.Models;
using Dicom;
using ImagePreprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AnalyzeController : Controller
    {

        public IActionResult SelectRegion(String FileName)
        {
            ViewBag.FileName = FileName;
            return View();
        }

        public IActionResult GetPngFromTempPath(String path)
        {
            path = "/tmp/" + path + ".tmp";
            return new FileStreamResult(DicomFile.Open(path).GetUshortImageInfo().GetPngAsMemoryStream(), "image/png");
        }

        public IActionResult StartAnalyzing()
        {
            (int, int, int, int) Rectangle = (int.Parse(Request.Form["x1"]),int.Parse(Request.Form["y1"]),int.Parse(Request.Form["x2"]),int.Parse(Request.Form["y2"]));

            return Redirect($"~/analyze/showResult/{Request.Form["filePath"]}");
            return Ok(new{x1 = Rectangle.Item1,y1=Rectangle.Item2,x2=Rectangle.Item3,y2=Rectangle.Item4, file=Request.Form["filePath"]});
        }

        public IActionResult ShowAnalysis(String path)
        {
            return View();
        }
    }
}