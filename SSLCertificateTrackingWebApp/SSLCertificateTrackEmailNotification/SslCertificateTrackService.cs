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
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Configuration;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace SSLCertificateTrackEmailNotification
{
    public partial class SslCertificateTrackService : ServiceBase
    {
        public SslCertificateTrackService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            this.WriteToFile("Start SSLCertificateTrackEmail service {0}");
            ScheduleService();
        }
        protected override void OnStop()
        {
            this.WriteToFile("SSLCertificateTrackEmailNotification service stopped.");
            this.Schedular.Dispose();
        }

        private Timer Schedular;
        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                this.WriteToFile("SSLCertificateTrackEmailNotification Service Mode: " + mode + " {0}");

                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;

                if (mode == "DAILY")
                {
                    //Get the Scheduled Time from AppSettings.
                    scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTime"]);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }

                if (mode.ToUpper() == "INTERVAL")
                {
                    //Get the Interval in Minutes from AppSettings.
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);

                    //Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                this.WriteToFile("SSLCertificateTrackEmailNotification Service scheduled to run after: " + schedule + " {0}");

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                this.WriteToFile("SSLCertificateTrackEmailNotification Service Error on: {0} " + ex.Message + ex.StackTrace);

                //Stop the Windows Service.
                using (ServiceController serviceController = new ServiceController("SSLCertificateTrackEmailNotification"))
                {
                    serviceController.Stop();
                }
            }
        }
        private void SchedularCallback(object e)
        {
            this.WriteToFile("SSLCertificateTrackEmailNotification Service Log Created {0}");
            SendMail();
            ScheduleService();
        }

        /// <summary>
        /// Sends email via the configured SMTP Server
        /// </summary>
        private void SendMail()
        {
            this.WriteToFile("Sending Email {0}");

            string emailRecipients = ConfigurationManager.AppSettings["EmailRecipients"];
     
            MailMessage emailMessage = new MailMessage()
              {
                    From = new MailAddress(ConfigurationManager.AppSettings["EmailSender"]),
                    IsBodyHtml = true,
                    Body = GetHtml(),
                    Subject = ConfigurationManager.AppSettings["EmailSubject"],
                };

            string[] allRecipients = emailRecipients.Trim().Split(';');
            foreach (string recipient in allRecipients)
            {
                emailMessage.To.Add(new MailAddress(recipient));
            }

            SmtpClient Client = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings["SMTPServer"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPServerPort"]),
                EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPServerEnableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTPServerUsername"], ConfigurationManager.AppSettings["SMTPServerPassword"])
            };

            Client.Send(emailMessage);      

            this.WriteToFile("SSL Certificate Email Sent successfully {0}");
         }

        /// <summary>
        /// Use to Format the HTML Body of the Email
        /// </summary>
        /// <returns></returns>
        private string GetHtml()
        {
            try
            {
                string messageBody = "<font>The following are the records: </font><br><br>";
                //if (grid.RowCount == 0) return messageBody;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Work Order Tracking #" + htmlTdEnd;
                messageBody += htmlTdStart + "Certificate Name" + htmlTdEnd;
                messageBody += htmlTdStart + "Certificate Expiration Date" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                //Loop all the rows from grid vew and added to html td  
               // for (int i = 0; i <= grid.RowCount - 1; i++)
               // {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + "WO# 58901452" + htmlTdEnd; //adding Work Order Tracking #
                    messageBody = messageBody + htmlTdStart + "Gmail Test Certificate" + htmlTdEnd; //adding Certificate Name  
                    messageBody = messageBody + htmlTdStart + "01/02/2022" + htmlTdEnd; //adding Certificate Expiration Date
                    messageBody = messageBody + htmlTrEnd;
               // }
                messageBody += htmlTableEnd;

                return messageBody; // return HTML Table as string from this function  
            }
            catch (Exception)
            {
                return "Exception has been encountered.  Please contact your system's administrator.";
            }
        }

        /// <summary>
        /// Create a log file to write different events
        /// </summary>
        /// <param name="text"></param>
        private void WriteToFile(string text)
        {
            string path = $"C:\\SSLCertTrackEmailServiceLog.log";

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

    }
}
