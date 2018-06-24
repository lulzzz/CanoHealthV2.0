using AutoMapper;
using CanoHealth.WebPortal.Core;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels.Admin;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    public class RolesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RolesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadRoles([DataSourceRequest] DataSourceRequest request)
        {
            var roles = _unitOfWork.RoleRepository
                .GetRoles()
                .Select(Mapper.Map<ApplicationRole, RoleViewModel>)
                .ToList();

            return Json(roles.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}