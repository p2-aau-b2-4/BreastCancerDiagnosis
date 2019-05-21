using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord.IO;
using Dicom;
using ImagePreprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using DimensionReduction;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using LiteDB;
using WebApp.Models;
using Configuration = ImagePreprocessing.Configuration;
using Serializer = Accord.IO.Serializer;

namespace WebApp.Controllers
{
    public class AnalyzeController : Controller
    {
        private static string loadedImageStatusStr = "Indlæste billedet";
        private static string croppedImageStatusStr = "Normaliserede billedet";
        private static string contrastImageStatusStr = "Forbedrede konstrasten";
        private static string pcaImageStatusStr = "Inddelte i komponenter";
        private static string svmImageStatusStr = "Kørte maskinlæring";
        private static string doneStatusStr = "Færdig";
        private static string startingImageStatusStr = "Starter..";
        private LiteDatabase db;

        public AnalyzeController()
        {
            db = Database.CreateConnection();
        }

        public IActionResult SelectRegion(String fileName)
        {
            ViewBag.Analysis = db.GetCollection<Analysis>("analysis").FindById(Guid.Parse(fileName));
            return View();
        }

        public IActionResult StartAnalyzing()
        {
            ViewBag.Analysis = db.GetCollection<Analysis>("analysis").FindById(Guid.Parse(Request.Form["filePath"]));

            Point a = new Point(int.Parse(Request.Form["x1"]), int.Parse(Request.Form["y1"]));
            Point b = new Point(int.Parse(Request.Form["x2"]), int.Parse(Request.Form["y2"]));
            // todo: fix if b is upper left corner
            Rectangle rectangle = new Rectangle(new Point(Math.Min(a.X,b.X),Math.Min(a.Y,b.Y)), 
                new Size(Math.Max(a.X,b.X)-Math.Min(a.X,b.X), Math.Max(a.Y,b.Y)-Math.Min(a.Y,b.Y)));

            //before returning, lets start the async task of analyzing.
#pragma warning disable 4014 // disable warning about starting async task without awaiting. This is not needed here.
            PerformAnalysis(Request.Form["filePath"], rectangle);
#pragma warning restore 4014

            return View();
        }

        private void UpdateStatus(string path, string status)
        {
            var analysis = db.GetCollection<Analysis>("analysis");
            Analysis currentAnalysis = analysis.FindOne(x => x.Id.ToString().Equals(path));
            currentAnalysis.Status = status;
            analysis.Update(currentAnalysis);
        }

        private async Task PerformAnalysis(String path, Rectangle rectangle)
        {
            UShortArrayAsImage image = null;
            double[] pcaComponents = null;
            int tasksComplete = 0;
            UpdateStatus(path, startingImageStatusStr);
            List<Task> tasks = new List<Task>()
            {
                new Task(() =>
                {
                    var file = db.FileStorage.FindById($"images/{path}");
                    var ms = new MemoryStream();
                    file.CopyTo(ms);
                    ms.Seek(0, 0);
                    image = DicomFile.Open(ms).GetUshortImageInfo();

                    UpdateStatus(path, loadedImageStatusStr);
                }),
                new Task(() =>
                {
                    image = Normalization.GetNormalizedImage(image, rectangle,
                        int.Parse(Configuration.Get("sizeImageToAnalyze")));
                    db.FileStorage.Upload($"images/{path}-cropped", $"{path}-cropped",
                        image.GetPngAsMemoryStream());

                    UpdateStatus(path, croppedImageStatusStr);
                }),
                new Task(() =>
                {
                    image = Contrast.ApplyHistogramEqualization(image);
                    db.FileStorage.Upload($"images/{path}-croppedContrast", $"{path}-croppedContrast",
                        image.GetPngAsMemoryStream());

                    UpdateStatus(path, contrastImageStatusStr);
                }),
                new Task(() =>
                {
                    //PCA
                    Pca pca = Pca.LoadModelFromFile("pca_model.bin");

                    if (!int.TryParse(Configuration.Get("componentsToUse"), out int components))
                    {
                        components = pca.Eigenvalues.Length;
                    }
                    pcaComponents = pca.GetComponentsFromImage(image, components);
                    UpdateStatus(path, pcaImageStatusStr);
                }),
                new Task(() =>
                {
                    //SVM
                    SVMProblem svmProblem = new SVMProblem();

                    // add all the components to an SVMNode[]
                    SVMNode[] nodes = new SVMNode[pcaComponents.Length];
                    for (int i = 0; i < pcaComponents.Length; i++)
                    {
                        nodes[i] = new SVMNode(i + 1, pcaComponents[i]);
                    }

                    svmProblem.Add(nodes, 0);

                    SVMModel svmModel = SVM.LoadModel(Configuration.Get("ModelLocation"));
                    
                    double results = SVM.PredictProbability(svmModel,nodes, out var probabilities);

                    var analysis = db.GetCollection<Analysis>("analysis");
                    Analysis currentAnalysis = analysis.FindOne(x => x.Id.ToString().Equals(path));
                    currentAnalysis.Certainty = probabilities[0]*100;
                    currentAnalysis.Diagnosis = results == 0
                        ? DdsmImage.Pathologies.Benign
                        : DdsmImage.Pathologies
                            .Malignant;
                    analysis.Update(currentAnalysis);


                    UpdateStatus(path, svmImageStatusStr);
                })
            };
            foreach (Task task in tasks)
            {
                task.Start();
                await task;

                // lets set percentage done:
                var analysis = db.GetCollection<Analysis>("analysis");
                Analysis currentAnalysis = analysis.FindOne(x => x.Id.ToString().Equals(path));
                currentAnalysis.PercentageDone = (++tasksComplete * 100) / tasks.Count;
                analysis.Update(currentAnalysis);
            }

            UpdateStatus(path, doneStatusStr);
        }

        public IActionResult GetAnalysisStatus()
        {
            var imageLoc = Request.Form["imageId"];

            var analysis = db.GetCollection<Analysis>("analysis");
            Analysis currentAnalysis = analysis.FindOne(x => x.Id.ToString().Equals(imageLoc));
            return Ok(currentAnalysis.Status + "," + currentAnalysis.PercentageDone);
        }


        public IActionResult ShowAnalysis()
        {
            string filePath = Request.Form["filePath"];
            var analysis = db.GetCollection<Analysis>("analysis");
            ViewBag.Analysis = analysis.FindOne(x => x.Id.ToString().Equals(filePath));
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