using E_Tracker.Authorization;
using E_Tracker.Data;
using E_Tracker.Email;
using E_Tracker.Repository.EmailRepo;
using E_Tracker.Repository.ItemRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.ReminderRepo
{
    public class ReminderService : IReminderService
    {
        private readonly IItemRepository itemRepository;
        private readonly ILogger<ReminderService> logger;
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;
        private readonly INotificationService notificationService;
        private readonly IEmailRepository emailRepository;

        public ReminderService(IItemRepository itemRepository, ApplicationDbContext context,
            UserManager<User> userManager, ILogger<ReminderService> logger, 
            INotificationService notificationService, IEmailRepository emailRepository)
        {
            this.itemRepository = itemRepository;
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
            this.notificationService = notificationService;
            this.emailRepository = emailRepository;
        }

        public async Task SendReminderMail()
        {
            logger.LogInformation($"---- Reminder mail Background invoked at {DateTime.Now.ToString()}-----");
            var reminders = GetUnSentReminders();
            foreach (var reminder in reminders)
            {
                if (reminder.DateOfReminder.Date == DateTime.Now.Date && reminder.DateOfReminder.Hour == DateTime.Now.Hour)
                {
                    var mail = new MailLog
                    {
                        //logic to determine email receiver
                        //find a way to check if the email is a valid email
                        Receiver = reminder.ReceipentEmail,

                        RecipientName = reminder.ReceipentEmail,
                        Subject = ItemExpiryReminderEmail.Subject(reminder.Item),
                        MessageBody = reminder.Message
                    };
                    context.MailLogs.Add(mail);
                    reminder.IsSent = true;
                    context.Reminders.Update(reminder);
                    context.SaveChanges();
                }
            }
            await emailRepository.SendMailAsync();
            logger.LogInformation($"---- Reminder mail Background finished at {DateTime.Now.ToString()}-----");
        }

        public async Task SendExpiredItemMail() 
        {
            var expiredItems = await itemRepository.GetAllExpiredItemsAsync();
            foreach (var expiredItem in expiredItems)
            {
                var itemCreator = await userManager.FindByIdAsync(expiredItem.CreatedById);
                itemCreator = itemCreator ?? new User();
                logger.LogInformation($"Sending expiry email to {itemCreator}");
                logger.LogInformation($"Sending expiry email to {itemCreator?.Email}");
                var mail = new MailLog
                {
                    //logic to determine email receiver
                    Receiver = itemCreator.Email,
                    RecipientName = itemCreator.OtherNames,
                    Subject = ItemExpiryReminderEmail.ExpiredItemSubject(expiredItem),
                    MessageBody = ItemExpiryReminderEmail.ExpiredItemMessageBody(expiredItem)
                };
                context.MailLogs.Add(mail);

                //After three days, if the item has not been serviced, notify his HOD
                if ((DateTime.Now.Date - expiredItem.ExpiredDate.Date).TotalDays > 3)
                {
                    var adminUsers = await userManager.GetUsersInRoleAsync(RolesList.Admin);
                    foreach (var adminUser in adminUsers.Where(x => x.DepartmentId == itemCreator.DepartmentId))
                    {
                        logger.LogInformation($"Sending expiry email to AdminUser: {adminUser} of {itemCreator}");
                        logger.LogInformation($"Sending expiry email to {adminUser?.Email}");
                        var adminMail = new MailLog
                        {
                            //logic to determine email receiver
                            Receiver = adminUser.Email,
                            RecipientName = adminUser.OtherNames,
                            Subject = ItemExpiryReminderEmail.ExpiredItemSubject(expiredItem),
                            MessageBody = ItemExpiryReminderEmail.ExpiredItemMessageBody(expiredItem)
                        };
                        context.MailLogs.Add(mail);
                    }
                }
                context.SaveChanges();
            }
            await emailRepository.SendMailAsync();
        }

        private async Task SendReminderEMail()
        {
            var items = await itemRepository.GetAllItemsAsync();
            foreach (var item in items)
            {
                string message = "";
                var todayDate = DateTime.Now.Date;
                //One day to expirydate
                if (item.ExpiredDate.AddDays(-1).Date == todayDate.Date)
                {
                    message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire in 1 day on {item.ExpiredDate.ToShortDateString()}";
                }
                //One Week to expirydate
                if (item.ExpiredDate.AddDays(-7).Date == todayDate.Date)
                {
                    message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire in 1 week on {item.ExpiredDate.ToShortDateString()}";
                }
                //Two weeks to expirydate
                if (item.ExpiredDate.AddDays(-14).Date == todayDate.Date)
                {
                    message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire in 2 weeks on {item.ExpiredDate.ToShortDateString()}";
                }
                //One month to expirydate
                if (item.ExpiredDate.AddMonths(-1).Date == todayDate.Date)
                {
                    message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire in 1 month on {item.ExpiredDate.ToShortDateString()}";
                }
                if (!string.IsNullOrEmpty(message))
                {
                    var mail = new MailLog
                    {
                        //logic to determine email receiver
                        Receiver = "onidavid97@gmail.com",
                        RecipientName = "onidavid97@gmail.com",
                        Subject = $"ITEM WITH NAME: {item.Name} AND TAGNO: {item.TagNo} WOULD SOON EXPIRE",
                        MessageBody = message
                    };
                    context.MailLogs.Add(mail);
                    context.SaveChanges();
                }
            }
            await emailRepository.SendMailAsync();
        }

        public async Task UpdateReminderLog(Item item, string itemCreatorEmail)
        {
            var itemReminders = await context.Reminders.Where( rem => rem.ItemId == item.Id).ToListAsync();
            context.Reminders.RemoveRange(itemReminders);
            await context.SaveChangesAsync();
            await SetReminderLog(item, itemCreatorEmail);
        }
        
        public async Task DeleteReminderLog(string itemId)
        {
            var itemReminders = await context.Reminders.Where( rem => rem.ItemId == itemId).ToListAsync();
            context.Reminders.RemoveRange(itemReminders);
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteReminderLog(Item item)
        {
            var itemReminders = await context.Reminders.Where( rem => rem.ItemId == item.Id).ToListAsync();
            context.Reminders.RemoveRange(itemReminders);
            await context.SaveChangesAsync();
        }

        public async Task SetReminderLog(Item item, string itemCreatorEmail)
        {
            logger.LogInformation($"---- About to set reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} created by {itemCreatorEmail}-----");
            logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            var notificationPeriod = item.NotificationPeriod.ToString();
            if (string.Equals(notificationPeriod,"Day",StringComparison.OrdinalIgnoreCase))
            {
                await AddDayReminder(item, itemCreatorEmail);
            }
            else if (string.Equals(notificationPeriod, "Week", StringComparison.OrdinalIgnoreCase))
            {
                await AddWeekReminder(item, itemCreatorEmail);
            }
            else if (string.Equals(notificationPeriod, "Month", StringComparison.OrdinalIgnoreCase))
            {
                await AddMonthReminder(item, itemCreatorEmail);
            }
            else if (string.Equals(notificationPeriod, "Year", StringComparison.OrdinalIgnoreCase))
            {
                await AddYearReminder(item, itemCreatorEmail);
            }
        }

        public async Task EscalateItemReminder(Item item, string itemCreatorEmail, string itemReminderEscalatorId)
        {
            var notificationFrequency = item.NotificationFrequency.ToString();
            var expiryDate = item.ExpiredDate;

                Reminder reminder = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    IsEscalation = true,
                    EscalatedById = itemReminderEscalatorId,
                    Message = $"Item with Name: {item.Name} and TagNo: {item.TagNo} has expired on {expiryDate.ToShortDateString()}, Kindly take necessary actions"
                };
                reminder.DateOfReminder = DateTime.Now;
                await context.Reminders.AddAsync(reminder);
            await context.SaveChangesAsync();
            await SendEscalationEmail(reminder);
        }

        //Refactor this to only send esca;ation Reminder and not all unsent reminders
        private async Task SendEscalationEmail(Reminder escalatedReminder)
        {
            logger.LogInformation($"---- Reminder mail escalation invoked at {DateTime.Now.ToString()}-----");
            var reminders = GetUnSentReminders();
            foreach (var reminder in reminders)
            {
                if (reminder.DateOfReminder.Date == DateTime.Now.Date && reminder.DateOfReminder.Hour == DateTime.Now.Hour)
                {
                    var mail = new MailLog
                    {
                        //logic to determine email receiver
                        //find a way to check if the email is a valid email
                        Receiver = reminder.ReceipentEmail,
                        
                        RecipientName = reminder.ReceipentEmail,

                        Subject = $"ITEM WITH NAME: {reminder.Item.Name} AND TAGNO: {reminder.Item.TagNo} HAS EXPIRED",
                        MessageBody = reminder.Message
                    };
                    context.MailLogs.Add(mail);
                    reminder.IsSent = true;
                    context.Reminders.Update(reminder);
                    context.SaveChanges();
                }
            }
            await emailRepository.SendMailAsync();
            logger.LogInformation($"---- Reminder mail escalation finished at {DateTime.Now.ToString()}-----");
        }

        private async Task AddDayReminder(Item item, string itemCreatorEmail)
        {
            var notificationFrequency = item.NotificationFrequency.ToString();
            var expiryDate = item.ExpiredDate;
            
            if (string.Equals(notificationFrequency, "Once", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminder = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = ItemExpiryReminderEmail.MessageBody(item)
                };
                reminder.DateOfReminder = expiryDate.Date.AddDays(-1).AddHours(8);
                await context.Reminders.AddAsync(reminder);
                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for Day added for Once-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Twice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = ItemExpiryReminderEmail.MessageBody(item)
                };
                reminderOne.DateOfReminder = expiryDate.Date.AddDays(-1).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for Day added for Once-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
                
                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderTwo.DateOfReminder = expiryDate.Date.AddDays(-1).AddHours(12);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for Day added for Twice-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Thrice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderOne.DateOfReminder = expiryDate.Date.AddDays(-1).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for Day added for Once-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderTwo.DateOfReminder = expiryDate.Date.AddDays(-1).AddHours(12);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for Day added for Twice-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderThree = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderThree.DateOfReminder = expiryDate.Date.AddDays(-1).AddHours(16);
                await context.Reminders.AddAsync(reminderThree);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for Day added for Thrice-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            await context.SaveChangesAsync();
            
            logger.LogInformation($"---- Reminder log changes saved succesfully for Item: {item?.Name} and TagNo: {item?.TagNo} for Day-----");
            logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
        }

        private async Task AddWeekReminder(Item item, string itemCreatorEmail)
        {
            var notificationFrequency = item.NotificationFrequency.ToString();
            var expiryDate = item.ExpiredDate;
           
            if (string.Equals(notificationFrequency, "Once", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminder = new Reminder
                {
                    ItemId = item?.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item?.Name} and TagNo: {item?.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminder.DateOfReminder = expiryDate.Date.AddDays(-7).AddHours(8);
                await context.Reminders.AddAsync(reminder);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK added for Once-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Twice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderOne.DateOfReminder = expiryDate.Date.AddDays(-7).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderTwo.DateOfReminder = expiryDate.Date.AddDays(-5).AddHours(8);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK added for TWICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Thrice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderOne.DateOfReminder = expiryDate.Date.AddDays(-7).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderTwo.DateOfReminder = expiryDate.Date.AddDays(-5).AddHours(8);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK added for TWICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderThree = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderThree.DateOfReminder = expiryDate.Date.AddDays(-2).AddHours(8);
                await context.Reminders.AddAsync(reminderThree);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK added for THRICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            
            await context.SaveChangesAsync();

            logger.LogInformation($"---- Reminder log changes saved succesfully for Item: {item?.Name} and TagNo: {item?.TagNo} for WEEK-----");
            logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
        }

        private async Task AddMonthReminder(Item item, string itemCreatorEmail)
        {
            var notificationFrequency = item.NotificationFrequency.ToString();
            var expiryDate = item.ExpiredDate;
           
            if (string.Equals(notificationFrequency, "Once", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminder = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminder.DateOfReminder = expiryDate.Date.AddMonths(-1).AddDays(7).AddHours(8);
                await context.Reminders.AddAsync(reminder);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Twice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderOne.DateOfReminder = expiryDate.Date.AddMonths(-1).AddDays(3).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderTwo.DateOfReminder = expiryDate.Date.AddMonths(-1).AddDays(15).AddHours(12);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH added for TWICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Thrice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderOne.DateOfReminder = expiryDate.Date.AddMonths(-1).AddDays(3).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderTwo.DateOfReminder = expiryDate.Date.AddMonths(-1).AddDays(15).AddHours(12);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH added for TWICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderThree = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                reminderThree.DateOfReminder = expiryDate.Date.AddMonths(-1).AddDays(25).AddHours(16);
                await context.Reminders.AddAsync(reminderThree);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH added for THRICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            await context.SaveChangesAsync();

            logger.LogInformation($"---- Reminder log changes saved succesfully for Item: {item?.Name} and TagNo: {item?.TagNo} for MONTH-----");
            logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
        }

        private async Task AddYearReminder(Item item, string itemCreatorEmail)
        {
            var notificationFrequency = item.NotificationFrequency.ToString();
            var expiryDate = item.ExpiredDate;
           
            if (string.Equals(notificationFrequency, "Once", StringComparison.OrdinalIgnoreCase))
            {
                //TimeSpan timeSpan = expiryDate.Date - DateTime.Now;
                Reminder reminder = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                //1 year to the expiry date
                reminder.DateOfReminder = expiryDate.Date.AddYears(-1).AddMonths(11).AddDays(1).AddHours(8);
                await context.Reminders.AddAsync(reminder);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for YEAR added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Twice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                //6 months to the expiry date
                reminderOne.DateOfReminder = expiryDate.Date.AddYears(-1).AddMonths(6).AddDays(1).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for YEAR added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                //1 month to the expiry date
                reminderTwo.DateOfReminder = expiryDate.Date.AddYears(-1).AddMonths(11).AddDays(1).AddHours(8);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for YEAR added for TWICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            else if (string.Equals(notificationFrequency, "Thrice", StringComparison.OrdinalIgnoreCase))
            {
                Reminder reminderOne = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                //1 year to the expiry date
                reminderOne.DateOfReminder = expiryDate.Date.AddYears(-1).AddDays(1).AddHours(8);
                await context.Reminders.AddAsync(reminderOne);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for YEAR added for ONCE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderTwo = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                //6 months to the expiry date
                reminderTwo.DateOfReminder = expiryDate.Date.AddYears(-1).AddMonths(6).AddDays(1).AddHours(8);
                await context.Reminders.AddAsync(reminderTwo);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for YEAR added for TWICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");

                Reminder reminderThree = new Reminder
                {
                    ItemId = item.Id,
                    ReceipentEmail = itemCreatorEmail,
                    Message = $"Kindly, take note that Item with Name: {item.Name} and TagNo: {item.TagNo} would expire on {expiryDate.ToShortDateString()}"
                };
                //1 month to the expiry date
                reminderThree.DateOfReminder = expiryDate.Date.AddYears(-1).AddMonths(11).AddDays(1).AddHours(8);
                await context.Reminders.AddAsync(reminderThree);

                logger.LogInformation($"---- Reminder log for Item: {item?.Name} and TagNo: {item?.TagNo} for YEAR added for THRICE-----");
                logger.LogInformation($"---- Item notification period: {item?.NotificationPeriod} and Notification Frequency: {item?.NotificationFrequency} -----");
            }
            await context.SaveChangesAsync();

        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            var reminders = await context.Reminders.ToListAsync();
            return reminders;
        }

        public IEnumerable<Reminder> GetUnSentReminders()
        {
            var reminders = context.Reminders.Include(rem => rem.Item).Where(rem => !rem.IsSent).ToList();
            return reminders;
        }
    }
}
