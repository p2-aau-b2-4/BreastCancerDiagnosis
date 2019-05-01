using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using BrysterAsp.Models;
using Dicom;
using ImagePreprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace WebApp.Controllers
{
    public class AnalyzeController : Controller
    {
        private IMemoryCache _cache;

        public AnalyzeController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
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
            StreamWriter sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");

            sw.WriteLine("Hello World!!");
            sw.Close();
            ViewBag.ImageId = Request.Form["filePath"];
            
            Point a = new Point(int.Parse(Request.Form["x1"]), int.Parse(Request.Form["y1"]));
            Point b = new Point(int.Parse(Request.Form["x2"]), int.Parse(Request.Form["y2"]));
            Rectangle rectangle = new Rectangle(a, new Size(b.X - a.X, b.Y - a.Y));

            //before returning, lets start the async task of analyzing.
#pragma warning disable 4014
            PerformAnalysis(Request.Form["filePath"], rectangle);
            _cache.
#pragma warning restore 4014

            return View();
            return Redirect($"~/analyze/showResult/{Request.Form["filePath"]}");
            //return Ok(new{x1 = Rectangle.Item1,y1=Rectangle.Item2,x2=Rectangle.Item3,y2=Rectangle.Item4, file=Request.Form["filePath"]});
        }

        private async Task PerformAnalysis(String imageLoc, Rectangle rectangle)
        {
            StreamWriter sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");
            await sw.WriteLineAsync("begin");
            sw.Close();
            string path = Path.GetTempPath() + imageLoc;
            var taskOriginal = new Task<UShortArrayAsImage>(() =>
                DicomFile.Open(path).GetUshortImageInfo());
            taskOriginal.Start();
            UShortArrayAsImage original = await taskOriginal;

            // lets crop the image
            

            var taskCrop = new Task<UShortArrayAsImage>(() =>
                Normalization.GetNormalizedImage(original, rectangle,
                    int.Parse(ConfigurationManager.AppSettings["SizeImageToAnalyze"])));
            taskCrop.Start();
            UShortArrayAsImage croppedImage = await taskCrop;
            Task saveCropped = new Task(() => croppedImage.SaveAsPng(path + "-cropped"));

            sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");
            await sw.WriteLineAsync("cropped");
            sw.Close();
            //sw.WriteLine("CroppedImage");


            // lets apply contrast
            var taskContrast = new Task<UShortArrayAsImage>(() => Contrast.ApplyHistogramEqualization(croppedImage));
            taskContrast.Start();
            UShortArrayAsImage croppedContrastImage =
                await taskContrast;
            Task saveCroppedContrast = new Task(() => croppedContrastImage.SaveAsPng(path + "-croppedContrast"));
            saveCroppedContrast.Start();

            sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");
            await sw.WriteLineAsync("contrast");
            sw.Close();
            //sw.WriteLine("Contrast Done");

            // and lets perform PCA
            Thread.Sleep(3000);
            //sw.WriteLine("Pca Done");

            sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");
            await sw.WriteLineAsync("pca");
            sw.Close();

            // and svm
            Thread.Sleep(3000);
            //sw.WriteLine("Svm Done");
            sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");
            await sw.WriteLineAsync("svm");
            sw.Close();


            await saveCroppedContrast;
            await saveCropped;

            sw = new StreamWriter(Path.GetTempPath() + Request.Form["filePath"] + "-pending.txt");
            await sw.WriteLineAsync("done");
            sw.Close();
        }

        public IActionResult GetAnalysisStatus()
        {
            StreamReader sr = new StreamReader(Path.GetTempPath() + Request.Form["imageId"] + "-pending.txt");
            var str = sr.ReadToEnd();
            sr.Close();
            return Ok(str);
        }


        public IActionResult ShowAnalysis()
        {
            string filePath = Request.Form["filePath"];
            string croppedImgSrc = filePath + "-cropped";
            string contrastImgSrc = filePath + "-contrast";

            ViewBag.CroppedImgSrc = croppedImgSrc;
            ViewBag.ContrastImgSrc = contrastImgSrc;

            ViewBag.Classification = DdsmImage.Pathologies.Benign;
            ViewBag.Probability = (new Random().Next() % 1000) / 10.0;

            return View();
        }
    }
}