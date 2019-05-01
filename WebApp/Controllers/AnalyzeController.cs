using System;
using System.Collections.Generic;
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
        private static string loadedImageStatusStr = "Loaded Image";
        private static string croppedImageStatusStr = "Cropped Image";
        private static string contrastImageStatusStr = "Applied contrast";
        private static string pcaImageStatusStr = "Ran PCA";
        private static string svmImageStatusStr = "Ran SVM";
        private static string doneStatusStr = "done";

        public AnalyzeController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public IActionResult SelectRegion(String fileName)
        {
            ViewBag.FileName = fileName;
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
            ViewBag.ImageId = Request.Form["filePath"];

            Point a = new Point(int.Parse(Request.Form["x1"]), int.Parse(Request.Form["y1"]));
            Point b = new Point(int.Parse(Request.Form["x2"]), int.Parse(Request.Form["y2"]));
            Rectangle rectangle = new Rectangle(a, new Size(b.X - a.X, b.Y - a.Y));

            //before returning, lets start the async task of analyzing.
#pragma warning disable 4014
            PerformAnalysis(Request.Form["filePath"], rectangle);
#pragma warning restore 4014

            return View();
            return Redirect($"~/analyze/showResult/{Request.Form["filePath"]}");
            //return Ok(new{x1 = Rectangle.Item1,y1=Rectangle.Item2,x2=Rectangle.Item3,y2=Rectangle.Item4, file=Request.Form["filePath"]});
        }

        private async Task PerformAnalysis(String imageLoc, Rectangle rectangle)
        {
            _cache.Set("_imageLoc-status", "Starting");

            UShortArrayAsImage image = null;
            int tasksComplete = 0;
            string path = Path.GetTempPath() + imageLoc;
            List<Task> tasks = new List<Task>()
            {
                new Task(() =>
                {
                    image = DicomFile.Open(path).GetUshortImageInfo(); 
                    _cache.Set($"_{imageLoc}-status", loadedImageStatusStr);
                }),
                new Task(() =>
                {
                    image = Normalization.GetNormalizedImage(image, rectangle,
                        int.Parse(ConfigurationManager.AppSettings["SizeImageToAnalyze"]));
                    image.SaveAsPng(path + "-cropped");
                    _cache.Set($"_{imageLoc}-status", croppedImageStatusStr);
                }),
                new Task(() =>
                {
                    image = Contrast.ApplyHistogramEqualization(image);
                    image.SaveAsPng(path + "-croppedContrast");
                    _cache.Set($"_{imageLoc}-status", contrastImageStatusStr);
                }),
                new Task(() =>
                {
                    //PCA
                    Thread.Sleep(3000);
                    _cache.Set($"_{imageLoc}-status", pcaImageStatusStr);
                }),
                new Task(() =>
                {
                    //SVM
                    Thread.Sleep(3000);
                    _cache.Set($"_{imageLoc}-status", svmImageStatusStr);
                })
            };
            foreach (Task task in tasks)
            {
                task.Start();
                await task;
                
                // lets set percentage done:
                _cache.Set($"_{imageLoc}-percentage", (++tasksComplete*100)/tasks.Count);
            }
            _cache.Set($"_{imageLoc}-status", doneStatusStr);
        }

        public IActionResult GetAnalysisStatus()
        {
            var imageLoc = Request.Form["imageId"];
            string status = _cache.Get($"_{imageLoc}-status").ToString();
            string percentage = _cache.Get($"_{imageLoc}-percentage").ToString();
            if (status.Equals(doneStatusStr));
            {
                _cache.Remove($"_{imageLoc}-status");
                _cache.Remove($"_{imageLoc}-percentage");
            }

            return Ok(status+","+percentage);
        }


        public IActionResult ShowAnalysis()
        {
            string filePath = Request.Form["filePath"];
            string croppedImgSrc = filePath + "-cropped";
            string contrastImgSrc = filePath + "-croppedContrast";

            ViewBag.CroppedImgSrc = croppedImgSrc;
            ViewBag.ContrastImgSrc = contrastImgSrc;

            ViewBag.Classification = DdsmImage.Pathologies.Benign;
            ViewBag.Probability = (new Random().Next() % 1000) / 10.0;

            return View();
        }
    }
}