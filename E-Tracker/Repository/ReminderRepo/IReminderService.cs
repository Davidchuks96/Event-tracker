using E_Tracker.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ReminderRepo
{
    public interface IReminderService
    {
        Task SendReminderMail();
        Task SendExpiredItemMail();
        Task SetReminderLog(Item item, string itemCreatorEmail);
        Task UpdateReminderLog(Item item, string itemCreatorEmail);
        Task DeleteReminderLog(string itemId);
        Task DeleteReminderLog(Item item);
        Task<IEnumerable<Reminder>> GetAllRemindersAsync();
        Task EscalateItemReminder(Item item, string itemCreatorEmail, string itemReminderEscalatorId);
        IEnumerable<Reminder> GetUnSentReminders();
    }
}