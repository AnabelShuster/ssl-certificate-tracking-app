using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSLCertificateTrackingWebApp.Models
{
    public class CertificateInfo
    {
        [Key]
        public int CertificateID { get; set; }

        [Required]
        [ForeignKey("CertificateCategoryID")]
        [Display(Name = "Certificate Category")]
        public int CertificateCategoryID { get; set; }

        [Required]
        [Range(1, 2147483647)]        
        [Display(Name = "Work Order Number")]
        public int WorkOrderNumber { get; set; }

        [Display(Name = "Certificate Name")]
        public string CertificateName { get; set; }

        [Display(Name = "Common Name")]
        public string CommonName { get; set; }

        [Display(Name = "Certificate Type")]
        public string CertificateType { get; set; }

        [Display(Name = "Certificate Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime CertificateExpirationDate { get; set; }

        [Display(Name = "Server Name")]
        public string ServerName { get; set; }

        [Display(Name = "Certificate Template")]
        public string CertificateTemplate { get; set; }
        public string Hosted { get; set; }

        [Display(Name = "Subject Alternative Names")]
        public string SubjectAlternativeNames { get; set; }

        [Display(Name = "Web Server")]
        public string WebServer { get; set; }
    
        [Display(Name = "Servers Installed On")]
        public string ServersInstalledOn { get; set; }

        [Display(Name = "Operating System")]
        public string OperatingSystem { get; set; }
        public string Organization { get; set; }
        public string Department { get; set; }
        public string Requester { get; set; }
        [Display(Name = "Certificate Effective Date")]
        [DataType(DataType.Date)]
        public DateTime? CertificateEffectiveDate { get; set; }

        [Display(Name = "Application Name")]
        public string ApplicationName { get; set; }       

        [Display(Name = "Issued By")]
        public string IssuesBy { get; set; }

        [Display(Name = "Issued Email Address")]
        [DataType(DataType.EmailAddress)]
        public string IssuedEmailAddress { get; set; }

        [Display(Name = "Server Type")]
        public string ServerType { get; set; }

        [Display(Name = "Specialist Who Issued")]
        public string SpecialistWhoIssued { get; set; }

        [Display(Name = "CM Cert")]
        public bool CMCert { get; set; }

        [Display(Name = "Domain Member")]
        public bool DomainMember { get; set; }
        public string Approver { get; set; }

        [Display(Name = "Issued From New PKI")]
        public bool IssuedFromNewPKI { get; set; }
        public string Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RequestedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DeclinedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RevokedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReplacedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DiscoveredDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}
