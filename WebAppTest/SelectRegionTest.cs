using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using WebApp.Controllers;

namespace Tests
{
    public class SelectRegionTest
    {
        private OperationsController _oc;
        private AnalyzeController _ac;

        [SetUp]
        public void Setup()
        {
            _oc = new OperationsController();
            var cache = new MemoryCache(new MemoryCacheOptions());
            _ac = new AnalyzeController();
        }
        
        [Test]
        public async Task TestValidUpload()
        {
            // Arrange
            var file = new List<IFormFile>();

            var ms = new MemoryStream();
            using (var fileStream = File.OpenRead("../../../testDcmFile.dcm"))
            {
                fileStream.CopyTo(ms);
            }

            file.Add(new FormFile(ms, 0, ms.Length, "files", "welp.dcm"));

            var result = await _oc.UploadFile(file) as RedirectToActionResult;
            result.RouteValues.TryGetValue("FileName",out var filePath);

            var srResult = _ac.SelectRegion(filePath as string) as ViewResult;
            srResult.ViewData.TryGetValue("FileName", out var fileNameTransferred);
            Assert.AreEqual(filePath,fileNameTransferred);
        }
    }
}