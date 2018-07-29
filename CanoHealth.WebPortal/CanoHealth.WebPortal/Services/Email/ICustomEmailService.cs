using CanoHealth.WebPortal.Core.Domain;
using System.Threading.Tasks;

namespace CanoHealth.WebPortal.Services.Email
{
    public interface ICustomEmailService
    {
        Task SendSmtpEmailAsync(Emails email);

        Task SendApiEmailAsync();
    }
}
