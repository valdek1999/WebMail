using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WebMail.Models
{
    /// <summary>
    /// POST request body class
    /// </summary>
    public class MailBody
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Recipients { get; set; }

    }
    /// <summary>
    /// Message Class
    /// </summary>
    public class Mail
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Result { get; set; }
        public string FailedMessage { get; set; }
        public DateTime Date { get; set; }
        public virtual List<Recipient> Recipients { get; set; }
    }

    /// <summary>
    /// The class of message recipients
    /// </summary>
    public class Recipient
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("recipient")]
        public string RecipientEmail { get; set; }

        [JsonIgnore]
        public int MailId { get; set; }
        [JsonIgnore]
        public Mail Mail { get; set; }
    }

}
