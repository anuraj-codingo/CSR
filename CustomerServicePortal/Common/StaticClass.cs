using CustomerServicePortal.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace CustomerServicePortal.Common
{
    public static class StaticClass
    {
        public static string SSN_DisplayForamatting(string SSN)
        {
            if (SSN.Length<9)
            {
                int length = 9 - SSN.Length;
                for (int i = 0; i < length; i++)
                {
                    SSN = "0" + SSN;
                }
                
            }
            return SSN;
          
        }
        public static decimal SSN_OriginalForamatting(decimal SSN)
        {
            decimal abs = Math.Abs(SSN);

            if ((int)(Math.Log10(decimal.ToDouble(abs)) + 1) >8)
            {
                SSN = SSN / 10;
            }
            return SSN;

        }
        public static string GetClientFromSSN(string SSN)
        {
            string Commandtext = "select * from [BICC_REPORTING].dbo.EMPYP where EMPSSN='" + SSN+"'";
            DBManager db = new DBManager("CustomerServicePortal");
         object Client=   db.GetScalarValue(Commandtext, CommandType.Text);
            if (Client==null)
            {
                Client = "ABC";
            }
            return Client.ToString();


        }
        public static async Task SendmailAsync(DataTable dt)
        {
            try
            {
                var senderEmail = new MailAddress("operator@abchldg.com", "ABC");              
                var receiverEmail = new MailAddress(dt.Rows[0]["Email"].ToString(), "Receiver");
                var password = "opsPass2019";
                var sub = "Login Details";
                var body = "HI {0}, <br><br> Username: " +
                    "{1}" +
                    "<br> Password: {2}<br><br>Thanks<br><br> &nbsp;";
                var smtp = new SmtpClient
                {
                    Host = "10.68.5.10",
                    Port = 25,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = string.Format(body, dt.Rows[0]["Name"].ToString(), dt.Rows[0]["Username"].ToString(), dt.Rows[0]["PassWord"].ToString()),
                    IsBodyHtml = true,
                })
                {
                    await smtp.SendMailAsync(mess);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}