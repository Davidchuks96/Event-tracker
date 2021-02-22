using E_Tracker.Authorization;
using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Data.Hubs;
using E_Tracker.Repository.ItemGroupRepository;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.ServiceRepo;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.EmailRepo
{
    public class NotificationService:INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IHubContext<SignalServer> _hubContext;
        private readonly IServiceRepository _serviceRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IItemGroupRepository _itemGroupRepository;

        public NotificationService(ApplicationDbContext context, IConfiguration configuration, ILogger<NotificationService> logger,
            RoleManager<Role> roleManager, UserManager<User> userManager, IItemRepository itemRepository, IItemGroupRepository itemGroupRepository,
            IHubContext<SignalServer> hubContext, IServiceRepository serviceRepository, IEmailRepository emailRepository)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
            _serviceRepository = serviceRepository;
            _emailRepository = emailRepository;
            _itemRepository = itemRepository;
            _itemGroupRepository = itemGroupRepository;
        }

        //Accept id(s) of user(s) to be notified
        public async Task AddCreateNotificationUsers(Notification notification, string userDepartmentId)
        {
            //notification sent to notifcation trigger user head-'Admin' 
            var users = await _userManager.GetUsersInRoleAsync(RolesList.Admin);
            var userNotification = new List<NotificationUser>();
            foreach (var adminUser in users.Where(x => x.DepartmentId == userDepartmentId))
            {
                SendCreateNotificationEmail(adminUser, notification);
                userNotification.Add(new NotificationUser
                {
                    UserId = adminUser.Id,
                    NotificationId = notification.Id
                });
            }
            _context.NotificationUsers.AddRange(userNotification);
            await _context.SaveChangesAsync();
        }
        
        //Accept id(s) of user(s) to be notified
        public async Task AddApprovalNotificationUsers(Notification notification, string type)
        {
            //send notification to the user that created the Item
            if (type == "Service")
            {
                _context.NotificationUsers.Add(new NotificationUser
                {
                    UserId = notification.Service?.CreatedById,
                    NotificationId = notification.Id
                });

            }
            else if (type == "Item")
            {
                _context.NotificationUsers.Add(new NotificationUser
                {
                    UserId = notification.Item?.CreatedById,
                    NotificationId = notification.Id
                });
            }
            else if (type == "ItemGroup")
            {
                _context.NotificationUsers.Add(new NotificationUser
                {
                    UserId = notification.ItemGroup?.CreatedById,
                    NotificationId = notification.Id
                });
            }
            await _context.SaveChangesAsync();
        }

        public List<Notification> GetUserNotifications(string userId)
        {
            var model =  _context.NotificationUsers.Where(u => u.UserId.Equals(userId) && !u.IsRead)
                                            .Include(nu => nu.Notification)
                                            .Select(n=>n.Notification)
                                            //So each user sees the most recent notification first
                                            .OrderByDescending(dc => dc.DateCreated)
                                            .ToList();
            return model;
        }

        public void ReadNotification(int notificationId, string userId)
        {
            var notification = _context.NotificationUsers
                                       .FirstOrDefault(n => n.UserId.Equals(userId)
                                       && n.NotificationId == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.NotificationUsers.Update(notification);
                _context.SaveChanges();
            }
        }

        public async Task SendCreateItemNotifications(string itemId, User user)
        {
            Item item = await _itemRepository.GetItemByIdAsync(itemId);
            var notification = new Notification
            {
                Text = $"A new item '{item.Name}' with Tag Number: {item.TagNo} for {item.ItemGroup?.Name} has been added by {user.Surname + " " + user.OtherNames}.",
                NotificationTriggerId = user.Id,
                Item = item,
                NotificationType = NotificationType.ToApproveItem
            };

            //save notification
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();



            //add admin users that belong to the department of the creator of the item
            //so that they can be notified of the item creation
            //Also send Email
            await AddCreateNotificationUsers(notification, user.DepartmentId);
            
            //send notification
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }
        
        public async Task SendCreateItemGroupNotifications(string itemGroupId, User user)
        {
            ItemGroup itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            var notification = new Notification
            {
                Text = $"A new ItemGroup {itemGroup?.Name} with TagNo: {itemGroup?.TagNo} has been added by {user.Surname + " " + user.OtherNames}.",
                NotificationTriggerId = user.Id,
                ItemGroup = itemGroup,
                NotificationType = NotificationType.ToApproveItemGroup
            };

            //save notification
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();


            //add admin users that belong to the department of the creator of the item
            //so that they can be notified of the item creation
            //Also send Email
            await AddCreateNotificationUsers(notification, user.DepartmentId);
            
            //send notification
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }

        private void SendCreateNotificationEmail(User user, Notification notification)
        {
            var mail = new MailLog();
            mail.RecipientName = user.OtherNames;
            mail.Receiver = user.Email;
            
            if (notification.ServiceId != null)
            {
                mail.Subject = "A SERVICE CREATED IS PENDING YOUR REVIEW AND APPROVAL";
                var approvalEnd = " Kindly login to the E-Tracker App to view and approve this service";
                mail.MessageBody = notification.Text + approvalEnd;
            }
            else if (notification.ItemId != null)
            {
                mail.Subject = "AN ITEM CREATED IS PENDING YOUR REVIEW AND APPROVAL";
                var approvalEnd = " Kindly login to the E-Tracker App to view and approve this item";
                mail.MessageBody = notification.Text + approvalEnd;
            }
            else if (notification.ItemGroupId != null)
            {
                mail.Subject = "AN ITEM GROUP CREATED IS PENDING YOUR REVIEW AND APPROVAL";
                var approvalEnd = " Kindly login to the E-Tracker App to view and approve this item group";
                mail.MessageBody = notification.Text + approvalEnd;
            }
            BackgroundJob.Enqueue(() => _emailRepository.SendMailAsync(mail));
        }
        
        public async Task SendApproveItemNotifications(string itemId, User user)
        {
            Item item = await _itemRepository.GetItemByIdAsync(itemId);
            var notification = new Notification
            {
                Text = $"Item '{item?.Name}' with Tag Number: {item?.TagNo} for {item.ItemGroup?.Name} has been approved by {user.Surname + " " + user.OtherNames}.",
                NotificationTriggerId = user.Id,
                ItemId = itemId,
                Item = item,
                NotificationType = NotificationType.ItemApproved
            };

            //save notification
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await SendApprovalNotificationEmail(notification, "Item");

            //add users to be notified of the approval of the item
            await AddApprovalNotificationUsers(notification, "Item");

            //send notification
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }
        
        public async Task SendApproveItemGroupNotifications(string itemGroupId, User user)
        {
            ItemGroup itemGroup = await _itemGroupRepository.GetItemGroupByIdAsync(itemGroupId);
            var notification = new Notification
            {
                Text = $"ItemGroup {itemGroup?.Name} with TagNo: {itemGroup?.TagNo} has been approved by {user.Surname + " " + user.OtherNames}.",
                NotificationTriggerId = user.Id,
                ItemGroupId = itemGroupId,
                ItemGroup = itemGroup,
                NotificationType = NotificationType.ItemGroupApproved
            };

            //save notification
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await SendApprovalNotificationEmail(notification, "ItemGroup");

            //add users to be notified of the approval of the item
            await AddApprovalNotificationUsers(notification, "ItemGroup");

            //send notification
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }

        private async Task SendApprovalNotificationEmail(Notification notification, string type)
        {
            var itemCreatorId = type == "Service"? notification.Service?.CreatedById:
                                type == "Item"? notification.Item?.CreatedById :
                                type == "ItemGroup" ? notification.ItemGroup?.CreatedById : null;
            var itemCreator = await _userManager.FindByIdAsync(itemCreatorId);
            if (itemCreator == null)
            {
                _logger.LogInformation($"-------- itemCreator is null at SendApprovalNotificationEmail for" +
                    $"{notification?.Text} and type: {type} -------------");

                _logger.LogInformation($"--------- Skipping ApprovalNotificationEmail sending for {notification?.Text}-----------");
                return;
            }
            //send email notification to the user that created the Item
            var mail = new MailLog();
            mail.RecipientName = itemCreator?.OtherNames;
            mail.Receiver = itemCreator?.Email;
            mail.MessageBody = notification.Text;
            mail.Subject = type == "Service"? "A SERVICE YOU CREATED HAS BEEN APPROVED" : 
                            type == "Item" ? "AN ITEM YOU CREATED HAS BEEN APPROVED" :
                             type == "ItemGroup" ? "AN ITEM Group YOU CREATED HAS BEEN APPROVED": "";
            BackgroundJob.Enqueue(() => _emailRepository.SendMailAsync(mail));
        }

        public async Task SendCreateServiceNotifications(string itemId, string serviceId, User user)
        {
            Service service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            Item item = await _itemRepository.GetItemByIdAsync(itemId);
            var notification = new Notification
            {
                NotificationTriggerId = user.Id,
                Item = item,
                Service = service,
                ServiceId = serviceId,
                NotificationType = NotificationType.ToApproveService
            };
            if (item?.ItemType?.Name == "Renewal")
            {
                notification.Text = $"Item '{item?.Name}' with Tag Number: {item?.TagNo} for {item?.ItemGroup?.Name} has been renewed by {user?.Surname + " " + user?.OtherNames}.";
            }
            else if (item?.ItemType?.Name == "Servicing")
            {
                notification.Text = $"Item '{item?.Name}' with Tag Number: {item?.TagNo} for {item?.ItemGroup?.Name} has been serviced by {user?.Surname + " " + user?.OtherNames}.";

            }
            //save notification
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();


            //add admin users that belong to the department of the creator of the Service
            //so that they can be notified of the service creation
            await AddCreateNotificationUsers(notification, user.DepartmentId);

            //send notification
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }

        public async Task SendApproveServiceNotifications(string itemId, string serviceId, User user)
        {
            Service service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            Item item = await _itemRepository.GetItemByIdAsync(itemId);
            var notification = new Notification
            {
                NotificationTriggerId = user.Id,
                ItemId = itemId,
                Item = item,
                Service = service,
                ServiceId = serviceId,
                NotificationType = NotificationType.ItemApproved
            };

            if (item?.ItemType?.Name == "Renewal")
            {
                notification.Text = $"Renewal of item '{item?.Name}' with Tag Number: {item?.TagNo} for {item.ItemGroup?.Name} has been approved by {user?.Surname + " " + user?.OtherNames}.";
            }
            else if (item?.ItemType?.Name == "Servicing")
            {
                notification.Text = $"Servicing of item '{item?.Name}' with Tag Number: {item?.TagNo} for {item.ItemGroup?.Name} has been approved by {user?.Surname + " " + user?.OtherNames}.";
            }

            //save notification
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            //send mail to users to be notified of the approval of the item
            await SendApprovalNotificationEmail(notification, "Service");

            //add users to be notified of the approval of the item
            await AddApprovalNotificationUsers(notification, "Service");

            //send notification
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }
    }
}