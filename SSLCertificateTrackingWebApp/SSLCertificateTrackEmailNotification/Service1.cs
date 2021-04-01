using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SSLCertificateTrackEmailNotification
{
    public partial class Service1 : ServiceBase
    {
       EventLog IPSLog = new EventLog();

        public Service1()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("IPSMail"))
            {
                EventLog.CreateEventSource("IPSMail", "IPSScheduler");
            }

            IPSLog.Source = "IPSMail";
            IPSLog.Log = "IPSScheduler";
        }

        private void RunCodeAt(DateTime date)
        {
            IPSLog.WriteEntry("In RunCodeAt -- Calculating next date.");

            var dateNow = DateTime.Now;
            TimeSpan ts;
            if (date > dateNow)
            {
                ts = date - dateNow;
            }
            else
            {

            }
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
