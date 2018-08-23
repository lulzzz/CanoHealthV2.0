using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.User;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using CanoHealth.WebPortal.Services.Files;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class PersonalFilesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _user;
        private readonly ICurrentDateTimeService _dateTime;
        private readonly IFileService _file;

        public PersonalFilesController(IUnitOfWork unitOfWork, IUserService userSrv, ICurrentDateTimeService dateTime, IFileService file)
        {
            _unitOfWork = unitOfWork;
            _user = userSrv;
            _dateTime = dateTime;
            _file = file;
        }

        public async Task<ActionResult> SavePersonalFile(DoctorFileFormViewModel doctorFileForm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please check the required fields.");

                //Catch all the logs
                var auditLogs = new List<AuditLog>();

                //Get the Doctor File Type knowing the name
                var doctorFileType = _unitOfWork.PersonalFileTypeRepository.GetFileTypeByName(doctorFileForm.DoctorFileTypeName);
                //If does not exist create it.
                if (doctorFileType == null)
                {
                    doctorFileType = new DoctorFileType
                    {
                        DoctorFileTypeId = Guid.NewGuid(),
                        DoctorFileTypeName = doctorFileForm.DoctorFileTypeName
                    };
                    var logs = _unitOfWork.PersonalFileTypeRepository.SaveFileTypes(new List<DoctorFileType> { doctorFileType });
                    auditLogs.AddRange(logs);
                }
                //Assign the DoctorFileTypeId to doctorFileForm ViewModel
                doctorFileForm.DoctorFileTypeId = doctorFileType.DoctorFileTypeId;
                //Check if doctor has already a file associate to him with the same type.
                var doctorFileStoredInDb = _unitOfWork.PersonalFileRepository.GetDoctorFileByType(doctorFileForm.DoctorId, doctorFileForm.DoctorFileTypeId.Value);
                if (doctorFileStoredInDb != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a {doctorFileForm.DoctorFileTypeName} file associated to this Doctor.");

                //Create a new instance of DoctorFile to store in Db 
                var doctorFileToStore = doctorFileForm.Convert();
                doctorFileToStore.UploadBy = _user.GetUserName();
                doctorFileToStore.UploadDateTime = _dateTime.GetCurrentDateTime();

                //Getting the physical files that are passing as a FormData from the View
                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                doctorFileToStore.ContentType = (filesCollection.Count > 0 && filesCollection[0] != null)
                    ? filesCollection[0].ContentType
                    : null;

                var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                {
                    new OriginalUniqueNameViewModel
                    {
                        OriginalName = doctorFileToStore.OriginalFileName,
                        UniqueName = doctorFileToStore.UniqueFileName
                    }
                };
                //Save file in Physical Path
                //_file.AddFiles(filesCollection, doctorFileToStore.ServerLocation, originalUniqueNameViewModels);
                await _file.SaveFileAzureStorageAccount(filesCollection, originalUniqueNameViewModels,
                   ConfigureSettings.GetPersonalContainer);

                //Save file info in Database
                var fileLogs = _unitOfWork.PersonalFileRepository.SavePersonalFiles(new List<DoctorFile> { doctorFileToStore });
                auditLogs.AddRange(fileLogs);

                //Save the logs in Database
                _unitOfWork.AuditLogs.AddRange(auditLogs);

                //Complete the transaction
                _unitOfWork.Complete();

                doctorFileForm.DoctorFileId = doctorFileToStore.DoctorFileId;
                doctorFileForm.ServerLocation = doctorFileToStore.ServerLocation;
                doctorFileForm.UploadDateTime = doctorFileToStore.UploadDateTime;
                doctorFileForm.UploadBy = doctorFileToStore.UploadBy;
                doctorFileForm.UniqueFileName = doctorFileToStore.UniqueFileName;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Something failed: {ex}");
            }

            return Json(doctorFileForm);
        }

        public async Task<ActionResult> UpdatePersonalFile(DoctorFileFormViewModel doctorFileForm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please check the required fields.");

                var doctorFileStoredInDb = _unitOfWork
                    .PersonalFileRepository
                    .Get(doctorFileForm.DoctorFileId.Value);
                if (doctorFileStoredInDb == null)
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "File not found.");

                //Catch all the logs
                var auditLogs = new List<AuditLog>();

                //Get the Doctor File Type knowing the name
                var doctorFileType = _unitOfWork.PersonalFileTypeRepository.GetFileTypeByName(doctorFileForm.DoctorFileTypeName);
                //If does not exist create it.
                if (doctorFileType == null)
                {
                    doctorFileType = new DoctorFileType
                    {
                        DoctorFileTypeId = Guid.NewGuid(),
                        DoctorFileTypeName = doctorFileForm.DoctorFileTypeName
                    };
                    var logs = _unitOfWork.PersonalFileTypeRepository.SaveFileTypes(new List<DoctorFileType> { doctorFileType });
                    auditLogs.AddRange(logs);
                }
                //Assign the DoctorFileTypeId to doctorFileForm ViewModel
                doctorFileForm.DoctorFileTypeId = doctorFileType.DoctorFileTypeId;
                //Check if doctor has already a file associate to him with the same type.
                var duplicateDoctorFile = _unitOfWork.PersonalFileRepository
                    .GetDoctorFileByType(doctorFileForm.DoctorId,
                        doctorFileForm.DoctorFileTypeId.Value,
                        doctorFileForm.DoctorFileId);
                if (duplicateDoctorFile != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"There is a {doctorFileForm.DoctorFileTypeName} file associated to this Doctor.");

                //Getting the physical files that are passing as a FormData from the View
                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                if (filesCollection.Count > 0 && filesCollection[0] != null)
                {
                    doctorFileForm.UploadBy = _user.GetUserName();
                    doctorFileForm.UploadDateTime = _dateTime.GetCurrentDateTime();
                    doctorFileForm.UniqueFileName = Guid.NewGuid() + doctorFileForm.FileExtension;
                    var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                    {
                        new OriginalUniqueNameViewModel
                        {
                            OriginalName = doctorFileForm.OriginalFileName,
                            UniqueName = doctorFileForm.UniqueFileName
                        }
                    };
                    //Save file in Physical Path
                    //_file.AddFiles(filesCollection, doctorFileForm.ServerLocation, originalUniqueNameViewModels);
                    await _file.SaveFileAzureStorageAccount(filesCollection, originalUniqueNameViewModels,
                        ConfigureSettings.GetPersonalContainer);
                }

                var newDoctorFile = doctorFileForm.Convert();
                var fileLogs = doctorFileStoredInDb.Modify(newDoctorFile);

                //Save the logs in Database
                auditLogs.AddRange(fileLogs);
                _unitOfWork.AuditLogs.AddRange(auditLogs);

                //Complete the transaction
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"Something failed: {ex}");
            }

            return Json(doctorFileForm);
        }
    }
}