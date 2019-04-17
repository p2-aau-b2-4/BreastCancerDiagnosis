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
            path = Path.GetTempPath() + path;
            return new FileStreamResult(DicomFile.Open(path).GetUshortImageInfo().GetPngAsMemoryStream(), "image/png");
        }

        public IActionResult StartAnalyzing()
        {
            //not used atm

            return Redirect($"~/analyze/showResult/{Request.Form["filePath"]}");
            //return Ok(new{x1 = Rectangle.Item1,y1=Rectangle.Item2,x2=Rectangle.Item3,y2=Rectangle.Item4, file=Request.Form["filePath"]});
        }

        public IActionResult ShowAnalysis(String path)
        {
            (int, int, int, int) Rectangle = (int.Parse(Request.Form["x1"]),int.Parse(Request.Form["y1"]),int.Parse(Request.Form["x2"]),int.Parse(Request.Form["y2"]));
            path = Path.GetTempPath() + Request.Form["filePath"];
            UshortArrayAsImage image = DicomFile.Open(path).GetUshortImageInfo().Crop(Rectangle);
            
            // lets first apply the crop
            
            // then apply the algorithm
            
            //values that should be present:
            // 1. Image PCA analyzed
            // 2. Classification and probability

            ViewBag.Classification = DdsmImage.Pathologies.Benign;
            ViewBag.Probability = 99.5;
            ViewBag.PcaInputPath = $"https://3c1703fe8d.site.internapcdn.net/newman/csz/news/800/2018/5bc47b1d196b0.jpg";
            
            return View();
        }
    }
}