using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace SSLCertificateTrackEmailNotification
{
    [RunInstaller(true)]
    public partial class SslCertificateTrackProjectInstaller : System.Configuration.Install.Installer
    {
        public SslCertificateTrackProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
