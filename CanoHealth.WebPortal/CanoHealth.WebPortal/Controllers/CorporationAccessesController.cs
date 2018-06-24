using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class CorporationAccessesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CorporationAccessesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetCorporationByUser([DataSourceRequest] DataSourceRequest request)
        {
            var userId = User.Identity.GetUserId();
            var corporations = _unitOfWork.UserCorporationAccessRepository
                    .GetCorporationAccessByUser(userId)
                    .Select(Mapper.Map<Corporation, CorporationViewModel>)
                    .ToList();
            return Json(corporations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}