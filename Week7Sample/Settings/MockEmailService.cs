using System;
using System.Net.Mail;

namespace Week7Sample.Common.Services
{
    public class MockEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(email);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    client.PickupDirectoryLocation = "c:\\temp\\maildrop";
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
