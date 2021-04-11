using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SSLCertificateTrackingWebApp.Models
{
    public class CertificateCategory
    {
        [Key]
        [Display(Name = "Category ID")] //May not be necessary
        public int CertificateCategoryID { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CertificateCategoryName { get; set; }

    }
}
