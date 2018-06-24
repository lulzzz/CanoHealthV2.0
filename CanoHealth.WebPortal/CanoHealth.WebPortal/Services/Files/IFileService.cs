using CanoHealth.WebPortal.Controllers;
using System.Collections.Generic;
using System.Web;
using CanoHealth.WebPortal.ViewModels;

namespace CanoHealth.WebPortal.Services.Files
{
    public interface IFileService
    {
        void AddFiles(HttpFileCollectionBase filesCollection, string path,
            IEnumerable<OriginalUniqueNameViewModel> originalFileNames);
    }
}