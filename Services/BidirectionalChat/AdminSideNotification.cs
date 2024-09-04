using Microsoft.AspNetCore.SignalR;

namespace Authorization_Authentication.Services.BidirectionalChat
{
    /*admin side notification*/
    public class AdminSideNotification :Hub
    {
            public async Task SendMessage(string sender, string message)
            {
                await Clients.All.SendAsync("ReceiveMessageFromUser", sender, message);
            }
    }
}
