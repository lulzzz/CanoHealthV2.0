using CanoHealth.WebPortal.Services.Email;
using System.Threading.Tasks;
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

        private readonly IEmailService _email;

        public HomeController(IMessageProvider messageProvider, IEmailService email)
        {
            _messageProvider = messageProvider;
            _email = email;
        }

        public async Task<ActionResult> Index()
        {
            var email = new EmailService();
            email.To.Add("suarezhar@gmail.com");
            email.Subject = "Confirm your account";


            email.Body = "<strong>Hello World</strong>";
            await email.SendSmtpEmailAsync();

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