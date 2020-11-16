using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace IdentityApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string host;
        private readonly int port;
        private readonly bool enableSSL;
        private readonly string userName;
        private readonly string password;
        private readonly string owner;

        public EmailSender(string host, int port, bool ssl, string userName, string password, string owner)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = ssl;
            this.userName = userName;
            this.password = password;
            this.owner = owner;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(this.owner, this.userName));
            emailMessage.To.Add(new MailboxAddress(string.Empty, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(this.host, this.port, this.enableSSL);
                await client.AuthenticateAsync(this.userName, this.password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
    
