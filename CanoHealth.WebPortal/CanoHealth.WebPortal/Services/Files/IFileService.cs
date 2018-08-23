using CanoHealth.WebPortal.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace CanoHealth.WebPortal.Services.Files
{
    public interface IFileService
    {
        void AddFiles(HttpFileCollectionBase filesCollection, string path,
            IEnumerable<OriginalUniqueNameViewModel> originalFileNames);

        void DownloadFile(string uniqueFileName, string container, string pathToDownload);

        Task SaveFileAzureStorageAccount(HttpFileCollectionBase filesCollection,
          IEnumerable<OriginalUniqueNameViewModel> originalFileNames, string container);
    }
}