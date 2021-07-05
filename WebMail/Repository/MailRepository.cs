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

namespace WebMail.Repository
{
    /// <summary>
    /// A class for managing the context of a message database
    /// </summary>
    public class MailRepository : IRepository<Mail>
    {
        private readonly WebMailDbContext _context;

        /// <summary>
        /// Property of the list of all messages
        /// </summary>
        public IEnumerable<Mail> All => _context.Mail.Include(x => x.Recipients).ToList();

        private readonly IOptions<MailConfig> _mailConfig;

        /// <summary>
        /// Property returns the MailConfig object containing the config parameters
        /// </summary>
        public MailConfig config => _mailConfig.Value;

        /// <summary>
        /// A constructor that accepts the database context and config parameters as input
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mailConfig"></param>
        public MailRepository(WebMailDbContext context, IOptions<MailConfig> mailConfig)
        {
            _context = context;
            _mailConfig = mailConfig;
        }
        /// <summary>
        /// Adds the Mail entity to the database
        /// </summary>
        /// <param name="entity">The Mail entity</param>
        /// <returns>Returns the Mail object</returns>
        public Mail Add(Mail entity)
        {
            _context.Mail.Add(entity);
            _context.SaveChanges();
            return entity;
        }
        /// <summary>
        /// Sends a message via mail and adds information about the message to the database
        /// </summary>
        /// <param name="jsMail">the body of the post request in Json format</param>
        /// <returns></returns>
        public async Task<Mail> Add(JsonMail jsMail)
        {
            Mail mail = new Mail()
            {
                Subject = jsMail.Subject,
                Body = jsMail.Body,
                Result = "OK",
                FailedMessage = "",
                Date = DateTime.Now,
                Recipients = jsMail.Recipients.Select(recipient => new Recipient()
                {
                    RecipientEmail = recipient
                }).ToList()
            };
            List<MimeMessage> messages = new List<MimeMessage>();
            foreach(var recipient in jsMail.Recipients)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(jsMail.Subject, config.Email));
                emailMessage.To.Add(new MailboxAddress("", recipient));
                emailMessage.Subject = jsMail.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = jsMail.Body
                };
                messages.Add(emailMessage);
            }
            foreach(var message in messages)
            {
                try
                {
                    using(var client = new SmtpClient())
                    {
                        await client.ConnectAsync(config.Host, config.Port, config.UseSsl);
                        await client.AuthenticateAsync(config.Email, config.Password);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                    var failedmessage = ProcessDeliveryStatusNotification(message);
                    if(failedmessage!=null)
                    {
                        throw new Exception(failedmessage);
                    }

                }
                catch(Exception ex)
                {
                    mail.FailedMessage += $"Failed: {message.To.ToString()}:{ex}\n";
                    mail.Result = "Failed";
                }
            }
             
            _context.Mail.Add(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        /// <summary>
        /// Checking the status of sending a message
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>Error status</returns>
        public string ProcessDeliveryStatusNotification(MimeMessage message)
        {
            var report = message.Body as MultipartReport;

            if(report == null || report.ReportType == null || !report.ReportType.Equals("delivery-status", StringComparison.OrdinalIgnoreCase))
            {
                
                return null;
            }

            // process the report
            foreach(var mds in report.OfType<MessageDeliveryStatus>())
            {
                // process the status groups - each status group represents a different recipient

                // The first status group contains information about the message
                var envelopeId = mds.StatusGroups[0]["Original-Envelope-Id"];

                // all of the other status groups contain per-recipient information
                for(int i = 1; i < mds.StatusGroups.Count; i++)
                {
                    var recipient = mds.StatusGroups[i]["Original-Recipient"];
                    var action = mds.StatusGroups[i]["Action"];

                    if(recipient == null)
                        recipient = mds.StatusGroups[i]["Final-Recipient"];

                    // the recipient string should be in the form: "rfc822;user@domain.com"
                    var index = recipient.IndexOf(';');
                    var address = recipient.Substring(index + 1);

                    switch(action)
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

        /// <summary>
        /// Deletes a message from the database
        /// </summary>
        /// <param name="entity"> The Mail entity</param>
        public void Delete(Mail entity)
        {
            _context.Mail.Remove(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a message in the database
        /// </summary>
        /// <param name="entity"> The Mail entity</param>
        public void Update(Mail entity)
        {
            _context.Mail.Update(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Search for a message by id
        /// </summary>
        /// <param name="Id">ID of the Mail object</param>
        /// <returns></returns>
        public Mail FindById(int Id)
        {
            return _context.Mail.FirstOrDefault(e => e.Id == Id);
            
        }

        /// <summary>
        /// Searches for a list of all messages by a certain condition
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>IEnumerable<Mail> list of all found objects by predicate</returns>
        public IEnumerable<Mail> FindAll(Func<Mail,bool> predicate)
        {
            IEnumerable<Mail> mails = All
                .Where(predicate)
                .AsQueryable()
                .Include(x => x.Recipients)
                .ToList();
            return mails;
        }

        
    }
}
