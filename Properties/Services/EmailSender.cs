using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace TatryExplorer.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implementacja wysyłania e-maili. Możesz użyć np. SMTP lub innej usługi e-mail.
            return Task.CompletedTask;
        }
    }
}
