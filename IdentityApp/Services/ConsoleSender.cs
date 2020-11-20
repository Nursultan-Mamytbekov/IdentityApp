using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace IdentityApp.Services
{
    public class ConsoleSender : IEmailSender
    {
        private readonly ILogger<ConsoleSender> logger;
        public ConsoleSender(ILogger<ConsoleSender> logger)
        {
            this.logger = logger;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await Task.Run(() => logger.LogInformation(email));
            await Task.Run(() => logger.LogInformation(subject));
            await Task.Run(() => logger.LogInformation(message));          
        }
    }
}
