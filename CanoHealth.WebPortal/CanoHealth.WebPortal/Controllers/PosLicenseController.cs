using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Services.Files;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class PosLicenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _file;

        public PosLicenseController(IUnitOfWork unitOfWork, IFileService file)
        {
            _unitOfWork = unitOfWork;
            _file = file;
        }

        public ActionResult CreateLicense(PlaceOfServiceLicenseFormViewModel license)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please check the required fields.");

                //Catch all the logs
                var auditLogs = new List<AuditLog>();

                var sameNumber = _unitOfWork.Licenses.GetByLicenseNumber(license.LicenseNumber);
                if (sameNumber != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a License with this Number: {license.LicenseNumber}.");

                var licenseType = _unitOfWork.LicenseTypes.GetLicenseTypeByName(license.LicenseTypeName);
                if (licenseType == null)
                {
                    licenseType = new LicenseType
                    {
                        LicenseTypeId = Guid.NewGuid(),
                        LicenseName = license.LicenseTypeName
                    };
                    var licenseTypeLogs = _unitOfWork.LicenseTypes.SaveLicenseTypes(new List<LicenseType> { licenseType });
                    auditLogs.AddRange(licenseTypeLogs);
                }

                var licenseToStore = license.Convert();
                licenseToStore.LicenseTypeId = licenseType.LicenseTypeId;

                //licenseToStore.LicenseTypeId = _unitOfWork.LicenseTypes.GetLicenseTypeId(license.LicenseTypeName);

                var sameType = _unitOfWork.Licenses.GetByTypeAndPlaceOfService(licenseToStore.LicenseTypeId, licenseToStore.PlaceOfServiceId);
                if (sameType != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a {license.LicenseTypeName} License related to this Clinic.");

                licenseToStore.UploadBy = User.Identity.GetUserName();
                licenseToStore.UploaDateTime = DateTime.Now;

                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                licenseToStore.ContentType = (filesCollection.Count > 0 && filesCollection[0] != null)
                    ? filesCollection[0].ContentType
                    : null;

                var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                {
                    new OriginalUniqueNameViewModel
                    {
                        OriginalName = licenseToStore.OriginalFileName,
                        UniqueName = licenseToStore.UniqueFileName
                    }
                };
                _file.AddFiles(filesCollection, licenseToStore.ServerLocation, originalUniqueNameViewModels);

                var locationLogs = _unitOfWork.Licenses.SaveLocationLicenses(new List<PosLicense> { licenseToStore });
                auditLogs.AddRange(locationLogs);

                _unitOfWork.AuditLogs.AddRange(auditLogs);

                _unitOfWork.Complete();

                license.PosLicenseId = licenseToStore.PosLicenseId;
                license.ServerLocation = licenseToStore.ServerLocation;
                license.UploaDateTime = licenseToStore.UploaDateTime;
                license.UploadBy = licenseToStore.UploadBy;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Something failed: {ex}");
            }
            return Json(license);
        }

        public ActionResult UpdateLicense(PlaceOfServiceLicenseFormViewModel license)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please check the required fields.");

                var licenseStoredInDb = _unitOfWork.Licenses.Get(license.PosLicenseId.Value);
                if (licenseStoredInDb == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "This License is not in our system.");

                var sameNumber = _unitOfWork.Licenses.SingleOrDefault(l => l.LicenseNumber == license.LicenseNumber &&
                                 l.PosLicenseId != license.PosLicenseId);
                if (sameNumber != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a License with this Number: {license.LicenseNumber}.");

                var licenseToStore = license.Convert();
                licenseToStore.LicenseTypeId = _unitOfWork.LicenseTypes.GetLicenseTypeId(license.LicenseTypeName);

                var sameType = _unitOfWork.Licenses.SingleOrDefault(l => l.LicenseTypeId == licenseToStore.LicenseTypeId &&
                    l.PlaceOfServiceId == licenseToStore.PlaceOfServiceId &&
                    l.PosLicenseId != licenseToStore.PosLicenseId);
                if (sameType != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a {license.LicenseTypeName} License related to this Clinic.");

                licenseToStore.UploadBy = User.Identity.GetUserName();
                licenseToStore.UploaDateTime = DateTime.Now;

                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                licenseToStore.ContentType = (filesCollection.Count > 0 && filesCollection[0] != null)
                    ? filesCollection[0].ContentType
                    : null;

                var updateLogsGenereted = licenseStoredInDb.Modify(licenseToStore);

                _unitOfWork.AuditLogs.AddRange(updateLogsGenereted);

                _file.AddFiles(filesCollection, licenseToStore.ServerLocation,
                    new List<OriginalUniqueNameViewModel>
                    {
                        new OriginalUniqueNameViewModel {OriginalName = licenseToStore.OriginalFileName, UniqueName = licenseToStore.UniqueFileName}
                    });

                _unitOfWork.Complete();

                license.ServerLocation = licenseToStore.ServerLocation;
                license.UploaDateTime = licenseToStore.UploaDateTime;
                license.UploadBy = licenseToStore.UploadBy;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Something failed: {ex}");
            }
            return Json(license);
        }
    }
}