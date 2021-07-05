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
    public struct JsonMail
    {
        public String Subject { get; set; }
        public String Body { get; set; }
        public List<String> Recipients { get; set; }

    }
    /// <summary>
    /// Message Class
    /// </summary>
    public class Mail
    {
        public int Id { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public String Result { get; set; }
        public String FailedMessage { get; set; }
        public DateTime Date { get; set; }

        public virtual List<Recipient> Recipients { get; set; }
    }
    
}
