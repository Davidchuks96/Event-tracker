using System.Threading.Tasks;

namespace E_Tracker.Repository.MailRepo
{
    public interface IMailService
    {       
        Task SendMail(string email, string message, string title);
    }
}
