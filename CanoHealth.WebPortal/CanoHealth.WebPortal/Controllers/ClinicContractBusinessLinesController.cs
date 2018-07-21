using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Dtos;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class ClinicContractBusinessLinesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicContractBusinessLinesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetLocationsByContractLineOfBusiness(
            [DataSourceRequest] DataSourceRequest request,
            Guid contractLineofBusinessId, Guid? insuranceId = null)
        {
            //Get active locations related to an specific line of business.
            var locations = _unitOfWork.ContracBusinessLineClinicRepository
                                       .GetLocationsByBusinessLines(contractLineofBusinessId)
                                       .Select(x => new ContractBusinessLineLocationDto
                                       {
                                           ContractLineofBusinessId = contractLineofBusinessId,
                                           PlaceOfServiceId = x.PlaceOfServiceId,
                                           CorporationId = x.CorporationId,
                                           Name = x.Name,
                                           Address = x.Address,
                                           PhoneNumber = x.PhoneNumber,
                                           FaxNumber = x.FaxNumber,
                                           Active = x.Active,
                                           InsuranceId = insuranceId
                                       })
                                       .ToList();

            return Json(locations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}