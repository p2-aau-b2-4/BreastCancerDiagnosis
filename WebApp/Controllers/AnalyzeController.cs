using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            return null;
        }

        public IActionResult GetPngFromTempPath(String path)
        {
            path = "/tmp/" + path + ".tmp";
            return new FileStreamResult(DicomFile.Open(path).GetUshortImageInfo().GetPngAsMemoryStream(), "image/png");
        }
    }
}