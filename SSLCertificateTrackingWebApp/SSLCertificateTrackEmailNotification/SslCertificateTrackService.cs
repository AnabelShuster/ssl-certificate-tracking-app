using System;
using System.ServiceProcess;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Configuration;
using Timer = System.Threading.Timer;
using System.Data.SqlClient;
using System.Collections.Generic;

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
        private List<Dictionary<string, string>> rows;
        private Dictionary<string, string> column;
        private SmtpClient smtpClient;
        private MailMessage emailMessage;

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

        private SqlCommand CreateSqlCommand(string selectStatement, SqlConnection sqlConnect)
        {
            SqlCommand cmd = new SqlCommand(selectStatement, sqlConnect);
            return cmd;
        }
        /// <summary>
        /// Gets smtp server information from database or app.config if no rows return
        /// Sends email via smtp
        /// </summary>
        private void PrepareAndSendEmail()
        {
            try
            {
                string connetionString = ConfigurationManager.ConnectionStrings["SSLCertificateTrackingWebAppContext"].ConnectionString;
                SqlConnection cnn = new SqlConnection(connetionString);
                WriteToFile("Connecting to Database {0}");
                cnn.Open();

                WriteToFile("Start getting Certificate Information from Database {0}");
                string certificateInfoSelectAll = "  SELECT t2.CertificateCategoryName, t1.WorkOrderNumber, t1.CertificateExpirationDate FROM CertificateInfo AS t1 LEFT JOIN CertificateCategory AS t2 on t1.CertificateCategoryID = t2.CertificateCategoryID";
                SqlCommand certificateInfoSqlCommand = CreateSqlCommand(certificateInfoSelectAll, cnn); ;
                SqlDataReader certifiateInfoDataReader = certificateInfoSqlCommand.ExecuteReader();
    
                rows = new List<Dictionary<string, string>>();
                column = new Dictionary<string, string>();

                WriteToFile("Getting Certificate Info from Database {0}");

                while (certifiateInfoDataReader.Read())
                {
                    column = new Dictionary<string, string>
                    {
                        ["CertificateCategoryName"] = certifiateInfoDataReader["CertificateCategoryName"].ToString(),
                        ["WorkOrderNumber"] = certifiateInfoDataReader["WorkOrderNumber"].ToString(),
                        ["CertificateExpirationDate"] = certifiateInfoDataReader["CertificateExpirationDate"].ToString()
                    };
                    rows.Add(column);
                }
                certifiateInfoDataReader.Close();

                WriteToFile("Start getting SMTP Server Information from Database {0}");
                string emailServerConfigSelectAll = "SELECT * FROM EmailServerConfiguration";
                SqlCommand emailServerConfigSqlCommand = CreateSqlCommand(emailServerConfigSelectAll, cnn); ;
                SqlDataReader emailServerConfigDataReader = emailServerConfigSqlCommand.ExecuteReader();
                emailServerConfigDataReader.Read();

                if (emailServerConfigDataReader.HasRows)
                {
                    WriteToFile("Getting Email Configuration from Database {0}");
                    smtpServer = emailServerConfigDataReader["SMTPServer"].ToString();
                    smtpPort = emailServerConfigDataReader["Port"].ToString();
                    smtpUsername = emailServerConfigDataReader["Username"].ToString();
                    smtpPassword = PasswordDecryptionUtil.Decrypt(emailServerConfigDataReader["Password"].ToString());
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

                emailMessage = new MailMessage()
                {
                    From = new MailAddress(emailSender),
                    IsBodyHtml = true,
                    Body = GetHtml(rows),
                    Subject = emailSubject,
                };

                allRecipients = emailRecipients.Trim().Split(';');

                foreach (string recipient in allRecipients)
                {
                    emailMessage.To.Add(new MailAddress(recipient));
                }

                smtpClient = new SmtpClient
                {
                    Host = smtpServer,
                    Port = Convert.ToInt32(smtpPort),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                WriteToFile("Sending Email {0}");

                smtpClient.Send(emailMessage);

                WriteToFile("SSL Certificate Email Sent successfully {0}");
                emailServerConfigDataReader.Close();
                emailServerConfigSqlCommand.Dispose();
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
        private string GetHtml(List<Dictionary<string, string>> rows)
        {
            try
            {
                int expireNumDays = Convert.ToInt32(ConfigurationManager.AppSettings["ExpireDeliveryDays"]);
                DateTime aboutToExpireDate = DateTime.Now.AddDays(expireNumDays);
                string messageBody = $"<font>The following are certificate(s) expiring in {expireNumDays}(s) or have expired: </font><br><br>";
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
                messageBody += htmlTdStart + "Certificate Category" + htmlTdEnd;
                messageBody += htmlTdStart + "Work Order Tracking #" + htmlTdEnd;
                messageBody += htmlTdStart + "Certificate Expiration Date" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                //Loop all the rows from grid vew and added to html td  
                foreach(Dictionary<string, string> column in rows)
                {
                    DateTime expireDate = DateTime.Parse(column["CertificateExpirationDate"]);

                    if (expireDate <= aboutToExpireDate)
                    {
                        messageBody += htmlTrStart;
                        messageBody = messageBody + htmlTdStart + column["CertificateCategoryName"] + htmlTdEnd; 
                        messageBody = messageBody + htmlTdStart + column["WorkOrderNumber"] + htmlTdEnd;  
                        messageBody = messageBody + htmlTdStart + Convert.ToDateTime(column["CertificateExpirationDate"]).ToString("MM/dd/yyyy") + htmlTdEnd;
                        messageBody += htmlTrEnd;
                        WriteToFile($"INSIDE IF.  CATEGORY NAME: {column["CertificateCategoryName"]}, WO#: {column["WorkOrderNumber"]}, EXPIRE DATE:{column["CertificateExpirationDate"] }");
                    }
                }
                messageBody += htmlTableEnd;

                return messageBody; // return HTML Table as string from this function  
            }
            catch (Exception ex)
            {
                WriteToFile($"ERROR WITH HTML GENERATION: {ex}");
                return $"Exception has been encountered.  Please contact your system's administrator. /n {ex}";
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
