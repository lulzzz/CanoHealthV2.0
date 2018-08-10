using CanoHealth.WebPortal.Infraestructure;
using CanoHealth.WebPortal.ViewModels;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
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

        public void DownloadFile(string uniqueFileName, string container, string pathToDownload)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a CloudFileClient object for credentialed access to Azure Files.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference(ConfigurationDAL.GetShare);//"canohealth"

            // Get a reference to the root directory for the share.
            CloudFileDirectory rootDir = share.GetRootDirectoryReference();

            // Get a reference to the directory we created previously.
            CloudFileDirectory sampleDir = rootDir.GetDirectoryReference(container);

            CloudFile file = sampleDir.GetFileReference(uniqueFileName);

            file.DownloadToFile(Path.Combine(pathToDownload, uniqueFileName), FileMode.Create);
        }

        public void SaveFileAzureStorageAccount(HttpFileCollectionBase filesCollection,
          IEnumerable<OriginalUniqueNameViewModel> originalFileNames, string container)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a CloudFileClient object for credentialed access to Azure Files.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference(ConfigurationDAL.GetShare);//"canohealth"

            // Get a reference to the root directory for the share.
            CloudFileDirectory rootDir = share.GetRootDirectoryReference();

            // Get a reference to the directory we created previously.
            CloudFileDirectory sampleDir = rootDir.GetDirectoryReference(container);

            for (int i = 0; i < filesCollection.Count; i++)
            {
                var httpPostedFileBase = filesCollection[i];

                if (httpPostedFileBase != null)
                {
                    var fileName = originalFileNames.Single(f => f.OriginalName == httpPostedFileBase.FileName);                    
                    CloudFile file = sampleDir.GetFileReference(fileName.UniqueName);
                    file.UploadFromStream(httpPostedFileBase.InputStream);
                }
            }
        }
    }
}