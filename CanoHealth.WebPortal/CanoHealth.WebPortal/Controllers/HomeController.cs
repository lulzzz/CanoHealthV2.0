using System.Web.Mvc;

namespace CanoHealth.WebPortal.Controllers
{
    //Testing StructureMap IoC
    public interface IMessageProvider
    {
        string GetMessage();
    }

    public class MessageProvider : IMessageProvider
    {
        public string GetMessage()
        {
            return "Message from " + this.GetType();
        }
    }

    public class MessageProvider1 : IMessageProvider
    {
        public string GetMessage()
        {
            return "Hello World!";
        }
    }

    public class HomeController : Controller
    {
        private readonly IMessageProvider _messageProvider;

        public HomeController(IMessageProvider messageProvider)
        {
            _messageProvider = messageProvider;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your app description page.";
            ViewBag.Message = _messageProvider.GetMessage();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logggin()
        {
            return View();
        }
    }
}