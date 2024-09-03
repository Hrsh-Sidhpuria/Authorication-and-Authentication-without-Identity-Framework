using Authorization_Authentication.Services.ServicesModel;
using NuGet.Protocol.Plugins;


namespace Authorization_Authentication.Services
{
    public interface ITicketManagementServices
    {
        public Task<bool> AddToTicketReplies(string sender, string chat,string ticketId);
        public Task<string> issueticket(Tickets tickets);

        public Task<List<Modules>> GetModuleList();
        public Task<List<Tickets>> GetticketsList(String UserId);

        public Task<Tickets> getTicketDetail(string TicketId);

        public Task<List<Tickets>> GetAllTickets();

        public Task<List<ChatViewModel>> GetChatMessagesAsync(string ticketId);

        public Task<bool> AddReply(string sender ,string ticketId,string reply);

        public Task<bool> UpdateStatus(string status, string ticketId);

        public Task<string> getStatusFromTicketId(string ticketId);

        public Task<bool> UploadChatImage(string ticketId, string sender, string message, string imageUrl);

       public string GetIdfromTicketId(string TicketId);
    }
}
