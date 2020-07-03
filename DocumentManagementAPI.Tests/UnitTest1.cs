using DocumentManagementAPI.Controllers;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http.Internal;
using System;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test, Order(1)]
        public void Test_Pdf_Gets_Uploaded_Successfully()
        {
            //Arrange
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;




            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.PostFile(file);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test, Order(2)]

        public void Test_Pdf_Downlaod_By_Filename_Sucess()
        {
            var fileName = "test.pdf";

            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.DownloadFile(fileName);

            Assert.IsNotNull(result);
        }

        [Test, Order(3)]

        public void Test_Non_Pdf_Uploaded_Failure()
        {
            //Arrange
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;


            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.PostFile(file);

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        }

        [Test, Order(4)]

        public void Test_Pdf_Size_Under_5MB_Sucess()
        {
            var fileName = Environment.CurrentDirectory + "\\TestFile\\TestFile-100KB.pdf";

            var stream = File.OpenRead(fileName);

            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));


            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.PostFile(file);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test, Order(5)]

        public void Test_Pdf_Size_Over_5MB_Failure()
        {
            var fileName = Environment.CurrentDirectory + "\\TestFile\\TestFile-5MB.pdf";

            var stream = File.OpenRead(fileName);

            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.PostFile(file);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsCompletedSuccessfully);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);            
        }

        [Test, Order(6)]

        public void Test_API_returns_Document_List()
        {
            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.GetFiles();
         

            Assert.IsNotNull(result);
            
        }

        [Test, Order(7)]

        public void Test_Pdfs_ReOrders_Sucess()
        {
            //have to implement
            
        }



        [Test, Order(8)]

        public void Test_Pdf_deleted_by_file_name_success()
        {
            string filename = "test.pdf";

            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.Delete(filename);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test, Order(9)]

        public void Test_Pdf_deleted_returns_badrequest()
        {
            string filename = "test.pdf";

            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.Delete(filename);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test, Order(10)]

        public void Test_Pdf_delete_the_deleted_one_returns_message()
        {

            string filename = "test.pdf";

            PdfDocumentController controller = new PdfDocumentController();
            var result = controller.Delete(filename);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

        }


        private IFormFile CreateTestFormFile(string p_Name, string p_Content)
        {
            byte[] s_Bytes = Encoding.UTF8.GetBytes(p_Content);

            return new FormFile(
                baseStream: new MemoryStream(s_Bytes),
                baseStreamOffset: 0,
                length: s_Bytes.Length,
                name: "Data",
                fileName: p_Name
            );
        }





    }
}