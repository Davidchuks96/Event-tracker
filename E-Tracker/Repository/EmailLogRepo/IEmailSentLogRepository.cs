using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.EmailLogRepo
{
    public interface IEmailSentLogRepository
    {
        Task<bool> LogEmailAsync(MailLog emailLog);
        Task<bool> LogEmailTransactionAsync(MailLog emailLog);

        Task<bool> UpdateEmailAsync(MailLog emailLog);

        Task<MailLog> GetEmailSentByIdAsync(int Id);

        Task<IEnumerable<MailLog>> GetEmailSentLogAsync();
        Task<IEnumerable<MailLog>> GetPendingEmailLogAsync();
    }
}
