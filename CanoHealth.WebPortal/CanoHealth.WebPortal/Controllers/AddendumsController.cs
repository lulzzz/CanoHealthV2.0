using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Services.Files;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class AddendumsController : Controller
    {
        private readonly IFileService _file;
        private readonly IUnitOfWork _unitOfWork;

        public AddendumsController(IUnitOfWork unitOfWork, IFileService file)
        {
            _unitOfWork = unitOfWork;
            _file = file;
        }

        public ActionResult CreateAddendum(ContractAddendumFormViewModel addendum)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ConvertModelState(ModelState));

                var addendumToStore = addendum.Convert();
                addendumToStore.UploadBy = User.Identity.GetUserName();
                addendumToStore.UploadDateTime = DateTime.Now;

                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                if (filesCollection.Count == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please select a valid file.");
                var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                {
                    new OriginalUniqueNameViewModel
                    {
                        OriginalName = addendumToStore.OriginalFileName,
                        UniqueName = addendumToStore.UniqueFileName
                    }
                };
                _file.AddFiles(filesCollection, addendumToStore.ServerLocation, originalUniqueNameViewModels);

                _unitOfWork.Addendums.SaveAddendums(new List<ContractAddendum> { addendumToStore });
                _unitOfWork.Complete();
                addendum.ContractAddendumId = addendumToStore.ContractAddendumId;
                addendum.UploadDateTime = addendumToStore.UploadDateTime;
                addendum.UploadBy = addendumToStore.UploadBy;
                addendum.ServerLocation = addendumToStore.ServerLocation;
                addendum.UniqueFileName = addendumToStore.UniqueFileName;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Something failed. Please contact your system administrator.");
            }
            return Json(addendum);
        }

        public ActionResult UpdateAddendum(ContractAddendumFormViewModel addendum)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ConvertModelState(ModelState));

                var contractAddendumConverted = Mapper.Map(addendum, new ContractAddendum());

                HttpFileCollectionBase filesCollection = HttpContext.Request.Files;
                if (filesCollection.Count > 0 && filesCollection[0] != null)
                {
                    contractAddendumConverted.UploadBy = User.Identity.GetUserName();
                    contractAddendumConverted.UploadDateTime = DateTime.Now;
                    contractAddendumConverted.UniqueFileName = Guid.NewGuid() + addendum.FileExtension;
                    addendum.UniqueFileName = contractAddendumConverted.UniqueFileName;
                    addendum.UploadDateTime = contractAddendumConverted.UploadDateTime;
                    addendum.UploadBy = contractAddendumConverted.UploadBy;

                    var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel>
                    {
                        new OriginalUniqueNameViewModel
                        {
                            OriginalName = contractAddendumConverted.OriginalFileName,
                            UniqueName = contractAddendumConverted.UniqueFileName
                        }
                    };
                    _file.AddFiles(filesCollection, contractAddendumConverted.ServerLocation, originalUniqueNameViewModels);
                }
                _unitOfWork.Addendums.SaveAddendums(new List<ContractAddendum> { contractAddendumConverted });
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Something failed. Please contact your system administrator.");
            }
            return Json(addendum);
        }

        private string ConvertModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
            string errors = string.Join(" ", errorList.ToArray());
            return errors;
        }
    }
}