using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accord;
using Dicom;
using Dicom.Imaging;
using Dicom.IO.Reader;
using ImagePreprocessing;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using FileMode = System.IO.FileMode;

namespace WebApp.Controllers
{
    public class OperationsController : Controller
    {
        private static LiteDatabase db;
        public OperationsController()
        {
            db = Database.CreateConnection();
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            if (files.Count == 0) return RedirectToAction("Index", "Home", new {error = true});
            var formFile = files.First();

            if (formFile.Length <= 0 || !formFile.FileName.EndsWith(".dcm") ||
                !IsDcomFileValid(formFile.OpenReadStream()))
            {
                return RedirectToAction("Index", "Home", new {error = true});
            }

            // use a unique id as filename/id
            Guid id;
            var analysis = db.GetCollection<Analysis>("analysis");
            UShortArrayAsImage img = DicomFile.Open(formFile.OpenReadStream()).GetUshortImageInfo();

            Analysis toInsert = new Analysis();
            analysis.Insert(toInsert);
            db.FileStorage.Upload($"images/{toInsert.Id.ToString()}", toInsert.Id.ToString(),
                formFile.OpenReadStream());
            id = toInsert.Id;


            return RedirectToAction("SelectRegion", "Analyze", new {FileName = id});
        }

        private Boolean IsDcomFileValid(Stream dcomStream)
        {
            DicomFile f;
            try
            {
                f = DicomFile.Open(dcomStream);
            }
            catch (DicomFileException)
            {
                return false;
            }
            catch (DicomReaderException)
            {
                return false;
            }

            if (f == null) return false;
            if (!f.Dataset.GetString(DicomTag.PhotometricInterpretation).Contains("MONOCHROME")) return false;
            if (DicomPixelData.Create(f.Dataset).BitDepth.BitsStored != 16) return false;
            return true;
        }

        public IActionResult ShowSavedDcmFileAsPng(String path)
        {
            var file = db.FileStorage.FindById($"images/{path}");
            var ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Seek(0, 0);
            var image = DicomFile.Open(ms).GetUshortImageInfo();
            return new FileStreamResult(image.GetPngAsMemoryStream(), "image/png");
        }

        public IActionResult ShowSavedPng(String path)
        {
            var file = db.FileStorage.FindById($"images/{path}");
            var ms = new MemoryStream();
            file.CopyTo(ms);
            ms.Seek(0, 0);
            return new FileStreamResult(ms, "image/png");
        }
    }
}