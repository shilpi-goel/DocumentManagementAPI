using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DocumentManagementAPI.Controllers
{

    // Wanted to make a generic controller and pass PdfDocument type so that It can cater other file type upload also
    [Route("api/[controller]")]
    [ApiController]
    public class PdfDocumentController : ControllerBase
    {
        //File upload location
        string _filePath = Path.GetTempPath() + "Uploads";
        private readonly string[] _extensions = new string[] { ".pdf" };
        private readonly int _maxFileSize = 5; //in mb


        public PdfDocumentController()
        {
            if (!Directory.Exists(_filePath))
                Directory.CreateDirectory(_filePath);
        }

        [HttpGet]
        [Route("GetFiles")]
        public ActionResult<IEnumerable<UploadedFileInfo>> GetFiles()
        {
            DirectoryInfo di = new DirectoryInfo(_filePath);

            var fileInfos = new List<UploadedFileInfo>();
            if (di.Exists)
            {
                FileInfo[] fiArr = di.GetFiles();
                foreach (FileInfo f in fiArr)
                {
                    var fi = new UploadedFileInfo();
                    fi.Name = f.Name;
                    fi.Location = f.Directory.FullName;
                    fi.Size = f.Length;
                    fileInfos.Add(fi);
                }
            }
            return fileInfos;
        }


        [HttpGet]
        [Route("DownloadFile/{filename}")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                          _filePath, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(path), Path.GetFileName(path));
        }




        [HttpPost]
        [Route("PostFiles")]
        public async Task<IActionResult> PostFile([FromBody] IFormFile file)
        {
            //Check file type
            var valid = IsValidFileExtension(file);
            if (valid != ValidationResult.Success)
            {
                return BadRequest(valid.ErrorMessage);
            }

            //Check File size
            valid = IsValidFileSize(file);
            if (valid != ValidationResult.Success)
            {
                return BadRequest(valid.ErrorMessage);
            }





            var filePath = Path.Combine(
                       _filePath, file.FileName);

            try
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Some went wrong !! " + ex);
            }




            return Ok("Success");
        }




        // DELETE api/values/5
        [HttpDelete]
        [Route("DeleteFile/{filename}")]
        public ActionResult Delete(string filename)
        {
            var file = _filePath + "\\" + filename;
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
                return Ok("deleted");
            }
            return BadRequest("File doesn't exists");
        }


        private ValidationResult IsValidFileExtension(IFormFile file)
        {

            var extension = Path.GetExtension(file.FileName);
            if (file != null)
            {
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(extension + " file extension is not allowed!");
                }
            }


            return ValidationResult.Success;
        }


        private ValidationResult IsValidFileSize(IFormFile file)
        {
            if (((file.Length / 1024) / 1024) > _maxFileSize)
            {
                return new ValidationResult($"Maximum allowed file size is { _maxFileSize} bytes.");
            }


            return ValidationResult.Success;
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".pdf", "application/pdf"}
            };
        }
    }



    public class UploadedFileInfo
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public long Size { get; set; }
    }
}



