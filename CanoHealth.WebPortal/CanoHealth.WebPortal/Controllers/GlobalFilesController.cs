using Elmah;
using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class GlobalFilesController : Controller
    {
        // GET: GlobalFiles
        public ActionResult Download(string originalFileName, string uniqueFileName, string contentType, string serverLocation)
        {
            WindowsImpersonationContext ctx = null;
            try
            {
                //IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle(ConfigureSettings.ImpersonationUsrName,
                //    ConfigureSettings.ImpersonationPassword,
                //    ConfigureSettings.ImpersonationDomain);
                //WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
                //ctx = wi.Impersonate();
                if (!Directory.Exists(serverLocation))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "The Directory does not exist.");
                }
                if (!System.IO.File.Exists(serverLocation + uniqueFileName))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "The File does not exist.");
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,
                    $"An exception has occurred: {ex}");
            }
            finally
            {
                if (ctx != null)
                {
                    ctx.Undo();
                }
            }
            return File(serverLocation + uniqueFileName, contentType, originalFileName);
        }
    }
}