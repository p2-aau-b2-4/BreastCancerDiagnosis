using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Accord.IO;
using Dicom;
using ImagePreprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using DimensionReduction;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using Configuration = ImagePreprocessing.Configuration;
using Serializer = Accord.IO.Serializer;

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
            ViewBag.Derp = _cache.ToString();
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
            //return Redirect($"~/analyze/showResult/{Request.Form["filePath"]}");
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
                        int.Parse(Configuration.Get("sizeImageToAnalyze")));
                    image.SaveAsPng(path + "-cropped");
                    _cache.Set($"_{imageLoc}-status", croppedImageStatusStr);
                }),
                new Task(() =>
                {
                    image = Contrast.ApplyHistogramEqualization(image);
                    image.Save(path + "-croppedContrastBinary");
                    image.SaveAsPng(path + "-croppedContrast");
                    _cache.Set($"_{imageLoc}-status", contrastImageStatusStr);
                }),
                new Task(() =>
                {
                    //PCA
                    PCA pca = PCA.LoadModelFromFile(Configuration.Get("pca_model.bin"));
                    var imageToPca = Serializer
                        .Load<UShortArrayAsImage>(path + "-croppedContrastBinary");
                    pca.GetComponentsFromImage(imageToPca,int.Parse(Configuration.Get("componentsToUse"))).Save(path + "-pcaComponents");
                    _cache.Set($"_{imageLoc}-status", pcaImageStatusStr);
                }),
                new Task(() =>
                {
                    //SVM
                    SVMProblem svmProblem = new SVMProblem();

                    // add all the components to an SVMNode[]
                    double[] pcaComponents = Serializer.Load<double[]>(path + "-pcaComponents");
                    int componentsToUse = int.Parse(Configuration.Get("componentsToUse"));
                    SVMNode[] nodes = new SVMNode[componentsToUse];
                    for (int i = 0; i < componentsToUse; i++)
                    {
                        nodes[i] = new SVMNode(i + 1, pcaComponents[i]);
                    }

                    svmProblem.Add(nodes, 0);

                    SVMModel svmModel = SVM.LoadModel("svmModel.txt");
                    double[] results = svmProblem.PredictProbability(svmModel, out var probabilities);
                    double[] resultsSavable = {results[0], probabilities[0][0]};
                    resultsSavable.Save(path + "-results");
                    _cache.Set($"_{imageLoc}-status", svmImageStatusStr);
                })
            };
            foreach (Task task in tasks)
            {
                task.Start();
                await task;

                // lets set percentage done:
                _cache.Set($"_{imageLoc}-percentage", (++tasksComplete * 100) / tasks.Count);
            }

            _cache.Set($"_{imageLoc}-status", doneStatusStr);
        }

        public IActionResult GetAnalysisStatus()
        {
            var imageLoc = Request.Form["imageId"];
            string status = _cache.Get($"_{imageLoc}-status").ToString();
            string percentage = _cache.Get($"_{imageLoc}-percentage").ToString();
            if (status.Equals(doneStatusStr))
            {
                _cache.Remove($"_{imageLoc}-status");
                _cache.Remove($"_{imageLoc}-percentage");
            }

            return Ok(status + "," + percentage);
        }


        public IActionResult ShowAnalysis()
        {
            string filePath = Request.Form["filePath"];
            string croppedImgSrc = filePath + "-cropped";
            string contrastImgSrc = filePath + "-croppedContrast";


            ViewBag.CroppedImgSrc = croppedImgSrc;
            ViewBag.ContrastImgSrc = contrastImgSrc;


            ViewBag.PcaComponents = Serializer.Load<double[]>(Path.GetTempPath() + filePath + "-pcaComponents");
            ViewBag.Classification = DdsmImage.Pathologies.Benign;

            double[] results = Serializer.Load<double[]>(Path.GetTempPath() + filePath + "-results");
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            ViewBag.Classification = results[0] == 0 ? "Benign" : "Malignant";
            ViewBag.Probability = results[1]*100;

            return View();
        }

        public IActionResult ShowPngFromTempPath(string path)
        {
            path = Path.GetTempPath() + path;
            Image img = new Bitmap(path);
            MemoryStream ms = new MemoryStream();

            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, 0);
            return new FileStreamResult(ms, "image/png");
        }
    }
}