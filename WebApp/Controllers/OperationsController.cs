using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BrysterAsp.Models;
using Dicom;
using ImagePreprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace BrysterAsp.Controllers
{
    public class OperationsController : Controller
    {
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            String filePath = Guid.NewGuid().ToString();
            
            // full path to file in temp location
            //var filePath = Path.GetTempFileName();

            var formFile = files.First();
            if (formFile.Length > 0)
            {
                using (var stream = new FileStream(Path.GetTempPath()+filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
            

            return Redirect("analyze/selectregion/" +filePath);


            //return Ok(new {count = files.Count, size, filePath, name = files.First().FileName});
        }
    }
}