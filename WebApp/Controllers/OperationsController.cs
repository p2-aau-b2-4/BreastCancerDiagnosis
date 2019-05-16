using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dicom;
using Dicom.Imaging;
using Dicom.IO.Reader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class OperationsController : Controller
    {
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            if(files.Count == 0) return RedirectToAction("Index","Home", new {error=true});
            var formFile = files.First();
            
            if (formFile.Length <= 0 || !formFile.FileName.EndsWith(".dcm") || !IsDcomFileValid(formFile.OpenReadStream()))
            {
                return RedirectToAction("Index","Home", new {error=true});
            }

            // use a unique id as filename/id
            String filePath = Guid.NewGuid().ToString();
            using (var stream = new FileStream(Path.GetTempPath() + filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return RedirectToAction("SelectRegion","Analyze", new { FileName = filePath });
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
    }
}