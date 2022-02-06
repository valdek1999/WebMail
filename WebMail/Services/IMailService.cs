using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMail.Models;

namespace WebMail.Services
{
    public interface IMailService
    {
        public Task<Mail> PublishMessage(MailBody mailBody);
    }
}
