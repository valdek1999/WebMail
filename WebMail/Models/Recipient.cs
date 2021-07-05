using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebMail.Models
{
    /// <summary>
    /// The class of message recipients
    /// </summary>
    public class Recipient
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("recipient")]
        public String RecipientEmail { get; set; }

        [JsonIgnore]
        public int MailId { get; set; }
        [JsonIgnore]
        public Mail Mail { get; set; }


    }
}