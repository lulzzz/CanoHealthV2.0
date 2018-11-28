using CanoHealth.WebPortal.CommonTools.ModelState;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Infraestructure;
using CanoHealth.WebPortal.Services.Files;
using CanoHealth.WebPortal.ViewModels;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class ContractsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _file;
        private readonly IConvertModelState _errorMessage;

        public ContractsController(IUnitOfWork unitOfWork, IFileService file, IConvertModelState errorMessage)
        {
            _unitOfWork = unitOfWork;
            _file = file;
            _errorMessage = errorMessage;
        }

        public ActionResult Index()
        {
            ViewData["Corporations"] = _unitOfWork.Corporations.GetActiveCorporations()
                .Select(x => new { x.CorporationId, x.CorporationName });
            return View("Contracts");
        }

        public ActionResult ReadContracts([DataSourceRequest] DataSourceRequest request)
        {
            var result = _unitOfWork.Contracts.GetContractWithInsurance()
                .Select(ContractFormViewModel.Wrap).ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateContract(ContractFormViewModel contract)
        {
            try
            {
                var auditLogs = new List<AuditLog>();
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, _errorMessage.GetErrorsFromModelState(ModelState));

                var contractByGroup = _unitOfWork.Contracts.GetContractByGroupNumber(contract.GroupNumber);
                if (contractByGroup != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Duplicate Data. There is a contract with the same Group Number.");

                var insuranceByName = _unitOfWork.Insurances.GetByName(contract.InsuranceName); //give me the insurance with its contracts
                if (insuranceByName != null)
                {
                    if (!insuranceByName.Active)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Inactive Insurance.");
                    var searchContract = insuranceByName.GetContract(contract.CorporationId);
                    if (searchContract != null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Duplicate Data. There is a contract for this Corporation and Insurance.");
                    contract.InsuranceId = insuranceByName.InsuranceId;
                }
                else
                {
                    contract.InsuranceId = Guid.NewGuid();
                    var insuranceLogs = _unitOfWork.Insurances.SaveItems(new List<Insurance> { contract.ConvertToInsurance() });
                    auditLogs.AddRange(insuranceLogs);
                }

                contract.ContractId = Guid.NewGuid();
                var contractLogs = _unitOfWork.Contracts.SaveContracts(new List<Contract> { contract.ConvertToContract() });
                auditLogs.AddRange(contractLogs);

                if (!String.IsNullOrEmpty(contract.OriginalFileName))
                {
                    var contractAddendum = contract.ContractAddendum(User.Identity.GetUserName(), DateTime.Now);
                    var addendumLogs = _unitOfWork.Addendums.SaveAddendums(new List<ContractAddendum> { contractAddendum });
                    auditLogs.AddRange(addendumLogs);
                    var originalUniqueNameViewModels = new List<OriginalUniqueNameViewModel> {
                        new OriginalUniqueNameViewModel
                        {
                            UniqueName = contractAddendum.UniqueFileName,
                            OriginalName = contractAddendum.OriginalFileName
                        }
                    };
                    _file.SaveFileAzureStorageAccount(HttpContext.Request.Files, originalUniqueNameViewModels, ConfigureSettings.GetAddendumsContainer);
                }
                _unitOfWork.AuditLogs.AddRange(auditLogs);
                _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, $"An exception occurred: {ex}");
            }
            return Json(contract);
        }

        public ActionResult UpdateContracts([DataSourceRequest] DataSourceRequest request,
            ContractFormViewModel contract)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contractByGroupNumber = _unitOfWork.Contracts
                        .GetContractByGroupNumber(contract.GroupNumber);
                    if (contractByGroupNumber != null && contractByGroupNumber.ContractId != contract.ContractId)
                    {
                        ModelState.AddModelError("GroupNumber", @"Duplicate Data. There is a contract with the same Group Number.");
                        return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
                    }

                    var insuranceByName = _unitOfWork.Insurances
                        .GetByName(contract.InsuranceName); //give me the insurance with its contracts
                    if (insuranceByName != null)
                    {
                        if (!insuranceByName.Active)
                        {
                            ModelState.AddModelError("InsuranceName", @"Inactive Insurance.");
                            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
                        }

                        var searchContract = insuranceByName.GetContract(contract.CorporationId);
                        if (searchContract != null && searchContract.ContractId != contract.ContractId)
                        {
                            ModelState.AddModelError("", @"Duplicate Data. There is a contract for this Corporation and Insurance.");
                            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
                        }
                        contract.InsuranceId = insuranceByName.InsuranceId;
                    }
                    else
                    {
                        contract.InsuranceId = Guid.NewGuid();
                        var insuranceLogs = _unitOfWork.Insurances.SaveItems(new List<Insurance> { contract.ConvertToInsurance() });
                        _unitOfWork.AuditLogs.AddRange(insuranceLogs);
                    }
                    var contractLogs = _unitOfWork.Contracts.SaveContracts(new List<Contract> { contract.ConvertToContract() });
                    _unitOfWork.AuditLogs.AddRange(contractLogs);

                    _unitOfWork.Complete();
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }

            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
        }
    }
}