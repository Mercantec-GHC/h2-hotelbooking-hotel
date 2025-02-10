using Microsoft.AspNetCore.Identity.UI.Services;

namespace BackendAPI.Services
{
    public class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implementer logik til at sende e-mails her.
            // For testformål kan du bare logge det til konsollen eller gøre ingenting.
            Console.WriteLine($"Send email to {email} with subject {subject}");
            return Task.CompletedTask;
        }
    }
}
