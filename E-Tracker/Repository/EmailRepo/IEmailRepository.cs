using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.EmailRepo
{
    public interface IEmailRepository
    {
        Task<bool> SendMailAsync(MailLog mailLog = null);
        Task<bool> SendEmailAsync(string email, string subject, string message, string attachedfiles);
        Task SendEmailAsync(string email, string CC, string subject, string message, string attachedfiles);
        Task SendEmailTransactionAsync(string email, string subject, string message, string attachedfiles);

    }
}
