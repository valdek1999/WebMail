using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMail.Config
{
    /// <summary>
    /// The essence of the smtp configuration
    /// </summary>
    public class MailConfig
    {
        /// <summary>
        /// Your mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Your Password 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Adress of the SMTP host 
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port of the host
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Does the smtp host use SSL
        /// </summary>
        public bool UseSsl { get; set; }
    }
}
