using Microsoft.EntityFrameworkCore.Storage;

namespace Authorization_Authentication.Services.ServicesModel
{
    public class ChatViewModel
    {
        public string TicketId { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public string ImageUrl { get; set; } 
        public bool IsImage { get; set; }
    }
}
