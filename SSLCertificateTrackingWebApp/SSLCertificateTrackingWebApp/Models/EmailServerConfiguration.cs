using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SSLCertificateTrackingWebApp.Models
{
    public class EmailServerConfiguration
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "SMTP Server")]
        public string SMTPServer { get; set; }

        [Required]
        public string Port { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
