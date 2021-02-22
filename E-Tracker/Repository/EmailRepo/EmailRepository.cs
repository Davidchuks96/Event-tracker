using AutoMapper.Configuration;
using E_Tracker.Data;
using E_Tracker.Email;
using E_Tracker.Repository.EmailLogRepo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace E_Tracker.Repository.EmailRepo
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailRepository> _logger;
        private readonly IEmailSentLogRepository _emailSentLogRepo;
        private readonly SendGridSettings _sendGridSettings;

        public EmailRepository(ILogger<EmailRepository> logger,
           IOptions<EmailSettings> emailSettings, IEmailSentLogRepository emailSentLogRepo, IOptions<SendGridSettings> sendGridSettings)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _sendGridSettings = sendGridSettings.Value;
            _emailSentLogRepo = emailSentLogRepo;
        }



        public async Task<bool> SendMailAsync(MailLog mailLog = null)
        {
            if (mailLog != null)
            {
                await _emailSentLogRepo.LogEmailAsync(mailLog);
            }
            _logger.LogInformation("---- Mail Background invoked -----");
            return await RunMailBackgroundService();

        }


        private async Task<bool> RunMailBackgroundService()
        {
            if (_emailSettings == null)
            {
                _logger.LogError("---- Email settings is null -----");
            }
            _logger.LogInformation($"---- Email settings Address: {_emailSettings.Sender} -----");
            var pendingMail = await _emailSentLogRepo.GetPendingEmailLogAsync();
            try
            {
                if (pendingMail.Any())
                {
                    foreach (var mailLog in pendingMail)
                    {
                        if (mailLog.Receiver != null)
                        {
                            _logger.LogInformation($"---- Attempting to send an email to {mailLog.Receiver} -----");
                            string body = "";

                            if (mailLog.Subject.Contains(ConfirmEmail.Subject, StringComparison.OrdinalIgnoreCase))
                            {
                                //Read confirmEmail mail template
                                StreamReader reader = new StreamReader("Views\\Shared\\ConfirmEmailTemplate.cshtml");
                                body = reader.ReadToEnd();
                                body = body.Replace("[ConfirmEmailLink]", mailLog.MessageBody);
                            }
                            else
                            {
                                //Read the regular Email template
                                StreamReader reader = new StreamReader("Views\\Shared\\EmailTemplate.cshtml");
                                body = reader.ReadToEnd();
                                body = body.Replace("[Body]", mailLog.MessageBody);
                            }
                            body = body.Replace("[Subject]", mailLog.Subject);
                            body = body.Replace("[RecipientName]", mailLog.RecipientName);
                            body = body.Replace("[Year]", DateTime.Now.Year.ToString());
                            //var success = await SendEmailAsync(mailLog.Receiver, mailLog.Subject, body);
                            var client = new SendGridClient(_sendGridSettings.ApiKey);
                            var from = new EmailAddress(_sendGridSettings.From, _sendGridSettings.FromName);
                            var subject = mailLog.Subject;
                            var to = new EmailAddress(mailLog.Receiver);
                            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", body);
                            var response = await client.SendEmailAsync(msg);
                            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                            {
                                mailLog.IsSent = true;
                                _logger.LogInformation($"---- mail has been successfully sent to {mailLog.Receiver} -----");
                            }
                            await _emailSentLogRepo.UpdateEmailAsync(mailLog);

                        }
                    }

                }
                //No exceptions
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"The source {ex?.Source} " +
                $"threw an exception {ex.ToString()}");

                //An exception was thrown
                _logger.LogError(ex, ex.Message);

                return false;
            }


        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message, string attachedfiles = null)
        {
            _logger.LogInformation($"---- Email settings Address: {_emailSettings.Sender} -----");
            try
            {
                if (_emailSettings == null)
                {
                    _logger.LogError("_emailSettings == null");
                }
                _logger.LogInformation($"_emailSettings sender-{_emailSettings.Sender}");
                MailMessage mm = new MailMessage();

                foreach (string n in email.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(n)) mm.To.Add(new MailAddress(n.Trim()));
                }
                mm.Sender = new MailAddress(_emailSettings.SenderName);
                mm.From = new MailAddress(_emailSettings.SenderName,"E-Tracker");
                mm.Subject = subject;
                mm.Body = message;


                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                //attachements
                if (!string.IsNullOrWhiteSpace(attachedfiles))
                {
                    foreach (string attachm in attachedfiles.Split(';'))
                    {
                        //if (!string.IsNullOrEmpty(attachm)) mm.Attachments.Add(new Attachment(attachm));
                    }
                }



                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                using (var client = new SmtpClient())
                {

                    client.Port = _emailSettings.MailPort;
                    client.Host = _emailSettings.MailServer;
                    client.EnableSsl = _emailSettings.SSl;   //true
                    client.Timeout = 20000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                    // Note: only needed if the SMTP server requires authentication
                    //await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
                    try
                    {
                        await Task.Run(() =>
                           client.Send(mm)
                        );
                        _logger.LogInformation($"---- Email sent successfully -----");

                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                //Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                //System.Threading.Thread.Sleep(5000);
                                client.Send(mm);

                                _logger.LogInformation($"---- Email resent successfully -----");
                                //Email resent successfully
                                return true;
                            }
                            else
                            {
                                _logger.LogError("Failed to deliver message to {0}",
                                   ex?.InnerExceptions[i]?.FailedRecipient);
                                _logger.LogError(ex, ex.Message);

                                return false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex?.StackTrace);
                // TODO: handle exception
                _logger.LogError(ex.StackTrace);
                throw new InvalidOperationException(ex.Message);
            }
            //Email sent successfully
            return true;
        }

        public async Task SendEmailAsync(string email, string CC, string subject, string message, string attachedfiles)
        {
            try
            {


                MailMessage mm = new MailMessage();

                foreach (string n in email.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(n)) mm.To.Add(new MailAddress(n.Trim()));
                }
                mm.Sender = new MailAddress(_emailSettings.SenderName);
                mm.From = new MailAddress(_emailSettings.SenderName);
                mm.Subject = subject;
                mm.Body = message;
                if (!string.IsNullOrWhiteSpace(CC))
                {
                    foreach (string c in CC.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(c)) mm.CC.Add(new MailAddress(c.Trim()));
                    }
                }

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                //attachements
                if (!string.IsNullOrWhiteSpace(attachedfiles))
                {
                    foreach (string attachm in attachedfiles.Split(';'))
                    {
                       // if (!string.IsNullOrEmpty(attachm)) mm.Attachments.Add(new Attachment(attachm));
                    }
                }



                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;




                MailLog emailLog = new MailLog
                {
                    MessageBody = message,
                    Receiver = email
                };


                using (var client = new SmtpClient())
                {

                    client.Port = _emailSettings.MailPort;
                    client.Host = _emailSettings.MailServer;
                    client.EnableSsl = _emailSettings.SSl;   //true
                    client.Timeout = 20000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_emailSettings.Sender, _emailSettings.Password);


                    // Note: only needed if the SMTP server requires authentication
                    //await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
                    try
                    {
                        await Task.Run(() =>
                           client.Send(mm)
                        );
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                //Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                //System.Threading.Thread.Sleep(5000);
                                client.Send(mm);
                            }
                            else
                            {
                                //Console.WriteLine("Failed to deliver message to {0}",
                                //    ex.InnerExceptions[i].FailedRecipient);

                                emailLog.IsSent = false;
                            }
                        }
                    }

                    await _emailSentLogRepo.LogEmailAsync(emailLog);
                    // await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
        public async Task SendEmailTransactionAsync(string email, string subject, string message, string attachedfiles)
        {
            try
            {
                //var mimeMessage = new MimeMessage();

                //mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
                //mimeMessage.To.Add(new MailboxAddress(email));
                //mimeMessage.Subject = subject;

                //mimeMessage.Body = new TextPart("html")
                //{
                //    Text = message
                //};
                //var _emailSettings =  _config.GetSection("EmailSettings").Value;

                //IOptions<EmailSettings> _emailSettings = new IOptions<EmailSettings>(_config.GetSection("EmailSettings"));

                // string fileUrl = _config.GetSection("GoSmarticleEndPoint:filesUrl").Value;

                MailMessage mm = new MailMessage();

                foreach (string n in email.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(n)) mm.To.Add(new MailAddress(n.Trim()));
                }
                mm.Sender = new MailAddress(_emailSettings.SenderName);
                mm.From = new MailAddress(_emailSettings.Sender);
                mm.Subject = subject;
                mm.Body = message;

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                //attachements
                if (!string.IsNullOrWhiteSpace(attachedfiles))
                {
                    foreach (string attachm in attachedfiles.Split(';'))
                    {
                       // if (!string.IsNullOrEmpty(attachm)) mm.Attachments.Add(new Attachment(attachm));
                    }
                }

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                MailLog emailLog = new MailLog
                {
                    MessageBody = message,
                    Receiver = email
                };


                using (var client = new SmtpClient())
                {

                    client.Port = _emailSettings.MailPort;
                    client.Host = _emailSettings.MailServer;
                    client.EnableSsl = _emailSettings.SSl;   //true
                    client.Timeout = 20000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_emailSettings.Sender, _emailSettings.Password);


                    // Note: only needed if the SMTP server requires authentication
                    //await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
                    try
                    {
                        await Task.Run(() =>
                           client.Send(mm)
                        );
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                //Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                //System.Threading.Thread.Sleep(5000);
                                client.Send(mm);
                            }
                            else
                            {
                                //Console.WriteLine("Failed to deliver message to {0}",
                                //    ex.InnerExceptions[i].FailedRecipient);

                                emailLog.IsSent = false;
                            }
                        }
                    }

                    await _emailSentLogRepo.LogEmailTransactionAsync(emailLog);
                    // await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
