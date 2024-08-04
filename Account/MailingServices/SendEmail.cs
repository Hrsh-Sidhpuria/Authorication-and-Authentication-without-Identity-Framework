using System.Net.Mail;
using System.Net;
using System.Text;

namespace Authorization_Authentication.Account.MailingServices
{
    public class SendEmail : ISendEmail
    {
        private readonly IConfiguration _configuration;

        public SendEmail(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public string sendEmail(MailData data)
        {

            // Set up SMTP client (In short setting up a connection.)
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("hrshsidhpuria173@gmail.com", "hgdofknskhxwtocj"),
                EnableSsl = true
            };

            // Create email message to send to the 
            string Emailcode = SetConfirmationCode();
            data.Body = "this is one time OTP for Email conformaion , Your OTP is <b>" + Emailcode + "</b>";
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("hrshsidhpuria173@gmail.com");
            mailMessage.To.Add(data.Email);
            mailMessage.Subject = data.Subject;
            mailMessage.IsBodyHtml = true;
            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendFormat("<h1Email Conformation </h1>");
            mailBody.AppendFormat("<br />");
            mailBody.AppendFormat(data.Body);
            mailBody.AppendFormat("<p>Thank you </p>");
            mailMessage.Body = mailBody.ToString();

            // Send email to that particular user
            client.Send(mailMessage);
            return Emailcode;
        }



        public string SetConfirmationCode()
        {
            string code = "";
            for (int i = 0; i < 6; i++)
            {
                Random random = new Random();
                int c = random.Next(0, 9);
                code = code + c;
            }
            return code;
        }
    }
}
