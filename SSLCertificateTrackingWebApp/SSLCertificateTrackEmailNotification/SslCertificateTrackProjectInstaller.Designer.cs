
namespace SSLCertificateTrackEmailNotification
{
    partial class SslCertificateTrackProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SslCertificateTrackServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.SslCertificateTrackServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // SslCertificateTrackServiceProcessInstaller
            // 
            this.SslCertificateTrackServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SslCertificateTrackServiceProcessInstaller.Password = null;
            this.SslCertificateTrackServiceProcessInstaller.Username = null;
            // 
            // SslCertificateTrackServiceInstaller
            // 
            this.SslCertificateTrackServiceInstaller.Description = "Sends email with certificates that are about to expire.";
            this.SslCertificateTrackServiceInstaller.DisplayName = "SSL Certificate Track Email Notification";
            this.SslCertificateTrackServiceInstaller.ServiceName = "SSLCertificateTrackEmailNotification";
            this.SslCertificateTrackServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // SslCertificateTrackProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SslCertificateTrackServiceProcessInstaller,
            this.SslCertificateTrackServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SslCertificateTrackServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller SslCertificateTrackServiceInstaller;
    }
}