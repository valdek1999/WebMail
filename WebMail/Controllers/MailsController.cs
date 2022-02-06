using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMail.Data;
using WebMail.Models;
using WebMail.Repository;
using WebMail.Services;

namespace WebMail.Controllers
{
    /// <summary>
    /// Message Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MailsController : Controller
    {
        private IRepository<Mail> _repositoryMail;
        private IMailService _mailService;

        public MailsController(IRepository<Mail> repositoryMail, IMailService mailService)
        {
            _repositoryMail = repositoryMail;
            _mailService = mailService;
        }

        /// <summary>
        /// GET request to output all messages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<Mail>> GetMail()
        {
            IEnumerable<Mail> mails = _repositoryMail.All;
            return Ok(mails);
        }

        /// <summary>
        /// GET a request to search for a message by the subject of the message
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>The list mails</returns>
        [HttpGet("{subject}")]
        public ActionResult<IEnumerable<Mail>> GetMail(string subject)
        {
            IEnumerable<Mail> mails = _repositoryMail.FindAll(x => x.Subject == subject);
            return Ok(mails);
        }
        /// <summary>
        /// POST request for a new message
        /// </summary>
        /// <param name="mailBody">json object</param>
        /// <returns>The Mail entity</returns>
        [HttpPost]
        public ActionResult<Mail> PostMail(MailBody mailBody)
        {
            try
            {
                var mail = _mailService.PublishMessage(mailBody).Result;
                _repositoryMail.Add(mail);
                return Ok(mail);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
        
    }
}