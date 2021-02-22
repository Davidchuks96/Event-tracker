using E_Tracker.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.MailRepo
{
    public class MailService:IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _logger;
        private readonly ApplicationDbContext _context;

        public MailService(IConfiguration configuration, ILogger<MailService> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task SendMail(string email, string message, string title)
        {
            await PostMail(email, message, title);

        }

        public async Task PostMail(string email, string message, string title)
        {
            var payload = new
            {
                From = _configuration.GetValue<string>("Notification:From"),
                recieverMail = new List<string> { email },
                subject = title,
                messageBody = message
            };

            await _context.MailLogs.AddAsync(new MailLog
            {
                IsSent = false,
                MessageBody = payload.messageBody,
                From = payload.From,
                Receiver = email,
                Subject = payload.subject
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation($"{email} has been logged successfully for processing");

        }

        
    }
}

