namespace Authorization_Authentication.Services.ServicesModel
{
    public class ReplyViewModel
    {
            public string TicketId { get; set; }
            public string Sender { get; set; }
            public string Message { get; set; }
            public IFormFile Image { get; set; }
    }
}
