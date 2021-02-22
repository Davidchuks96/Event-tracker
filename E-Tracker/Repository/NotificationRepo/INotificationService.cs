using E_Tracker.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.EmailRepo
{
    public interface INotificationService
    {
        List<Notification> GetUserNotifications(string userId);
        Task AddCreateNotificationUsers(Notification notification, string userDepartmentId);
        Task AddApprovalNotificationUsers(Notification notification, string type);
        Task SendCreateItemNotifications(string itemId, User user);
        Task SendApproveItemNotifications(string itemId, User user);
        Task SendCreateItemGroupNotifications(string itemGroupId, User user);
        Task SendApproveItemGroupNotifications(string itemGroupId, User user);
        Task SendCreateServiceNotifications(string itemId, string serviceId, User user);
        Task SendApproveServiceNotifications(string itemId, string serviceId, User user);
        void ReadNotification(int notificationId, string userId);
    }
}