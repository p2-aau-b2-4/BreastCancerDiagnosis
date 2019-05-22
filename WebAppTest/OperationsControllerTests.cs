using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using WebApp.Controllers;

namespace Tests
{
    public class Tests
    {
        private OperationsController _oc;

        [SetUp]
        public void Setup()
        {
            _oc = new OperationsController();
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
            Assert.AreEqual("SelectRegion", result.ActionName);
            Assert.AreEqual("Analyze", result.ControllerName);
            
        }

        [Test]
        public async Task TestWrongFileNameUpload()
        {
            // Arrange
            var file = new List<IFormFile>();

            var ms = new MemoryStream();
            using (var fileStream = File.OpenRead("../../../testDcmFile.dcm"))
            {
                fileStream.CopyTo(ms);
            }

            file.Add(new FormFile(ms, 0, ms.Length, "files", "welp.txt"));

            var result = await _oc.UploadFile(file) as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        [Test]
        public async Task TestEmptyFileUpload()
        {
            // Arrange
            var file = new List<IFormFile>();

            var ms = new MemoryStream();
            using (var fileStream = File.OpenRead("../../../testDcmFile.dcm"))
            {
                //fileStream.CopyTo(ms);
            }

            file.Add(new FormFile(ms, 0, ms.Length, "files", "welp.dcm"));

            var result = await _oc.UploadFile(file) as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }
        
        [Test]
        public async Task TestNoFileUpload()
        {
            // Arrange
            var file = new List<IFormFile>();

            var result = await _oc.UploadFile(file) as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }
        [Test]
        public async Task TestCorruptFile()
        {
            // Arrange
            var file = new List<IFormFile>();
            var ms = new MemoryStream(new byte[] {1,2,3,4,12,3,4});

            file.Add(new FormFile(ms, 0, ms.Length, "files", "welp.dcm"));
            
            var result = await _oc.UploadFile(file) as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }
        [Test]
        public async Task Test8bitFileUpload()
        {
            // Arrange
            var file = new List<IFormFile>();

            var ms = new MemoryStream();
            using (var fileStream = File.OpenRead("../../../8bit.dcm"))
            {
                fileStream.CopyTo(ms);
            }

            file.Add(new FormFile(ms, 0, ms.Length, "files", "welp.dcm"));

            var result = await _oc.UploadFile(file) as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        [Test]
        public void Test1()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var controller = new AnalyzeController();

            controller.SelectRegion("");
            Assert.Pass();
        }
    }
}