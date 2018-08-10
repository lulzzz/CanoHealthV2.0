using CanoHealth.WebPortal.Infraestructure;
using CanoHealth.WebPortal.Services.Files;
using Elmah;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class FilesController : ApiController
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public HttpResponseMessage Download(string originalFileName, string uniqueFileName, string contentType, string container)
        {
            var content = new HttpResponseMessage();
            try
            {
                //The path to the target file in the local file system.
                var localFileSystemDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    ConfigureSettings.GetLocalFileSystem);//@"Documents\"

                //Create the path if it does not exist
                if (!Directory.Exists(localFileSystemDirectory))
                {
                    Directory.CreateDirectory(localFileSystemDirectory);
                }

                var filePath = Path.Combine(localFileSystemDirectory, uniqueFileName);

                //Download the file from Storage Account to local file system
                _fileService.DownloadFile(uniqueFileName,
                    container,
                    localFileSystemDirectory);

                //Ge the file's info.
                var info = File.ReadAllBytes(filePath);

                //Create the content
                content.Content = new ByteArrayContent(info);
                content.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = originalFileName
                };

                //Delete file from local file system
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                content.ReasonPhrase = "failed";
            }

            return content;
        }
    }
}
