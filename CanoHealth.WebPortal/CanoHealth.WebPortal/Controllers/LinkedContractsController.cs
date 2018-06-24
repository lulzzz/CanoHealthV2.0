using CanoHealth.WebPortal.Core;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class LinkedContractsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LinkedContractsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult ReadLinkedContracts([DataSourceRequest] DataSourceRequest request,
            Guid doctorId)
        {
            var result = _unitOfWork.LinkedContractStoredProcedures
                .GetDoctorLinkedContractInfo(doctorId)
                .ToList();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}