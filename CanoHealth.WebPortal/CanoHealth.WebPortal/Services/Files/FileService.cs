using CanoHealth.WebPortal.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CanoHealth.WebPortal.Services.Files
{
    public class FileService : IFileService
    {
        public void AddFiles(HttpFileCollectionBase filesCollection, string path,
            IEnumerable<OriginalUniqueNameViewModel> originalFileNames)
        {
            if (filesCollection.Count > 0)
            {
                //IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle(ConfigureSettings.ImpersonationUsrName,
                //    ConfigureSettings.ImpersonationPassword, ConfigureSettings.ImpersonationDomain);

                //WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
                //WindowsImpersonationContext wic = null;

                //wic = wi.Impersonate();

                for (int i = 0; i < filesCollection.Count; i++)
                {
                    var httpPostedFileBase = filesCollection[i];

                    if (httpPostedFileBase != null)
                    {
                        var fileName = originalFileNames.Single(f => f.OriginalName == httpPostedFileBase.FileName);
                        httpPostedFileBase.SaveAs(Path.Combine(path, fileName.UniqueName));
                    }
                }
                //wic.Undo();
            }
        }
    }
}