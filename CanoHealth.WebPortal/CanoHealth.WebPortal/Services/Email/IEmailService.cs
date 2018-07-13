using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Services.Email
{
    public interface IEmailService
    {
        Task SendSmtpEmailAsync();

        Task SendApiEmailAsync();
    }
}
