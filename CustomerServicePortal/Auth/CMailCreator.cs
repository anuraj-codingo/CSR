using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CustomerServicePortal.Auth
{
    public class CMailCreator
    {
        private string sFromAddress = string.Empty;
        private string sPassword = string.Empty;
        private string sCCMailID = string.Empty;
        private string sBCCMailID = string.Empty;
        private string sHost = string.Empty;
        private int iPort = int.MinValue;
        private string sSubject = string.Empty;
        private string sMessageBody = string.Empty;
        private string sToMail = string.Empty;
        private int iRetryCount = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dtMailConfig"></param>
        public CMailCreator()
        {
            sFromAddress = ConfigurationManager.AppSettings.Get("FromAddress").ToString();
            sPassword = ConfigurationManager.AppSettings.Get("Password").ToString();
            sHost = ConfigurationManager.AppSettings.Get("Host").ToString();
            iPort = int.Parse(ConfigurationManager.AppSettings.Get("Port").ToString());
            sBCCMailID = ConfigurationManager.AppSettings.Get("BCCMailID").ToString();
            sCCMailID = ConfigurationManager.AppSettings.Get("CCMailID").ToString();
            sToMail = ConfigurationManager.AppSettings.Get("ToMail").ToString();
        }

        /// <summary>
        /// SendMails
        /// </summary>
        /// <returns></returns>
        public bool SendMail(System.Data.DataTable dtDetails, DateTime dtmTransDate, decimal dcHours, int iAction)
        {
            bool bMailStatus = false;

            //iRetryCount = 3 ;

            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient
                {
                    EnableSsl = false,
                    UseDefaultCredentials = false,
                    Host = sHost,
                    Port = iPort,

                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    //Credentials = new System.Net.NetworkCredential( sFromAddress, sPassword ),
                    Credentials = new System.Net.NetworkCredential(sFromAddress, sPassword, "abchldg.com"),
                    Timeout = 60000,
                };

                sMessageBody = dtDetails.Rows[0]["EmployeeName"].ToString() +
                               " had ##Action## a timesheet transaction for date : " + dtmTransDate.ToString("dd/MM/yyyy") +
                               "\r\nTask   : " + dtDetails.Rows[0]["TaskName"].ToString() +
                               "\r\nClient : " + dtDetails.Rows[0]["ClientName"].ToString() +
                               "\r\nHours  : " + string.Format("{0:0.00}", dcHours);

                if (iAction == 0 /*Create*/ )
                {
                    sSubject = "Timesheet | New Transaction";
                    sMessageBody = sMessageBody.Replace("##Action##", "created");
                }
                else if (iAction == 1 /*Update*/ )
                {
                    sSubject = "Timesheet | Update Transaction";
                    sMessageBody = sMessageBody.Replace("##Action##", "updated");
                }
                else if (iAction == 2 /*Delete*/ )
                {
                    sSubject = "Timesheet | Delete Transaction";
                    sMessageBody = sMessageBody.Replace("##Action##", "deleted");
                }

                System.Net.Mail.MailMessage MailMessage = new System.Net.Mail.MailMessage();
                MailMessage.From = new System.Net.Mail.MailAddress(sFromAddress);
                MailMessage.Subject = sSubject;
                MailMessage.Body = sMessageBody;

                //To mail
                string[] sToAddress = sToMail.Split(',');

                for (int i = 0; i < sToAddress.Length; ++i)
                    MailMessage.To.Add(new System.Net.Mail.MailAddress(sToAddress[i]));

                //CC
                if (sCCMailID != string.Empty)
                {
                    string[] sCC = sCCMailID.Split(',');

                    for (int i = 0; i < sCC.Length; ++i)
                        MailMessage.CC.Add(new System.Net.Mail.MailAddress(sCC[i]));
                }

                //BCC
                if (sBCCMailID != string.Empty)
                {
                    string[] sBCC = sBCCMailID.Split(',');

                    for (int i = 0; i < sBCC.Length; ++i)
                        MailMessage.Bcc.Add(new System.Net.Mail.MailAddress(sBCC[i]));
                }

                do
                {
                    try
                    {
                        client.Send(MailMessage);

                        bMailStatus = true;
                    }

                    catch (System.Exception ex)
                    {
                        iRetryCount--;
                        bMailStatus = false;
                        LogError(ex);
                    }
                }
                while (!bMailStatus && iRetryCount != 0);
            }
            catch (System.Exception ex)
            {
                bMailStatus = false;
                LogError(ex);
            }

            return bMailStatus;
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="ex"></param>
        private void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            string sDirectory = System.IO.Path.GetPathRoot(Environment.SystemDirectory) + "OneSmarter\\taskt";

            if (!System.IO.Directory.Exists(sDirectory)) System.IO.Directory.CreateDirectory(sDirectory);

            string path = sDirectory + "\\ErrorLog.txt";

            if (!System.IO.File.Exists(path))
            {
                System.IO.FileStream f = System.IO.File.Create(path);
                f.Close();
            }

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}