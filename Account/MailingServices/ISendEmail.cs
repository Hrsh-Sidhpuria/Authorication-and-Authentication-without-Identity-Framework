namespace Authorization_Authentication.Account.MailingServices
{
    public interface ISendEmail
    {
        public string sendEmail(MailData data);
    }
}
