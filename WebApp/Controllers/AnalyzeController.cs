using System;
using System.Configuration;
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
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
        public IActionResult GetPngFromSavedTempPath(String path)
        {
            path = Path.GetTempPath() + path;
            return new FileStreamResult(System.IO.File.OpenRead(path), "image/png");
        }

        public IActionResult StartAnalyzing()
        {
            //not used atm

            return Redirect($"~/analyze/showResult/{Request.Form["filePath"]}");
            //return Ok(new{x1 = Rectangle.Item1,y1=Rectangle.Item2,x2=Rectangle.Item3,y2=Rectangle.Item4, file=Request.Form["filePath"]});
        }

        public IActionResult ShowAnalysis()
        {
            Point a = new Point(int.Parse(Request.Form["x1"]),int.Parse(Request.Form["y1"]));
            Point b = new Point(int.Parse(Request.Form["x2"]),int.Parse(Request.Form["y2"]));
            Rectangle rectangle = new Rectangle(a,new Size(b.X-a.X,b.Y-a.Y));
            
            string filePath = Request.Form["filePath"];
            string path = Path.GetTempPath() + filePath;
            UshortArrayAsImage image = DicomFile.Open(path).GetUshortImageInfo().GetNormalizedCrop(rectangle,Convert.ToInt32(ConfigurationManager.AppSettings["CroppedImageSize"]),Convert.ToInt32(ConfigurationManager.AppSettings["CroppedImageTumourSize"]));
            //save the crop:
            string croppedImgSrc = filePath + "-cropped";
            image.SaveAsPng(Path.GetTempPath()+croppedImgSrc);
            
            // lets first apply the contrast:
            image.ApplyHistogramEqualization();
            string contrastImgSrc = filePath + "-contrast";
            image.SaveAsPng(Path.GetTempPath()+contrastImgSrc);

            string edgeImgSrc = filePath + "-edged";
            image.Edge(3000).SaveAsPng(Path.GetTempPath()+edgeImgSrc);
            // then apply the algorithm

            ViewBag.CroppedImgSrc = croppedImgSrc;
            ViewBag.ContrastImgSrc = contrastImgSrc;
            ViewBag.EdgeImgSrc = edgeImgSrc;
            //values that should be present:
            // 1. Image PCA analyzed
            // 2. Classification and probability

            ViewBag.Classification = DdsmImage.Pathologies.Benign;
            ViewBag.Probability = (new Random().Next()%1000)/10.0;
            
            return View();
        }
    }
}