using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSLCertificateTrackingWebApp.Models;

namespace SSLCertificateTrackingWebApp.Data
{
    public class SSLCertificateTrackingWebAppContext : DbContext
    {
        public SSLCertificateTrackingWebAppContext (DbContextOptions<SSLCertificateTrackingWebAppContext> options)
            : base(options)
        {
        }

        public DbSet<SSLCertificateTrackingWebApp.Models.EmailServerConfiguration> EmailServerConfiguration { get; set; }
    }
}
