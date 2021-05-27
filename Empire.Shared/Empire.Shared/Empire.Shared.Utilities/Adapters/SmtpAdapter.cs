using System;
using System.Linq;
using System.Net.Mail;

namespace Empire.Shared.Utilities
{
    public class SmtpAdapter
    {
        #region  methods
        public void SendEmail(string sEmailBody, string sSubject = null)
        {

            try
            {
                var oMailSettings = AppSettings.Current.Mail;
                string[] aToAddresses = oMailSettings.ToAddress.Split(';');

                using (SmtpClient oSmtpClient = new SmtpClient())
                {
                    oSmtpClient.Timeout = 10000;

                    using (MailMessage oMailMessage = new MailMessage())
                    {
                        oMailMessage.Subject = String.IsNullOrWhiteSpace(sSubject) ? oMailSettings.Subject : sSubject;
                        oMailMessage.From = new MailAddress(oMailSettings.FromAddress, oMailSettings.DisplayName);

                        aToAddresses.ToList().ForEach(sAddress => oMailMessage.To.Add(new MailAddress(sAddress)));

                        oMailMessage.Body = sEmailBody;
                        oMailMessage.IsBodyHtml = true;
                        oSmtpClient.Send(oMailMessage);
                    }
                }
            }
            catch (Exception oEx)
            {
                oEx.Innermost().Log();
            }
        }


    #endregion

    }

}

