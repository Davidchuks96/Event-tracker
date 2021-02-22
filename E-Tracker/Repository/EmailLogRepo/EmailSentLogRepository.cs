using E_Tracker.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.EmailLogRepo
{
    public class EmailSentLogRepository: IEmailSentLogRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailSentLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LogEmailAsync(MailLog emailLog)
        {
            if (emailLog != null)
            {

                await _context.MailLogs.AddAsync(emailLog);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> LogEmailTransactionAsync(MailLog emailLog)
        {
            if (emailLog != null)
            {

                await _context.MailLogs.AddAsync(emailLog);
                //await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateEmailAsync(MailLog emailLog)
        {
            if (emailLog != null)
            {
                _context.Update(emailLog);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<MailLog> GetEmailSentByIdAsync(int Id)
        {
            return await _context.MailLogs.FindAsync(Id);
        }
        public async Task<IEnumerable<MailLog>> GetEmailSentLogAsync()
        {
            return await _context.MailLogs.ToListAsync();
        }
        public async Task<IEnumerable<MailLog>> GetPendingEmailLogAsync()
        {
            return await _context.MailLogs.Where(x => !x.IsSent).ToListAsync();
        }
    }
}
