using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMail.Models;
using WebMail.Data;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using WebMail.Config;
using Microsoft.Extensions.Configuration;
using MailKit;
using System.Text;

namespace WebMail.Services
{
    /// <summary>
    /// Service for sending a message to the mails
    /// </summary>
    public class MailService : IMailService
    {
        private readonly MailConfig _config;
        public MailService(IOptions<MailConfig> mailConfig)
        {
            _config = mailConfig.Value;
        }
        /// <summary>
        /// Publishing message to mails 
        /// </summary>
        /// <param name="mailBody">Body request. Format is json</param>
        /// <returns></returns>
        public async Task<Mail> PublishMessage(MailBody mailBody)
        {
            Mail mail = new Mail()
            {
                Subject = mailBody.Subject,
                Body = mailBody.Body,
                Result = "OK",
                FailedMessage = "",
                Date = DateTime.Now,
                Recipients = mailBody.Recipients.Select(recipient => new Recipient()
                {
                    RecipientEmail = recipient
                }).ToList()
            };
            List<MimeMessage> messages = new List<MimeMessage>();
            foreach (var recipient in mailBody.Recipients)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(mailBody.Subject, _config.Email));
                emailMessage.To.Add(new MailboxAddress("", recipient));
                emailMessage.Subject = mailBody.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = mailBody.Body
                };
                messages.Add(emailMessage);
            }
            foreach (var message in messages)
            {
                try
                {
                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(_config.Host, _config.Port, _config.UseSsl);
                        await client.AuthenticateAsync(_config.Email, _config.Password);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                    var failedmessage = ProcessDeliveryStatusNotification(message);
                    if (failedmessage != null)
                    {
                        throw new Exception(failedmessage);
                    }

                }
                catch (Exception ex)
                {
                    mail.FailedMessage += $"Failed: {message.To}:{ex}\n";
                    mail.Result = "Failed";
                }
            }
            return mail;
        }

        /// <summary>
        /// Checking the status of sending a message
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Status of sending</returns>
        private string ProcessDeliveryStatusNotification(MimeMessage message)
        {
            var report = message.Body as MultipartReport;

            if (report == null || report.ReportType == null || !report.ReportType.Equals("delivery-status", StringComparison.OrdinalIgnoreCase))
            {

                return null;
            }

            // process the report
            foreach (var mds in report.OfType<MessageDeliveryStatus>())
            {
                // process the status groups - each status group represents a different recipient

                // The first status group contains information about the message
                var envelopeId = mds.StatusGroups[0]["Original-Envelope-Id"];

                // all of the other status groups contain per-recipient information
                for (int i = 1; i < mds.StatusGroups.Count; i++)
                {
                    var recipient = mds.StatusGroups[i]["Original-Recipient"];
                    var action = mds.StatusGroups[i]["Action"];

                    if (recipient == null)
                        recipient = mds.StatusGroups[i]["Final-Recipient"];

                    // the recipient string should be in the form: "rfc822;user@domain.com"
                    var index = recipient.IndexOf(';');
                    var address = recipient.Substring(index + 1);

                    switch (action)
                    {
                        case "failed":
                            return $"Delivery of message {envelopeId} failed for { address}";

                        case "delayed":
                            return $"Delivery of message {envelopeId} delayed for { address}";

                        case "delivered":
                            return $"Delivery of message {envelopeId} delivered for { address}";
                        case "relayed":
                            return $"Delivery of message {envelopeId} relayed for { address}";
                        case "expanded":
                            return $"Delivery of message {envelopeId} expanded for { address}";
                    }
                }
            }
            return null;
        }

    }
}
