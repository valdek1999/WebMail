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

namespace WebMail.Controllers
{
    /// <summary>
    /// Message Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MailsController : Controller
    {
        private IRepository<Mail> _contextMail { get; set; }

        /// <summary>
        /// The controller constructor that accepts the database context as input
        /// </summary>
        /// <param name="contextMail"></param>
        public MailsController(IRepository<Mail> contextMail)
        {
            _contextMail = contextMail;
        }

        /// <summary>
        /// GET request to output all messages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<Mail>> GetMail()
        {
            IEnumerable<Mail> mails = _contextMail.All;
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
            IEnumerable<Mail> mails = _contextMail.FindAll(x => x.Subject == subject);
            return Ok(mails);
        }
        /// <summary>
        /// POST request for a new message
        /// </summary>
        /// <param name="jsMail">json object</param>
        /// <returns>The Mail entity</returns>
        [HttpPost]
        public ActionResult<Mail> PostMail(JsonMail jsMail)
        {
            try
            {
                var context = _contextMail as MailRepository;
                Mail mail = context.Add(jsMail).Result;
                return Ok(mail);
            }
            catch(InvalidCastException ex)
            {
                return BadRequest(ex);
            }
        }
        
    }
}