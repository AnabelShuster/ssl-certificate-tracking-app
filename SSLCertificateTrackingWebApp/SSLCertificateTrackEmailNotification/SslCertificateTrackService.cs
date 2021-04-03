using System;
using System.ServiceProcess;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Configuration;
using Timer = System.Threading.Timer;
using System.Data.SqlClient;

namespace SSLCertificateTrackEmailNotification
{
    public partial class SslCertificateTrackService : ServiceBase
    {
        //Email Server fields
        private string smtpServer;
        private string smtpPort;
        private string smtpUsername;
        private string smtpPassword;
        private string emailRecipients;
        private string[] allRecipients;
        private string emailSender;
        private string emailSubject;

        //Scheduler field(s)
        private Timer Schedular;
        public SslCertificateTrackService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Start SSLCertificateTrackEmail service {0}");
            ScheduleService();
        }
        protected override void OnStop()
        {
            WriteToFile("SSLCertificateTrackEmailNotification service stopped.");
            Schedular.Dispose();
        }
        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                WriteToFile("SSLCertificateTrackEmailNotification Service Mode: " + mode + " {0}");

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

                WriteToFile("SSLCertificateTrackEmailNotification Service scheduled to run after: " + schedule + " {0}");

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToFile("SSLCertificateTrackEmailNotification Service Error on: {0} " + ex.Message + ex.StackTrace);

                //Stop the Windows Service.
                using (ServiceController serviceController = new ServiceController("SSLCertificateTrackEmailNotification"))
                {
                    serviceController.Stop();
                }
            }
        }
        private void SchedularCallback(object e)
        {
            WriteToFile("SSLCertificateTrackEmailNotification Service Log Created {0}");
            PrepareAndSendEmail();
            ScheduleService();
        }

        /// <summary>
        /// Gets smtp server information from database or app.config if no rows return
        /// Sends email via smtp
        /// </summary>
        private void PrepareAndSendEmail()
        {
            try
            {
                WriteToFile("Start getting SMTP Server Information from Database {0}");
                string sql = "SELECT * FROM EmailServerConfiguration";
                string connetionString = ConfigurationManager.ConnectionStrings["SSLCertificateTrackingWebAppContext"].ConnectionString;

                SqlConnection cnn = new SqlConnection(connetionString);
                WriteToFile("Connecting to Database {0}");
                cnn.Open();

                SqlCommand cmd = new SqlCommand(sql, cnn);

                SqlDataReader dataReader = cmd.ExecuteReader();
                dataReader.Read();

                if (dataReader.HasRows)
                {
                    WriteToFile("Getting Email Configuration from Database {0}");
                    smtpServer = dataReader["SMTPServer"].ToString();
                    smtpPort = dataReader["Port"].ToString();
                    smtpUsername = dataReader["Username"].ToString();
                    smtpPassword = PasswordDecryptionUtil.Decrypt(dataReader["Password"].ToString());
                }
                else
                {
                    WriteToFile("No information found in database. Getting Email Configuration from .config {0}");
                    smtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                    smtpPort = ConfigurationManager.AppSettings["SMTPServerPort"];
                    smtpUsername = ConfigurationManager.AppSettings["SMTPServerUsername"];
                    smtpPassword = ConfigurationManager.AppSettings["SMTPServerPassword"];
                }

                emailRecipients = ConfigurationManager.AppSettings["EmailRecipients"];
                emailSender = ConfigurationManager.AppSettings["EmailSender"];
                emailSubject = ConfigurationManager.AppSettings["EmailSubject"];

                MailMessage emailMessage = new MailMessage()
                {
                    From = new MailAddress(emailSender),
                    IsBodyHtml = true,
                    Body = GetHtml(),
                    Subject = emailSubject,
                };

                allRecipients = emailRecipients.Trim().Split(';');

                foreach (string recipient in allRecipients)
                {
                    emailMessage.To.Add(new MailAddress(recipient));
                }

                SmtpClient Client = new SmtpClient
                {
                    Host = smtpServer,
                    Port = Convert.ToInt32(smtpPort),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                WriteToFile("Sending Email {0}");

                Client.Send(emailMessage);

                WriteToFile("SSL Certificate Email Sent successfully {0}");
                dataReader.Close();
                cmd.Dispose();
                cnn.Close();
            }
            catch (Exception ex)
            {
                WriteToFile("SSLCertificateTrackEmailNotification Service Error on: {0} " + ex.Message + ex.StackTrace);
            }
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
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

    }
}
