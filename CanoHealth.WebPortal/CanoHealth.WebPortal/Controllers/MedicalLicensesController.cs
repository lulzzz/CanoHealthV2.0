using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.ModelState;
using CanoHealth.WebPortal.CommonTools.User;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Services.Files;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class MedicalLicensesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _file;
        private readonly IConvertModelState _errorHandler;
        private readonly IUserService _user;
        private readonly ICurrentDateTimeService _dateTime;

        public MedicalLicensesController(IUnitOfWork unitOfWork, IConvertModelState errorHandler, IFileService file, IUserService user, ICurrentDateTimeService dateTime)
        {
            _unitOfWork = unitOfWork;
            _errorHandler = errorHandler;
            _file = file;
            _user = user;
            _dateTime = dateTime;
        }

        public ActionResult CreateMedicalLicense(MedicalLicenseFormViewModel medicalLicenseViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, _errorHandler.GetErrorsFromModelState(ModelState));

                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                if (filesCollection.Count == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please select a valid file.");

                var licenseFound = _unitOfWork.MedicalLicenses.GetLicenseByNumber(medicalLicenseViewModel.LicenseNumber);
                if (licenseFound != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a License with this Number: {medicalLicenseViewModel.LicenseNumber}.");

                var medicalLicenseType = _unitOfWork.MedicalLicenseTypes.GetMedicalLicenseTypeByName(medicalLicenseViewModel.MedicalLicenseTypeName) ??
                                         medicalLicenseViewModel.ConvertToMedicalLicenseType();

                var licenseByDoctorAndType = _unitOfWork.MedicalLicenses.GetLicenseByDoctorAndType(medicalLicenseViewModel.DoctorId, medicalLicenseType.MedicalLicenseTypeId);
                if (licenseByDoctorAndType != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a { medicalLicenseViewModel.MedicalLicenseTypeName } License related to this Doctor.");

                var licenseToStore = medicalLicenseViewModel.ConvertToMedialLicense();
                licenseToStore.MedicalLicenseTypeId = medicalLicenseType.MedicalLicenseTypeId;
                licenseToStore.UploadBy = _user.GetUserName();
                licenseToStore.UploaDateTime = _dateTime.GetCurrentDateTime();

                var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                {
                    new OriginalUniqueNameViewModel
                    {
                        OriginalName = licenseToStore.OriginalFileName,
                        UniqueName = licenseToStore.UniqueFileName
                    }
                };
                _file.AddFiles(filesCollection, licenseToStore.ServerLocation, originalUniqueNameViewModels);

                var auditLogs = new List<AuditLog>();

                var medicalLicenseLogs = _unitOfWork.MedicalLicenses.SaveMedicalLicenses(new List<MedicalLicense> { licenseToStore }).ToList();
                var medicalLicenseTypeLogs = _unitOfWork.MedicalLicenseTypes.SaveMadicalLicenseTypes(new List<MedicalLicenseType> { medicalLicenseType }).ToList();
                auditLogs.AddRange(medicalLicenseLogs);
                auditLogs.AddRange(medicalLicenseTypeLogs);

                _unitOfWork.AuditLogs.AddRange(auditLogs);
                _unitOfWork.Complete();

                medicalLicenseViewModel.UploadBy = licenseToStore.UploadBy;
                medicalLicenseViewModel.UploaDateTime = licenseToStore.UploaDateTime;
                medicalLicenseViewModel.Active = licenseToStore.Active;
                medicalLicenseViewModel.UniqueFileName = licenseToStore.UniqueFileName;
                medicalLicenseViewModel.ServerLocation = licenseToStore.ServerLocation;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Something failed: {ex}");
            }
            return Json(medicalLicenseViewModel);
        }

        public ActionResult UpdateMedicalLicense(MedicalLicenseFormViewModel medicalLicenseViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, _errorHandler.GetErrorsFromModelState(ModelState));

                var licenseFound = _unitOfWork.MedicalLicenses
                    .GetLicenseByNumber(medicalLicenseViewModel.LicenseNumber,
                        medicalLicenseViewModel.MedicalLicenseId);
                if (licenseFound != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a License with this Number: {medicalLicenseViewModel.LicenseNumber}.");

                var medicalLicenseType = _unitOfWork.MedicalLicenseTypes.GetMedicalLicenseTypeByName(medicalLicenseViewModel.MedicalLicenseTypeName) ??
                                         medicalLicenseViewModel.ConvertToMedicalLicenseType();

                var licenseByDoctorAndType = _unitOfWork.MedicalLicenses
                    .GetLicenseByDoctorAndType(medicalLicenseViewModel.DoctorId,
                        medicalLicenseType.MedicalLicenseTypeId,
                        medicalLicenseViewModel.MedicalLicenseId);
                if (licenseByDoctorAndType != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a { medicalLicenseViewModel.MedicalLicenseTypeName } License related to this Doctor.");

                //Getting the physical files that are passing as a FormData from the View
                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                if (filesCollection.Count > 0 && filesCollection[0] != null)
                {
                    medicalLicenseViewModel.UploadBy = _user.GetUserName();
                    medicalLicenseViewModel.UploaDateTime = _dateTime.GetCurrentDateTime();
                    medicalLicenseViewModel.UniqueFileName = Guid.NewGuid() + medicalLicenseViewModel.FileExtension;
                    var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                    {
                        new OriginalUniqueNameViewModel
                        {
                            OriginalName = medicalLicenseViewModel.OriginalFileName,
                            UniqueName = medicalLicenseViewModel.UniqueFileName
                        }
                    };
                    //Save file in Physical Path
                    _file.AddFiles(filesCollection, medicalLicenseViewModel.ServerLocation, originalUniqueNameViewModels);
                }

                var medicalLicense = new MedicalLicense
                {
                    MedicalLicenseId = medicalLicenseViewModel.MedicalLicenseId.Value,
                    DoctorId = medicalLicenseViewModel.DoctorId,
                    MedicalLicenseTypeId = medicalLicenseType.MedicalLicenseTypeId,
                    LicenseNumber = medicalLicenseViewModel.LicenseNumber,
                    EffectiveDate = medicalLicenseViewModel.EffectiveDate,
                    ExpireDate = medicalLicenseViewModel.ExpireDate,
                    Note = medicalLicenseViewModel.Note,
                    ServerLocation = medicalLicenseViewModel.ServerLocation,
                    OriginalFileName = medicalLicenseViewModel.OriginalFileName,
                    UniqueFileName = medicalLicenseViewModel.UniqueFileName,
                    FileExtension = medicalLicenseViewModel.FileExtension,
                    FileSize = medicalLicenseViewModel.FileSize,
                    ContentType = medicalLicenseViewModel.ContentType,
                    UploadBy = medicalLicenseViewModel.UploadBy,
                    UploaDateTime = medicalLicenseViewModel.UploaDateTime,
                    Active = medicalLicenseViewModel.Active
                };
                var auditLogs = new List<AuditLog>();

                var medicalLicenseLogs = _unitOfWork.MedicalLicenses.SaveMedicalLicenses(new List<MedicalLicense> { medicalLicense }).ToList();
                var medicalLicenseTypeLogs = _unitOfWork.MedicalLicenseTypes.SaveMadicalLicenseTypes(new List<MedicalLicenseType> { medicalLicenseType }).ToList();
                auditLogs.AddRange(medicalLicenseLogs);
                auditLogs.AddRange(medicalLicenseTypeLogs);

                _unitOfWork.AuditLogs.AddRange(auditLogs);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Something failed: {ex}");
            }
            return Json(medicalLicenseViewModel);
        }
    }
}