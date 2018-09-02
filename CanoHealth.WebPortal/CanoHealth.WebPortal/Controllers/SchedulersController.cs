using CanoHealth.WebPortal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize]
    public class SchedulersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SchedulersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Schedulers
        public ActionResult Index()
        {
            return View("Scheduler");
        }
    }
}