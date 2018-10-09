using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        // GET: Schedules
        public ActionResult Index()
        {
            return View();
        }
    }
}